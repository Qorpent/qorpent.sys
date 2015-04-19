using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.IO.Http;
using Qorpent.Mvc;
using Qorpent.Serialization;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class UsonHandler : RequestHandlerBase{
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


	    public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
	        CancellationToken cancel) {
                var path = request.Uri.AbsolutePath;
                if (path.StartsWith("/~"))
                {
                    AsynchronousBegin(request,response);
                }
                else if (path.StartsWith("/!"))
                {
                    AsynchronousEnd(request, response);
                }
                else if (path.StartsWith("/-"))
                {
                    RunTrace(request, response);
                }
                else
                {
                    RunSynchronous(request, response);
                }
	    }

	    private void RunTrace(HttpRequestDescriptor request, HttpResponseDescriptor response) {
            try {
#pragma warning disable 219
                var sb = new StringBuilder();
                var sw = Stopwatch.StartNew();
                var parameters = PrepareParameters(request,response);
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
               response.Finish(sb.ToString());
            } catch (Exception ex) {
                response.Finish(ex.ToString(), "text/plain", 500);
            }
	    }
 #pragma warning restore 219
	    private void AsynchronousEnd(HttpRequestDescriptor request, HttpResponseDescriptor response){
			if (null == currentAsyncCall)
			{
				response.Finish("no asynchronous task ever started", "text/plain", 500);
			}
			currentAsyncCall.Wait();
			if (currentAsyncCall.IsFaulted)
			{
				response.Finish("last call to async fail with " + currentAsyncCall.Exception, "text/plain", 500);
			}
			else if (currentAsyncCall.IsCanceled)
			{
				response.Finish("last call to async was cancelled", "text/plain", 500);
			}
			else
			{
				if (request.Uri.AbsolutePath.EndsWith("/xml"))
				{
					response.Finish(((UObj)currentAsyncCall.Result).ToXmlString(), "text/xml");
				}
				else
				{
					response.Finish(((UObj)currentAsyncCall.Result).ToJson(), "application/json");
				}
			}
		}

		private void AsynchronousBegin(HttpRequestDescriptor request, HttpResponseDescriptor response){
			
			if (null != currentAsyncCall){
				if (!currentAsyncCall.IsCompleted){
					response.Finish("{\"state\":\"run\"}");
					return;
				}
			}
			var parameters = PrepareParameters(request,response);
			currentAsyncCall = Task.Run(() => _handler(parameters));

			response.Finish("{\"state\":\"start\"}");
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

		private static UObj PrepareParameters(HttpRequestDescriptor request, HttpResponseDescriptor response){
			var uri = request.Uri;
			UObj parameters = null;
		    
			var q = "";
			if (uri.Query.Length > 1){
				q = Uri.UnescapeDataString(uri.Query.Substring(1));
			}
			if (q.StartsWith("{") && q.EndsWith("}")){
				parameters = q.ToUson();
			}
			else{
				var ps = RequestParameters.Create(request).GetParameters();
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

		private void RunSynchronous(HttpRequestDescriptor request, HttpResponseDescriptor response){
			try{
				var parameters = PrepareParameters(request,response);
			    var obj = _handler(parameters);
                var result = obj.ToUson();
				if (request.Uri.AbsolutePath.EndsWith("/xml")){
                   
					response.Finish(result.ToXmlString(), "text/xml");
				}
				else{
					response.Finish(result.ToJson());
				}
			}
			catch (Exception ex){
				response.Finish(ex.ToString(), "text/plain", 500);
			}
		}
	}
}
