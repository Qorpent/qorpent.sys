using System;
using System.Net;
using System.Text;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host{
	internal class HostRequestHandler{
		private readonly CancellationToken _cancel;
		private readonly WebContext _context;
		private readonly HostServer _server;

		public HostRequestHandler(HostServer server, WebContext context){
			_server = server;
			_context = context;
			_cancel = _server._cancel;
		}

		public void Execute(){
            
			string callbackEndPoint = _context.GetHeader("qorpent-callback-endpoint");
			_context.SetHeader("Server", "Qorpent RESTFull Server 0.1");
			_context.ContentType = "application/json; charset=utf-8";
			if (String.IsNullOrWhiteSpace(callbackEndPoint)){
				ProcessSyncRequest();
			}
			else{
				ProcessAsyncRequest(callbackEndPoint);
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
            _context.Finish("true");
		}

		private void FailRequest(Exception exception){
            _context.Finish(exception.ToString(),"text/plain",500);
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
            _server.Router.Route(_context);
			var handler = _server.Factory.GetHandler(_server, _context, callbackEndPoint);
			handler.Run(_server, _context, callbackEndPoint, _cancel);
		    if (!_context.Response.WasClosed) {
		        _context.Response.Finish("");
		    }
		}
	}
}