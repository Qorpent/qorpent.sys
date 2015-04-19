using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Qorpent.Host{
	internal class HostRequestHandler{
		private readonly CancellationToken _cancel;
		private readonly HttpListenerContext _context;
		private readonly HostServer _server;

		public HostRequestHandler(HostServer server, HttpListenerContext context){
			_server = server;
			_context = context;
			_cancel = _server._cancel;
		}

		public void Execute(){
			_server.Auth.Authenticate(_context,_context);
			string callbackEndPoint = _context.Request.Headers.Get("qorpent-callback-endpoint");
			_context.Response.Headers["Server"] = "Qorpent RESTFull Server 0.1";
			_context.Response.ContentType = "application/json; charset=utf-8";
			CopyCookies();
			if (String.IsNullOrWhiteSpace(callbackEndPoint)){
				ProcessSyncRequest();
			}
			else{
				ProcessAsyncRequest(callbackEndPoint);
			}
		}

		private void CopyCookies(){
			foreach (Cookie cookie in _context.Request.Cookies){
				if (cookie.Name == _server.Config.AuthCookieName){
					continue;
				}
				_context.Response.Cookies.Add(cookie);
			}
		}


		private void ProcessAsyncRequest(string callbackEndPoint){
			try{
				FinishAsyncRequest();
				ProcessRequest(callbackEndPoint);
			}
			catch (Exception e){
				FailRequest(e);
			}
		}


		private void FinishAsyncRequest(){
			_context.Response.StatusCode = 200;
			byte[] content = Encoding.UTF8.GetBytes("OK");
			_context.Response.OutputStream.Write(content, 0, content.Length);
			_context.Response.Close();
		}

		private void FailRequest(Exception exception){
			_context.Response.StatusCode = 500;
			byte[] content = Encoding.UTF8.GetBytes(exception.ToString());
			_context.Response.OutputStream.Write(content, 0, content.Length);
			_context.Response.Close();
		}

		private void ProcessSyncRequest(){
			try{
				ProcessRequest(null);
			}
			catch (Exception e){
				FailRequest(e);
			}
		}

		private void ProcessRequest(string callbackEndPoint){
			_server.RequestCount++;
			IRequestHandler handler = _server.Factory.GetHandler(_server, _context,_context, callbackEndPoint);
			handler.Run(_server, _context,_context, callbackEndPoint, _cancel);
		}
	}
}