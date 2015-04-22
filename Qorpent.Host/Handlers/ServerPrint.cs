using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using Qorpent.IO.Http;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Handlers {
    /// <summary>
    /// 
    /// </summary>
    public class ServerPrint:RequestHandlerBase {
        static  object sync = new object();
        
        public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
                lock (sync)
                {
                    var config = server.Config.Definition;
                    var pdfprintpath =
                        EnvironmentInfo.ResolvePath(config.Attr("pdfprintpath",
                            "@repos@/../bin/pdf/bullzip/API/EXE/config.exe"));
                    var mozillapath =
                        EnvironmentInfo.ResolvePath(config.Attr("mozillapath", "@repos@/../bin/firefox/firefox.exe"));
                    var reportpath = EnvironmentInfo.ResolvePath(config.Attr("reportpath", "@repos@/.reports"));
                    Directory.CreateDirectory(reportpath);
                    var dict = context.Uri.Query.Split('&')
                        .Select(_ => _.Split('='))
                        .ToDictionary(_ => _[0], _ => Uri.UnescapeDataString(_.Length == 1 ? "1" : _[1]));
                    var reporturl = dict["reporturl"];
                    var title = dict["title"];
                    bool cached = false;
                    if (dict.ContainsKey("cached"))
                    {
                        cached = dict["cached"].ToBool();
                    }

                    var hashFileName = Path.Combine(reportpath, (reporturl + "_" + title).GetMd5() + ".pdf");

                    if (!cached || !File.Exists(hashFileName))
                    {
                        if (File.Exists(hashFileName))
                        {
                            File.Delete(hashFileName);
                        }
                        Process.Start(pdfprintpath, "/S Output \"" + hashFileName + "\"").WaitForExit();
                        Process.Start(pdfprintpath, "/S ShowSettings never").WaitForExit();
                        Process.Start(pdfprintpath, "/S ShowPdf no").WaitForExit();
                        Process.Start(pdfprintpath, "/S confirmoverwrite no").WaitForExit();
                        var p = Process.Start(mozillapath, "-no-remote -height 300 -width 300  -p dev -url \"" + reporturl + "\"");
                        var now = DateTime.Now;
                        while ((DateTime.Now - now).TotalSeconds < 30)
                        {
                            Thread.Sleep(300);
                            if (!File.Exists(hashFileName)) continue;
                            if (new FileInfo(hashFileName).Length > 50000)
                            {
                                break;
                            }
                        }
                        p.CloseMainWindow();
                        p.Close();
                        Process.Start(pdfprintpath, "/C ").WaitForExit();
                    }

                    if (!File.Exists(hashFileName))
                    {
                        throw new Exception("some errors in report generation " + hashFileName);
                    }

                    var pseudofileName = title.ToSafeFileName() + ".pdf";


                    context.SetHeader("Content-Disposition", "attachment; filename*=UTF-8''" + Uri.EscapeDataString(pseudofileName));

                    using (var s = File.OpenRead(hashFileName))
                    {
                        context.Finish(s, "application/pdf; charset=utf-8");
                        s.Close();
                    }


                }
        }
    }
}