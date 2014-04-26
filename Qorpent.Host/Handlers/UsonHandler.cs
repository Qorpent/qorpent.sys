using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Mvc;
using Qorpent.Serialization;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class UsonHandler : IRequestHandler{
		/// <summary>
		/// 
		/// </summary>
		private  Task<object> currentAsyncCall = null;
		private Func<dynamic, object> _handler;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="executeDelegate"></param>
		public UsonHandler(Func<dynamic,object> executeDelegate)
		{
			this._handler = executeDelegate;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel){
			var path = callcontext.Request.Url.AbsolutePath;
			if (path.StartsWith("/~")){
				AsynchronousBegin(callcontext);
			}
			else if (path.StartsWith("/!")){
				AsynchronousEnd(callcontext);
			} 
            else if (path.StartsWith("/-")) {
                RunTrace(callcontext);
            }
			else{
				RunSynchronous(callcontext);
			}
		}

		private void RunTrace(HttpListenerContext callcontext) {
            try {
                var sb = new StringBuilder();
                var sw = Stopwatch.StartNew();
                var parameters = PrepareParameters(callcontext);
                sw.Stop();
                sb.AppendLine("Prepare: " + sw.Elapsed);
                sw = Stopwatch.StartNew();
                var result = _handler(parameters);
                sw.Stop();
                sb.AppendLine("Execute: "+sw.Elapsed);
                sw = Stopwatch.StartNew();
                var uson = result.ToUson();
                sw.Stop();
                sb.AppendLine("Usonify: " + sw.Elapsed);
                sw = Stopwatch.StartNew();
                var json = uson.ToJson();
                sw.Stop();
                sb.AppendLine("Jsonify: " + sw.Elapsed);
                sw = Stopwatch.StartNew();
                json = new JsonSerializer().Serialize("", result);
                sw.Stop();
                sb.AppendLine("Jsonify 2: " + sw.Elapsed);
               callcontext.Finish(sb.ToString());
            } catch (Exception ex) {
                callcontext.Finish(ex.ToString(), "text/plain", 500);
            }
	    }

	    private void AsynchronousEnd(HttpListenerContext callcontext){
			if (null == currentAsyncCall)
			{
				callcontext.Finish("no asynchronous task ever started", "text/plain", 500);
			}
			currentAsyncCall.Wait();
			if (currentAsyncCall.IsFaulted)
			{
				callcontext.Finish("last call to async fail with " + currentAsyncCall.Exception, "text/plain", 500);
			}
			else if (currentAsyncCall.IsCanceled)
			{
				callcontext.Finish("last call to async was cancelled", "text/plain", 500);
			}
			else
			{
				if (callcontext.Request.Url.AbsolutePath.EndsWith("/xml"))
				{
					callcontext.Finish(((UObj)currentAsyncCall.Result).ToXmlString(), "text/xml");
				}
				else
				{
					callcontext.Finish(((UObj)currentAsyncCall.Result).ToJson(), "application/json");
				}
			}
		}

		private void AsynchronousBegin(HttpListenerContext callcontext){
			
			if (null != currentAsyncCall){
				if (!currentAsyncCall.IsCompleted){
					callcontext.Finish("{\"state\":\"run\"}");
					return;
				}
			}
			var parameters = PrepareParameters(callcontext);
			currentAsyncCall = Task.Run(() => _handler(parameters));

			callcontext.Finish("{\"state\":\"start\"}");
		}

		private static UObj PrepareParameters(Uri uri){
			UObj parameters = null;
			var q = "";
			if (uri.Query.Length > 1)
			{
				q = Uri.UnescapeDataString(uri.Query.Substring(1));
			}
			if (q.StartsWith("{") && q.EndsWith("}"))
			{
				parameters = q.ToUson();
			}
			RefineParameters(parameters);
			return parameters;
		}

		private static UObj PrepareParameters(HttpListenerContext callcontext){
			var uri = callcontext.Request.Url;
			UObj parameters = null;
		    
			var q = "";
			if (uri.Query.Length > 1){
				q = Uri.UnescapeDataString(uri.Query.Substring(1));
			}
			if (q.StartsWith("{") && q.EndsWith("}")){
				parameters = q.ToUson();
			}
			else{
				var ps = new Utils.RequestDataRetriever(callcontext.Request).GetRequestData().GetParameters();
				if (ps.ContainsKey("__postdata")){
					var pd = ps["__postdata"].Trim();
					if (pd.StartsWith("{") && pd.EndsWith("}")){
						parameters = pd.ToUson();
					}
					ps.Remove("__postdata");
				}

				if (null == parameters){
					parameters = ps.ToUson();
				}
			}
			RefineParameters(parameters);
			return parameters;
		}

		private static void RefineParameters(UObj parameters){
			foreach (var p in parameters.Properties.ToArray()){
				var str = p.Value as string;
				if (null != str){
					var toint = str.ToInt();
					if ("0" == str || 0 != toint){
						parameters.Properties[p.Key] = toint;
					}
					else if ("true" == str){
						parameters.Properties[p.Key] = true;
					}
					else if ("false" == str){
						parameters.Properties[p.Key] = false;
					}
				}
				else if (p.Value is decimal){
					var d = (decimal) p.Value;
					if (Math.Round(d, 0) == d){
						if (d > int.MaxValue){
							parameters.Properties[p.Key] = (long) d;
						}
						else{
							parameters.Properties[p.Key] = (int) d;
						}
					}
				}
			}
			parameters.IgnoreCase = true;
		}

		private void RunSynchronous(HttpListenerContext callcontext){
			try{
				var parameters = PrepareParameters(callcontext);
			    var obj = _handler(parameters);
                var result = obj.ToUson();
				if (callcontext.Request.Url.AbsolutePath.EndsWith("/xml")){
                   
					callcontext.Finish(result.ToXmlString(), "text/xml");
				}
				else{
					callcontext.Finish(result.ToJson(), "application/json");
				}
			}
			catch (Exception ex){
				callcontext.Finish(ex.ToString(), "text/plain", 500);
			}
		}
	}
}
