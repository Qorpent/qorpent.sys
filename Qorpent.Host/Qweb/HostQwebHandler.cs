﻿using System;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Qorpent.IO.Http;
using Qorpent.Mvc;
using Qorpent.Mvc.HttpHandler;
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
                var context = server.Application.Container.Get<IMvcContext>(null, server, ctx, callbackEndPoint, cancel);
                context.NotModified = false;
                try
                {
                    if (!Authorize(server, context)) return;
                    BindContext(context);                  
                    Execute(context);
                    if (context.NotModified)
                    {
                        MvcHandler.ProcessNotModified(context);
                       ctx.Response.Close();
                    }
                    else
                    {
                        MvcHandler.SetModifiedHeader(context);
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

	    private bool Authorize(IHostServer server, IMvcContext context) {
	        if (!string.IsNullOrWhiteSpace(context.ActionDescriptor.Role)) {
	            if (!context.ActionDescriptor.Role.Contains("GUEST")) {
                   
	                var roles = context.ActionDescriptor.Role.SmartSplit();
                    var rr = server.Container.Get<IRoleResolver>();
	                if (rr.IsInRole(context.User, "ADMIN")) return true;
                    foreach (var role in roles) {
                        if (role.StartsWith("!")) {
                            if (rr.IsInRole(context.User, role.Substring(1))) {
                                ProcessError(context, new SecurityException("access denied"));
                                return false;
                            }
                        }
                        else {
                            if (!rr.IsInRole(context.User, role)) {
                                ProcessError(context, new SecurityException("access denied"));
                                return false;
                            }
                        }
                    }
	                
	                
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

			context.NotModified = MvcHandler.CanReturnNotModifiedStatus(context);
		}

		

		private void BindContext(IMvcContext context)
		{
			if (context.RenderName == "form") return;
			context.Bind();
		}
	}
}