using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Qorpent.Experiments;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Обертка для работы с HTTP
	/// </summary>
	public class HttpClient:IContentSource{
	    /// <summary>
	    /// Вызов запроса по URL
	    /// </summary>
	    /// <param name="url"></param>
	    /// <param name="post"></param>
	    /// <returns></returns>
	    public HttpResponse Call( string url,string post = null, Action<HttpRequest> setup = null) {
	        var uri = new Uri(url, UriKind.RelativeOrAbsolute);
	        if (!uri.IsAbsoluteUri) {
	            if (null != BaseUri) {
	                uri = new Uri(BaseUri, uri);
	            }
	            else {
	                throw new Exception("relative url given without base set");
	            }
	        }
	        var req = new HttpRequest {Uri = uri};
	        
	        if (null != post) {
	            req.Method = "POST";
	            req.PostData = post;
	        
	        }
            if (null != setup) {
                setup(req);
            }
			return Call(req);
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="url"></param>
	    /// <param name="post"></param>
	    /// <returns></returns>
	    string IContentSource.GetString(string url, string post = null) {
	        return GetString(url, post, null);
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="url"></param>
	    /// <param name="post"></param>
	    /// <returns></returns>
        public string GetString(string url, string post = null, Action<HttpRequest> setup = null)
        {
			var resp = Call(url,post,setup);
			if (resp.Success){
				return resp.StringData;
			}
			throw new IOException("error in response",resp.Error);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        public string GetString(string url, object post , Action<HttpRequest> setup = null) {
            if (post is string) return GetString(url, (string) post, setup);
            return GetString(url, post.stringify(), setup);
        }

	    public object GetObject(string url, object post, Action<HttpRequest> setup = null) {
	        return GetString(url, post, setup).jsonify();
	    }

        public object GetObject(string url, string post = null, Action<HttpRequest> setup = null)
        {
            return GetString(url, post, setup).jsonify();
        }


	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="url"></param>
	    /// <param name="post"></param>
	    /// <returns></returns>
	    byte[] IContentSource.GetData(string url, string post = null) {
	        return GetData(url, post, null);
	    }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="url"></param>
	    /// <param name="post"></param>
	    /// <returns></returns>
        public byte[] GetData(string url, string post = null, Action<HttpRequest> setup = null)
        {
			var resp = Call(url,post,setup);
			if (resp.Success)
			{
				return resp.Data;
			}
			throw new IOException("error in response", resp.Error);
		}
		/// <summary>
		///		Разрешает результирующий адрес документа
		/// </summary>
		/// <param name="uri">Исходный адрес документа</param>
		/// <returns>Результирующий адрес документа</returns>
		public string ResolveUri(string uri) {
			return uri;
		}

        public CookieCollection Cookies { get; set; }
	    public Uri BaseUri { get; set; }
	    public int ConnectionTimeout { get; set; }


	    readonly HttpResponseReader _reader = new HttpResponseReader();
		/// <summary>
		/// Выполнить запрос
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpResponse Call(HttpRequest request){
			try{
				var secure = request.Uri.Scheme.StartsWith("https");
				request.Cookies = request.Cookies ?? this.Cookies ?? new CookieCollection();
				var endpoint = GetEndpoint(request.Uri);
				using (var socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)) {
				    if (ConnectionTimeout <= 0) {
				        socket.Connect(endpoint);
				    }
				    else {
				        var async = socket.BeginConnect(endpoint,_ => { }, null);
				        var finished = async.AsyncWaitHandle.WaitOne(ConnectionTimeout);
				        if (!finished) {
				            throw new IOException("timeouted");
				        }
				    }
					using (var ns = new NetworkStream(socket)){
						using (var rs = secure ? (Stream) new SslStream(ns, false, UserCertificateValidationCallback) : ns){
							if (secure){
								((SslStream) rs).AuthenticateAsClient(request.Uri.Host);
							}
							var writer = new HttpRequestWriter(rs);
							writer.Write(request);
							var response = _reader.Read(rs);
							response.Request = request;
							if (response.IsRedirect && !request.PreventRedirect){
								request.Uri = response.RedirectUri;
								return Call(request);
							}
							response.Cookies = request.Cookies;
							if (null!=response.Cookies && response.RawCookies.Count!=0){
							    foreach (var cookie in response.RawCookies) {
							        var realcookie = HttpUtils.ParseCookies(cookie).First();
                                    response.Cookies.Add(realcookie);
							    }
							}
							response.Request = request;
							return response;
						}
					}
				}
			}
			catch (Exception ex){
				return new HttpResponse{Error = new IOException("some exceptions during http call", ex)};
			}

		}

		private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){
			return true;
		}

		private readonly ConcurrentDictionary<string, IPEndPoint> _dnsCache = new ConcurrentDictionary<string, IPEndPoint>(); 
		private IPEndPoint GetEndpoint(Uri uri){
           
			return _dnsCache.GetOrAdd(uri.ToString(), _ =>{
				var host = uri.Host;
			    var ip = Dns.GetHostAddresses(host).First(addr => addr.AddressFamily == AddressFamily.InterNetwork);
                
				var port = uri.Port;
				if (0 == port)
				{
					port = uri.Scheme.StartsWith("https") ? 443 : 80;
				}
				var ep = new IPEndPoint(ip, port);
				return ep;
			});	
		}
	}
}