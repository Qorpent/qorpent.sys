using System;
using System.IO;
using System.Net;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers{
    /// <summary>
	/// Хэндлер для вики-страниц
	/// </summary>
	public class WikiHandler:RequestHandlerBase{
		

        public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
                var code = context.Uri.Query.Replace("?", "");
                string wikicode = "*** страница с данным кодом не найдена ***";
                string tpl = "<html><body>${wikipage}</body></html>";
                var wikidesc = server.Static.Get(code + ".wiki", withextensions: true);
                if (null == wikidesc)
                {
                    wikidesc = server.Static.Get(code, withextensions: true);

                }
                if (null != wikidesc)
                {
                    using (var s = wikidesc.Open())
                    {
                        using (var r = new StreamReader(s))
                        {
                            wikicode = r.ReadToEnd();
                        }

                        if (!wikidesc.FullName.EndsWith(".wiki"))
                        {
                            if (wikidesc.FullName.EndsWith(".bxl") || wikidesc.FullName.EndsWith(".bxls") || wikidesc.FullName.EndsWith(".bsproj"))
                            {
                                wikicode = "[[code]]\r\n" + wikicode + "\r\n[[/code]]\r\n[[script-last type=bxl]]";
                            }
                            else if (wikidesc.FullName.EndsWith(".js") || wikidesc.FullName.EndsWith(".css") ||
                                     wikidesc.FullName.EndsWith(".cs"))
                            {
                                wikicode = "[[code]]\r\n" + wikicode + "\r\n[[/code]]";
                            }
                            else
                            {
                                wikicode = wikicode.Replace("\r\n", "\r\n\r\n ");
                            }
                            wikicode = "= Файл: [href:" + wikidesc.FullName + "]\r\n\r\n" + wikicode;
                        }
                    }
                }
                var tpldesc = server.Static.Get("wiki.html");
                if (null != tpldesc)
                {
                    using (var s = tpldesc.Open())
                    {
                        using (var r = new StreamReader(s))
                        {
                            tpl = r.ReadToEnd();
                        }
                    }
                }
                var wikipage = tpl.Replace("${wikipage}", wikicode);
                context.Finish(wikipage, "text/html");
        }
    }
}