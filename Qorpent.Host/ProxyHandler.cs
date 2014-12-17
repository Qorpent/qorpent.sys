using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host {
    /// <summary>
    /// Проксирует 
    /// </summary>
    public class ProxyHandler : IRequestHandler {
        /// <summary>
        /// Выполняет HTTP запрос на другом указанном сервере
        /// </summary>
        /// <param name="server"></param>
        /// <param name="callcontext"></param>
        /// <param name="callbackEndPoint"></param>
        /// <param name="cancel"></param>
        public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint,
            CancellationToken cancel) {
                try
                {

                    
            string path = callcontext.Request.Url.AbsolutePath;
            var srvDef = server.Config.Proxize.FirstOrDefault(_ => path.StartsWith(_.Key)).Value;
            var srv = HostUtils.ParseUrl(srvDef);
            var req = (HttpWebRequest) WebRequest.Create(new Uri(new Uri(srv), callcontext.Request.Url.PathAndQuery));
             
            foreach (var header in callcontext.Request.Headers.AllKeys) {
                var v = callcontext.Request.Headers[header];
                if (header == "Host") {
                  //  req.Host = "127.0.0.1";
                }
                else if (header == "User-Agent") {
                    req.UserAgent = v;
                }
                else if (header == "Accept") {
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
                    if (v == "Keep-Alive") {
                        req.KeepAlive = true;
                    }
                    else if (v == "Close") {
                        continue;
                    }

                }
                else if (header == "Content-Length") {
                   // req.ContentLength = v.ToInt();
                    continue;
                }
                else if (header == "Expect")
                {
                    // req.ContentLength = v.ToInt();
                    continue;
                }
                
                else {
                    req.Headers[header] = callcontext.Request.Headers[header];
                }
            }
            req.Method = callcontext.Request.HttpMethod;
            req.Headers["QHPROXYORIGIN"] = Uri.EscapeDataString(callcontext.Request.Url.ToString());

       
                ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                if (req.Method == "POST") {

                    using (var reqStream = req.GetRequestStream()) {
                        while ((bytesRead = callcontext.Request.InputStream.Read(buffer, 0, buffer.Length)) != 0) {
                            reqStream.Write(buffer, 0, bytesRead);
                        }
                        reqStream.Flush();
                        reqStream.Close();
                    }
                }
                var resp = req.GetResponse();
                foreach (var header in resp.Headers.AllKeys) {
                    var v = resp.Headers[header];
                    if (header == "Transfer-Encoding") {
                        continue;
                    }
                    if (header == "Content-Type") {
                        callcontext.Response.ContentType = v;
                    }
                    callcontext.Response.Headers[header] = resp.Headers[header];
                }
                using (var resstream = resp.GetResponseStream()) {
                    while ((bytesRead = resstream.Read(buffer, 0, buffer.Length)) != 0) {
                        callcontext.Response.OutputStream.Write(buffer, 0, bytesRead);
                        callcontext.Response.OutputStream.Flush();
                    }
                }
                callcontext.Response.Close();
            }
            catch (Exception ex) {
                callcontext.Response.Finish("Error: " + ex.ToString(), status: 500);
            }
            finally {
                ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
            }
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}