using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Threading;
using Qorpent.IO;
using Qorpent.IO.Http;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class StaticFileHandler : RequestHandlerBase
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


	    public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
	        CancellationToken cancel) {
                var abspath = context.Uri.AbsolutePath;
                if (string.IsNullOrWhiteSpace(abspath) || abspath == "/")
                {
                    if (!string.IsNullOrWhiteSpace(server.Config.DefaultPage))
                    {
                        abspath = server.Config.DefaultPage;
                    }
                    else
                    {
                        abspath = DefaultPage;
                    }
                }
                var staticdescriptor = server.Static.Get(abspath, context);
	            
                //в случае, если запрошен HTML и он отсутствует, то в качестве результата возвращаем стартуовую страницу 
                //указанного в начале имени приложения (для этого в видимости должен находится скрипт с контроллерами приложения
                if (null == staticdescriptor && abspath.EndsWith(".html"))
                {
                    RunApplication(server, context, callbackEndPoint, cancel, abspath);
                    return;
                }
                if (null == staticdescriptor && abspath.EndsWith("-starter.js"))
                {
                    RunApplicationStarter(server, context, callbackEndPoint, cancel, abspath);
                    return;
                }
                if (null == staticdescriptor)
                {
                    FinishWirh404(context);
                    return;
                }
                if (!string.IsNullOrWhiteSpace(staticdescriptor.Role)) {
                    var roles = server.Container.Get<IRoleResolver>();
                    if (!roles.IsInRole(context.User, staticdescriptor.Role)) {
                        throw new SecurityException("access denied");
                    }
                }
             //   context.Response.AddHeader("Accept-Ranges","bytes");
                Finish200(server, context, staticdescriptor);
	    }

	    private void RunApplicationStarter(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel, string abspath){
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath).Split('-')[0];
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists)
				{

					var template = server.Static.Get("template.starter.js", context).Read();

					var apphtml = template.Replace("__APPNAME__", appname);
					_applicationCache[abspath] = new FixedWebFileRecord(abspath, "text/javascript", apphtml);
				}
				else
				{
					_applicationCache[abspath] = null;
				}
			}
			Finish(server,context,abspath);
		}

        private static void Finish200(IHostServer server, WebContext context, IWebFileRecord staticdescriptor) {

			var filetime = context.SetLastModified(staticdescriptor.Version);
            if (filetime <= context.GetIfModifiedSince())
            {
				context.Finish("", status: 304);
			}
			else{
				context.SetHeader("Qorpent-Disposition", staticdescriptor.FullName);
				if (server.Config.Cached.Contains(Path.GetFileName(staticdescriptor.FullName))){
                    context.SetHeader("Cache-Control", "public, max-age=86400");
				}
				else if (server.Config.ForceNoCache) {
                    context.SetHeader("Cache-Control", "no-cache, must-revalidate");
				}
				else {
                    context.SetHeader("Cache-Control", "public");
				}

                RangeDescriptor range = null;
                if (0 < staticdescriptor.Length && (staticdescriptor.MimeType.StartsWith("image/")||staticdescriptor.MimeType.StartsWith("video/"))) {
                    context.Response.SetHeader("Content-Length",staticdescriptor.Length.ToString());
                    if (staticdescriptor.Length > 4096) {
                        context.Response.SetHeader("Accept-Ranges","bytes");
                    }
                    var rangeHeader = context.Request.GetHeader("Range");
                    if (!string.IsNullOrWhiteSpace(rangeHeader)) {
                        var rangeparts = rangeHeader.Substring(6).SmartSplit(false,true,'-');
                        range = new RangeDescriptor {Total = staticdescriptor.Length};
                        range.Finish = range.Total - 1;
                        range.Start = rangeparts[0].ToInt();
                        if (rangeparts.Count > 1) {
                            range.Finish = rangeparts[1].ToInt();
                        }

                    }
                }

                try {

                    if (staticdescriptor.IsFixedContent) {
                        if (null != staticdescriptor.FixedData) {
                            context.Finish(staticdescriptor.FixedData, staticdescriptor.MimeType, range: range);
                        }
                        else {
                            context.Finish(staticdescriptor.FixedContent, staticdescriptor.MimeType, range: range);
                        }
                    }
                    else {
                        using (var s = staticdescriptor.Open()) {
                            context.Finish(s, staticdescriptor.MimeType, range: range);

                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.GetType().Name);
                }
                finally {
                    context.Response.Close();
                }
            }
		}

        private static void FinishWirh404(WebContext context)
        {
			context.Finish("no file found", "text/plain; charset=utf-8", 404);
		}

		//кэш страниц, являющихся приложениями
		static IDictionary<string, IWebFileRecord> _applicationCache = new Dictionary<string, IWebFileRecord>();
        private void RunApplication(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel, string abspath)
        {
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath);
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists){

					var template = server.Static.Get("template.app.html", context).Read();

					var apphtml = string.Format(template, appname);
					_applicationCache[abspath] = new FixedWebFileRecord(abspath,"text/html",apphtml);
				}
				else{
					_applicationCache[abspath] = null;
				}
			}

			Finish(server,context, abspath);
		}

        private static void Finish(IHostServer server, WebContext context, string abspath)
        {
			if (null == _applicationCache[abspath]){
				FinishWirh404(context);
			}
			else{
				Finish200(server, context, _applicationCache[abspath]);
			}
		}
	}
}
