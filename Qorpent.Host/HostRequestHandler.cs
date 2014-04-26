using System;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent.Host
{
	internal class HostRequestHandler
	{
		private HostServer _server;
		private HttpListenerContext _context;
		private CancellationToken _cancel;

		public HostRequestHandler(HostServer server,HttpListenerContext context)
		{
			_server = server;
			_context = context;
			_cancel = _server._cancel;
		}

		public void Execute()
		{
			var callbackEndPoint = _context.Request.Headers.Get("qorpent-callback-endpoint");
			_context.Response.Headers["Server"] = "Qorpent RESTFull Server 0.1";
			_context.Response.ContentType = "application/json; charset=utf-8";
			if (String.IsNullOrWhiteSpace(callbackEndPoint))
			{
				ProcessSyncRequest();
			}
			else
			{
				ProcessAsyncRequest(callbackEndPoint);
			}
				
		}

		



		private void ProcessAsyncRequest(string callbackEndPoint)
		{
			try
			{
				FinishAsyncRequest();
				ProcessRequest(callbackEndPoint);
			}
			catch (Exception e)
			{
				FailRequest(e);
			}
		}


		private void FinishAsyncRequest()
		{
			_context.Response.StatusCode = 200;
			var content = Encoding.UTF8.GetBytes("OK");
			_context.Response.OutputStream.Write(content, 0, content.Length);
			_context.Response.Close();
		}

		private void FailRequest(Exception exception)
		{
			_context.Response.StatusCode = 500;
			var content = Encoding.UTF8.GetBytes(exception.ToString());
			_context.Response.OutputStream.Write(content, 0, content.Length);
			_context.Response.Close();
		}

		private void ProcessSyncRequest()
		{
			try
			{
				ProcessRequest(null);
			}
			catch (Exception e)
			{
				FailRequest(e);
			}
		}

		private void ProcessRequest(string callbackEndPoint)
		{
			_server.RequestCount++;
			var handler = _server.Factory.GetHandler(_server, _context, callbackEndPoint);
			handler.Run(_server, _context, callbackEndPoint, _cancel);
			
		}
	}
}