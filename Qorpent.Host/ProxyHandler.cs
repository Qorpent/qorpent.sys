using System;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host {
    /// <summary>
    /// Проксирует 
    /// </summary>
    public class ProxyHandler : RequestHandlerBase {
        

        public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
                try
                {


                    string path = context.Uri.AbsolutePath;
                    var srvDef = server.Config.Proxize.FirstOrDefault(_ => path.StartsWith(_.Key)).Value;
                    var srv = HostUtils.ParseUrl(srvDef);
                    var req = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(srv), context.Uri.PathAndQuery));

                    foreach (var _header in context.Request.Headers) {
                        var v = _header.Value;
                        var header = _header.Key;
                        if (header == "Host")
                        {
                            //  req.Host = "127.0.0.1";
                        }
                        else if (header == "User-Agent")
                        {
                            req.UserAgent = v;
                        }
                        else if (header == "Accept")
                        {
                            req.Accept = v;
                        }
                        else if (header == "Content-Type")
                        {
                            req.ContentType = v;
                        }
                        else if (header == "Referer")
                        {
                            req.Referer = v;
                        }
                        else if (header == "Connection")
                        {
                            if (v == "Keep-Alive")
                            {
                                req.KeepAlive = true;
                            }
                            else if (v == "Close")
                            {
                                continue;
                            }

                        }
                        else if (header == "Content-Length")
                        {
                            // req.ContentLength = v.ToInt();
                            continue;
                        }
                        else if (header == "Expect")
                        {
                            // req.ContentLength = v.ToInt();
                            continue;
                        }

                        else
                        {
                            req.Headers[header] = context.Request.Headers[header];
                        }
                    }
                    req.Method = context.Method;
                    req.Headers["QHPROXYORIGIN"] = Uri.EscapeDataString(context.Uri.ToString());


                    ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    if (req.Method == "POST")
                    {

                        using (var reqStream = req.GetRequestStream())
                        {
                            while ((bytesRead = context.Request.Stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                reqStream.Write(buffer, 0, bytesRead);
                            }
                            reqStream.Flush();
                            reqStream.Close();
                        }
                    }
                    var resp = req.GetResponse();
                    foreach (var header in resp.Headers.AllKeys)
                    {
                        var v = resp.Headers[header];
                        if (header == "Transfer-Encoding")
                        {
                            continue;
                        }
                        if (header == "Content-Type")
                        {
                            context.Response.ContentType = v;
                        }
                        context.Response.SetHeader(header,resp.Headers[header]);
                    }
                    using (var resstream = resp.GetResponseStream()) {
                        var stream = resstream;
                        if ((resp.Headers["Content-Encoding"]??"").Contains("gzip")) {
                            stream = new GZipStream(stream,CompressionLevel.Optimal);    
                        }
                        stream.CopyTo(context.Response.Stream);
                        
                    }
                    context.Response.Close();
                }
                catch (Exception ex)
                {
                    context.Finish("Error: " + ex, status: 500);
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
                }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}