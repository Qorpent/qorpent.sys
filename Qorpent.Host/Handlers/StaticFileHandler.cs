using System;
using System.Linq;
using System.Net;
using System.Threading;
using Qorpent.Host.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class StaticFileHandler : IRequestHandler
	{
        /// <summary>
        /// 
        /// </summary>
        public StaticFileHandler() {
            DefaultPage = "/app.html";
        }
		/// <summary>
		/// 
		/// </summary>
		public static readonly DateTime ResourceVersion = new DateTime(2014,2,4);
        /// <summary>
        /// 
        /// </summary>
        public string DefaultPage { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel){
			var abspath = callcontext.Request.Url.AbsolutePath;
            if (string.IsNullOrWhiteSpace(abspath) || abspath == "/") {
                abspath = DefaultPage;
            }
			var staticdescriptor = server.Static.Get(abspath, callcontext);
			if (null == staticdescriptor){
				callcontext.Finish("no file found", "text/plain; charset=utf-8", status: 404);
				return;
			}
			var filetime = callcontext.SetLastModified(staticdescriptor.GetLastVersion());
			if (filetime <= callcontext.GetIfModifiedSince())
			{
				callcontext.Finish("", status: 304);
			}
			else
			{
				callcontext.Response.AddHeader("Qorpent-Disposition",staticdescriptor.FullName);
				if (staticdescriptor.IsFixedContent){
					callcontext.Finish(staticdescriptor.FixedContent, staticdescriptor.MimeType + "; charset=utf-8");

				}
				else{
				    using (var s = staticdescriptor.Open()) {
				        callcontext.Finish(s, staticdescriptor.MimeType + "; charset=utf-8");
                        s.Close();
				    }

				}
				
			}
		}
	}
}
