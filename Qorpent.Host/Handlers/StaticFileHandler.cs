using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Qorpent.IO;
using Qorpent.IO.Http;

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


	    public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
	        CancellationToken cancel) {
                var abspath = request.Uri.AbsolutePath;
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
                var staticdescriptor = server.Static.Get(abspath, request);
                //в случае, если запрошен HTML и он отсутствует, то в качестве результата возвращаем стартуовую страницу 
                //указанного в начале имени приложения (для этого в видимости должен находится скрипт с контроллерами приложения
                if (null == staticdescriptor && abspath.EndsWith(".html"))
                {
                    RunApplication(server, request,response, callbackEndPoint, cancel, abspath);
                    return;
                }
                if (null == staticdescriptor && abspath.EndsWith("-starter.js"))
                {
                    RunApplicationStarter(server, request, response, callbackEndPoint, cancel, abspath);
                    return;
                }
                if (null == staticdescriptor)
                {
                    FinishWirh404(response);
                    return;
                }
                Finish200(server, request, response, staticdescriptor);
	    }

	    private void RunApplicationStarter(IHostServer server, HttpRequestDescriptor request,HttpResponseDescriptor response, string callbackEndPoint, CancellationToken cancel, string abspath){
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath).Split('-')[0];
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists)
				{

					var template = server.Static.Get("template.starter.js", request).Read();

					var apphtml = template.Replace("__APPNAME__", appname);
					_applicationCache[abspath] = new FixedWebFileRecord(abspath, "text/javascript", apphtml);
				}
				else
				{
					_applicationCache[abspath] = null;
				}
			}
			Finish(server,request,response,abspath);
		}

        private static void Finish200(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, IWebFileRecord staticdescriptor)
        {
			var filetime = response.SetLastModified(staticdescriptor.Version);
            if (filetime <= request.GetIfModifiedSince())
            {
				response.Finish("", status: 304);
			}
			else{
				response.SetHeader("Qorpent-Disposition", staticdescriptor.FullName);
				if (server.Config.Cached.Contains(Path.GetFileName(staticdescriptor.FullName))){
                    response.SetHeader("Cache-Control", "public, max-age=86400");
				}
				else if (server.Config.ForceNoCache) {
                    response.SetHeader("Cache-Control", "no-cache, must-revalidate");
				}
				else {
                    response.SetHeader("Cache-Control", "public");
				}
				if (staticdescriptor.IsFixedContent){
					if (null != staticdescriptor.FixedData){
						response.StatusCode = 200;
                        response.StatusDescription = "OK";
                        response.ContentType = staticdescriptor.MimeType;
                        response.Stream.Write(staticdescriptor.FixedData, 0, staticdescriptor.FixedData.Length);

					}
					else{
						response.Finish(staticdescriptor.FixedContent, staticdescriptor.MimeType + "; charset=utf-8");
					}
				}
				else{
					using (var s = staticdescriptor.Open()){
						response.Finish(s, staticdescriptor.MimeType + "; charset=utf-8");
						s.Close();
					}
				}
			}
		}

        private static void FinishWirh404(HttpResponseDescriptor response)
        {
			response.Finish("no file found", "text/plain; charset=utf-8", 404);
		}

		//кэш страниц, являющихся приложениями
		static IDictionary<string, IWebFileRecord> _applicationCache = new Dictionary<string, IWebFileRecord>();
        private void RunApplication(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint, CancellationToken cancel, string abspath)
        {
			if (!_applicationCache.ContainsKey(abspath)){
				var appname = Path.GetFileNameWithoutExtension(abspath);
				var appexists = server.Static.Get(appname + "_controllers.js") != null;
				if (appexists){

					var template = server.Static.Get("template.app.html", request).Read();

					var apphtml = string.Format(template, appname);
					_applicationCache[abspath] = new FixedWebFileRecord(abspath,"text/html",apphtml);
				}
				else{
					_applicationCache[abspath] = null;
				}
			}

			Finish(server,request,response, abspath);
		}

        private static void Finish(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string abspath)
        {
			if (null == _applicationCache[abspath]){
				FinishWirh404(response);
			}
			else{
				Finish200(server, request,response, _applicationCache[abspath]);
			}
		}
	}
}
