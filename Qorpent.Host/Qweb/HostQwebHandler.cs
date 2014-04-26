using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Qorpent.Mvc;
using Qorpent.Mvc.HttpHandler;

namespace Qorpent.Host.Qweb
{
	/// <summary>
	/// Qweb-handler для HostServer
	/// </summary>
	public class HostQwebHandler:IRequestHandler	
	{
		/// <summary>
		/// Выполняет указанный запрос
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel)
		{
			SetCurrentUser(server, callcontext);
			callcontext.Response.ContentEncoding = Encoding.UTF8;
			callcontext.Response.Headers["Content-Encoding"] = "utf-8";
			var context = server.Application.Container.Get<IMvcContext>(null,server, callcontext, callbackEndPoint, cancel);
			context.NotModified = false;
			try
			{
				BindContext(context);
				Execute(context);
				if (context.NotModified)
				{
					MvcHandler.ProcessNotModified(context);
					context.Output.Close();
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

		private static void SetCurrentUser(IHostServer server, HttpListenerContext callcontext)
		{
			var identity = callcontext.Request.Headers.Get("Qorpent-Impersonate");
			if (string.IsNullOrWhiteSpace(identity))
			{
				identity = "admin";
			}

			server.Application.Principal.SetCurrentUser(
				new GenericPrincipal(new GenericIdentity(identity), null)
				);
		}

		private void RenderResult(IMvcContext context)
		{
			context.Render();
			context.Output.Flush();
			context.Output.Close();
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
			context.Output.Close();
		}

		private void ExecuteWithoutResult(IMvcContext context)
		{
			CheckNotModified(context);
			context.AuthrizeResult = Security.AuthorizationResult.OK;
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