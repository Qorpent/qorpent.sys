using System;
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
        

        public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
            CancellationToken cancel) {
                try
                {


                    string path = request.Uri.AbsolutePath;
                    var srvDef = server.Config.Proxize.FirstOrDefault(_ => path.StartsWith(_.Key)).Value;
                    var srv = HostUtils.ParseUrl(srvDef);
                    var req = (HttpWebRequest)WebRequest.Create(new Uri(new Uri(srv), request.Uri.PathAndQuery));

                    foreach (var header in request.Headers.Keys)
                    {
                        var v = request.Headers[header];
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
                            req.Headers[header] = request.Headers[header];
                        }
                    }
                    req.Method = request.Method;
                    req.Headers["QHPROXYORIGIN"] = Uri.EscapeDataString(request.Uri.ToString());


                    ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;
                    if (req.Method == "POST")
                    {

                        using (var reqStream = req.GetRequestStream())
                        {
                            while ((bytesRead = request.Stream.Read(buffer, 0, buffer.Length)) != 0)
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
                            response.ContentType = v;
                        }
                        response.SetHeader(header,resp.Headers[header]);
                    }
                    using (var resstream = resp.GetResponseStream())
                    {
                        while ((bytesRead = resstream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            response.Stream.Write(buffer, 0, bytesRead);
                            response.Stream.Flush();
                        }
                    }
                    response.Close();
                }
                catch (Exception ex)
                {
                    response.Finish("Error: " + ex, status: 500);
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