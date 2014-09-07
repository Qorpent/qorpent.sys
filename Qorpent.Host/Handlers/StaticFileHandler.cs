﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Qorpent.Host.Static;
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
	            if (!string.IsNullOrWhiteSpace(server.Config.DefaultPage)){
		            abspath = server.Config.DefaultPage;
	            }
	            else{
		            abspath = DefaultPage;
	            }
            }
			var staticdescriptor = server.Static.Get(abspath, callcontext);
			//в случае, если запрошен HTML и он отсутствует, то в качестве результата возвращаем стартуовую страницу 
			//указанного в начале имени приложения (для этого в видимости должен находится скрипт с контроллерами приложения
			if (null == staticdescriptor && abspath.EndsWith(".html")){
				RunApplication(server,callcontext, callbackEndPoint, cancel, abspath);
				return;
			}
			if (null == staticdescriptor && abspath.EndsWith("-starter.js"))
			{
				RunApplicationStarter(server, callcontext, callbackEndPoint, cancel, abspath);
				return;
			}
			if (null == staticdescriptor){
				FinishWirh404(callcontext);
				return;
			}
			Finish200(server,callcontext, staticdescriptor);
		}

		private void RunApplicationStarter(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel, string abspath){
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath).Split('-')[0];
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists)
				{

					var template = server.Static.Get("template.starter.js", callcontext).Read();

					var apphtml = template.Replace("__APPNAME__", appname);
					_applicationCache[abspath] = new FixedContentDescriptor(apphtml, abspath){MimeType = "text/javascript"};
				}
				else
				{
					_applicationCache[abspath] = null;
				}
			}
			Finish(server,callcontext,abspath);
		}

		private static void Finish200(IHostServer server, HttpListenerContext callcontext, StaticContentDescriptor staticdescriptor){
			var filetime = callcontext.SetLastModified(staticdescriptor.GetLastVersion());
			if (filetime <= callcontext.GetIfModifiedSince()){
				callcontext.Finish("", status: 304);
			}
			else{
				callcontext.Response.AddHeader("Qorpent-Disposition", staticdescriptor.FullName);
				if (server.Config.Cached.Contains(Path.GetFileName(staticdescriptor.FullName))){
					callcontext.Response.AddHeader("Cache-Control","public, max-age=86400");
				}
				else if (server.Config.ForceNoCache){
					callcontext.Response.AddHeader("Cache-Control", "no-cache, must-revalidate");
				}
				if (staticdescriptor.IsFixedContent){
					if (null != staticdescriptor.FixedData){
						callcontext.Response.StatusCode = 200;
						callcontext.Response.StatusDescription = "OK";
						callcontext.Response.ContentType = staticdescriptor.MimeType;
						callcontext.Response.OutputStream.Write(staticdescriptor.FixedData, 0, staticdescriptor.FixedData.Length);

					}
					else{
						callcontext.Finish(staticdescriptor.FixedContent, staticdescriptor.MimeType + "; charset=utf-8");
					}
				}
				else{
					using (var s = staticdescriptor.Open()){
						callcontext.Finish(s, staticdescriptor.MimeType + "; charset=utf-8");
						s.Close();
					}
				}
			}
		}

		private static void FinishWirh404(HttpListenerContext callcontext){
			callcontext.Finish("no file found", "text/plain; charset=utf-8", status: 404);
		}

		//кэш страниц, являющихся приложениями
		static IDictionary<string, StaticContentDescriptor> _applicationCache = new Dictionary<string, StaticContentDescriptor>();
		private void RunApplication(IHostServer server, HttpListenerContext context, string callbackEndPoint, CancellationToken cancel, string abspath){
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath);
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists){

					var template = server.Static.Get("template.app.html", context).Read();

					var apphtml = string.Format(template, appname);
					_applicationCache[abspath] = new FixedContentDescriptor(apphtml, abspath){MimeType = "text/html"};
				}
				else{
					_applicationCache[abspath] = null;
				}
			}

			Finish(server,context, abspath);
		}

		private static void Finish(IHostServer server,HttpListenerContext context, string abspath){
			if (null == _applicationCache[abspath]){
				FinishWirh404(context);
			}
			else{
				Finish200(server, context, _applicationCache[abspath]);
			}
		}
	}
}
