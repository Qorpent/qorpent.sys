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


	    public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
	        CancellationToken cancel) {
                var path = context.Uri.AbsolutePath;
                if (path.StartsWith("/~"))
                {
                    AsynchronousBegin(context);
                }
                else if (path.StartsWith("/!"))
                {
                    AsynchronousEnd(context);
                }
                else if (path.StartsWith("/-"))
                {
                    RunTrace(context);
                }
                else
                {
                    RunSynchronous(context);
                }
	    }

	    private void RunTrace(WebContext context) {
            try {
#pragma warning disable 219
                var sb = new StringBuilder();
                var sw = Stopwatch.StartNew();
                var parameters = PrepareParameters(context);
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
               context.Finish(sb.ToString());
            } catch (Exception ex) {
                context.Finish(ex.ToString(), "text/plain", 500);
            }
	    }
 #pragma warning restore 219
	    private void AsynchronousEnd(WebContext context){
			if (null == currentAsyncCall)
			{
				context.Finish("no asynchronous task ever started", "text/plain", 500);
			}
			currentAsyncCall.Wait();
			if (currentAsyncCall.IsFaulted)
			{
				context.Finish("last call to async fail with " + currentAsyncCall.Exception, "text/plain", 500);
			}
			else if (currentAsyncCall.IsCanceled)
			{
				context.Finish("last call to async was cancelled", "text/plain", 500);
			}
			else
			{
				if (context.Uri.AbsolutePath.EndsWith("/xml"))
				{
					context.Finish(((UObj)currentAsyncCall.Result).ToXmlString(), "text/xml");
				}
				else
				{
					context.Finish(((UObj)currentAsyncCall.Result).ToJson(), "application/json");
				}
			}
		}

		private void AsynchronousBegin(WebContext context){
			
			if (null != currentAsyncCall){
				if (!currentAsyncCall.IsCompleted){
					context.Finish("{\"state\":\"run\"}");
					return;
				}
			}
			var parameters = PrepareParameters(context);
			currentAsyncCall = Task.Run(() => _handler(parameters));

			context.Finish("{\"state\":\"start\"}");
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

		private static UObj PrepareParameters(WebContext context){
			var uri = context.Uri;
			UObj parameters = null;
		    
			var q = "";
			if (uri.Query.Length > 1){
				q = Uri.UnescapeDataString(uri.Query.Substring(1));
			}
			if (q.StartsWith("{") && q.EndsWith("}")){
				parameters = q.ToUson();
			}
			else{
				var ps = RequestParameters.Create(context).GetParameters();
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

		private void RunSynchronous(WebContext context){
			try{
				var parameters = PrepareParameters(context);
			    var obj = _handler(parameters);
                var result = obj.ToUson();
				if (context.Uri.AbsolutePath.EndsWith("/xml")){
                   
					context.Finish(result.ToXmlString(), "text/xml");
				}
				else{
					context.Finish(result.ToJson());
				}
			}
			catch (Exception ex){
				context.Finish(ex.ToString(), "text/plain", 500);
			}
		}
	}
}
