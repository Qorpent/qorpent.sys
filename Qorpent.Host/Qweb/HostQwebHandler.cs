using System;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using qorpent.v2.security.authorization;
using Qorpent.IO.Http;
using Qorpent.Mvc;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Qweb
{
	/// <summary>
	/// Qweb-handler для HostServer
	/// </summary>
	public class HostQwebHandler:RequestHandlerBase	
	{
		

	    public override void Run(IHostServer server, WebContext ctx, string callbackEndPoint,
	        CancellationToken cancel) {
	        if (null != ctx.User) {
                SetCurrentUser(server, ctx.User);
	        }
            ctx.ContentEncoding = Encoding.UTF8;
            ctx.SetHeader("Content-Encoding", "utf-8");
                var context = server.Container.Get<IMvcContext>(null, server, ctx, callbackEndPoint, cancel);
                context.NotModified = false;
                try
                {
                    if (!Authorize(server, context)) return;
                    BindContext(context);
                    var action = context.ActionDescriptor.Action as ActionBase;
                    if (null != action) {
                        action.HostServer = server;
                        action.WebContext = ctx;
                        action.WebContextParameters = RequestParameters.Create(ctx);
                    }
                    Execute(context);
                    if (context.NotModified) {
                        context.StatusCode = 304;
                       ctx.Response.Close();
                    }
                    else
                    {
                        SetModifiedHeader(context);
                        RenderResult(context);

                    }
                }
                catch (Exception ex)
                {
                    ProcessError(context, ex);
                }
                finally
                {
                    context.Release();
                }

	    }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public static void SetModifiedHeader(IMvcContext context)
        {
            INotModifiedStateProvider cp = null;
            if (null != context.RenderDescriptor && context.RenderDescriptor.SupportNotModifiedState)
            {
                cp = context.RenderDescriptor;
            }
            else if (null == context.ActionDescriptor)
            {
                return;
            }
            cp = cp ?? context.ActionDescriptor;
            if (!cp.SupportNotModifiedState)
            {
                return;
            }
            context.LastModified = cp.LastModified;
            context.Etag = cp.ETag;
        }

	    private bool Authorize(IHostServer server, IMvcContext context) {
	        if (!string.IsNullOrWhiteSpace(context.ActionDescriptor.Role)) {
	            if (!context.ActionDescriptor.Role.Contains("GUEST")) {
	                if (!context.User.Identity.IsAuthenticated) {
                        ProcessError(context, new SecurityException("guest permitted"));
	                }
	                var roles = context.ActionDescriptor.Role.SmartSplit();
                    var rr = server.Container.Get<IRoleResolverService>();
	                if (rr.IsInRole(context.User, "ADMIN")) return true;
                    foreach (var role in roles) {
                        if (role.StartsWith("!")) {
                            if (rr.IsInRole(context.User, role.Substring(1))) {
                                ProcessError(context, new SecurityException("access denied"));
                                return false;
                            }
                        }
                    }
	                foreach (var role in roles) {
	                    if (role.StartsWith("!")) continue;
	                    if (rr.IsInRole(context.User, role)) {
	                        return true;
	                    }

	                }
	                ProcessError(context, new SecurityException("access denied"));
                    return false;

	                
	            }
	        }
	        return true;
	    }

	    private static void SetCurrentUser(IHostServer server,IPrincipal user)
		{
			
			server.Application.Principal.SetCurrentUser(user);
		}

		private void RenderResult(IMvcContext context)
		{
			context.Render();
			context.Output.Flush();
            ((HostMvcContext)context).WebContext.Response.Close();
		}

		private void Execute(IMvcContext context)
		{
			if (!context.IgnoreActionResult)
			{
				EvaluateActionResult(context);
			}
			else
			{
				ExecuteWithoutResult(context);
			}
		}

		private void ProcessError(IMvcContext context, Exception ex)
		{
			context.StatusCode = 500;
			context.Error = ex;
			context.RenderError(ex);
            ((HostMvcContext)context).WebContext.Response.Close();
		}

		private void ExecuteWithoutResult(IMvcContext context)
		{
			CheckNotModified(context);
			context.AuthrizeResult = AuthorizationResult.OK;
		}

		private void EvaluateActionResult(IMvcContext context)
		{
			CheckNotModified(context);
			if (!context.NotModified)
			{
				context.Process();
			}
			
		}

		private void CheckNotModified(IMvcContext context)
		{

			context.NotModified = CanReturnNotModifiedStatus(context);
		}


        public static bool CanReturnNotModifiedStatus(IMvcContext context)
        {
            if (context.IfModifiedSince.Year <= 1900 && string.IsNullOrWhiteSpace(context.IfNoneMatch))
            {
                return false;
            }
            if (null != context.RenderDescriptor && context.RenderDescriptor.SupportNotModifiedState)
            {
                var rcp = (INotModifiedStateProvider)context.RenderDescriptor;
                return CheckbyLastModAndEtag(context, rcp.LastModified, rcp.ETag);
            }
            if (null == context.ActionDescriptor)
            {
                return false;
            }
            var cp = (INotModifiedStateProvider)context.ActionDescriptor;
            if (!cp.SupportNotModifiedState)
            {
                return false;
            }
            return CheckbyLastModAndEtag(context, cp.LastModified, cp.ETag);
        }

        private static bool CheckbyLastModAndEtag(IMvcContext context, DateTime lastmod, string etag)
        {
            if (context.IfModifiedSince < lastmod)
            {
                return false;
            }
            if (etag != context.IfNoneMatch)
            {
                return false;
            }
            return true;
        }

		

		private void BindContext(IMvcContext context)
		{
			if (context.RenderName == "form") return;
			context.Bind();
		}
	}
}