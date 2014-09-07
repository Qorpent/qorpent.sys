using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Обертка для работы с HTTP
	/// </summary>
	public class HttpClient{
		/// <summary>
		///  Коллекция куки
		/// </summary>
		public CookieCollection Cookies { get; set; }
		/// <summary>
		/// Вызов запроса по URL
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public HttpResponse Call(string url){
			return Call(new HttpRequest{Uri = new Uri(url)});
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public string GetString(string url){
			var resp = Call(url);
			if (resp.Success){
				return resp.StringData;
			}
			throw new IOException("error in response",resp.Error);
		}

		readonly HttpResponseReader reader = new HttpResponseReader();
		/// <summary>
		/// Выполнить запрос
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpResponse Call(HttpRequest request){
			bool secure = request.Uri.Scheme.StartsWith("https");
			request.Cookies = request.Cookies ?? Cookies;
			var endpoint = GetEndpoint(request.Uri);
			using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)){
				socket.Connect(endpoint);
				using (var ns = new NetworkStream(socket)){
					using (var rs = secure?(Stream)new SslStream(ns,false,UserCertificateValidationCallback) : ns){
						if (secure){
							((SslStream)rs).AuthenticateAsClient(request.Uri.Host);
						}
						var writer = new HttpRequestWriter(rs);
						writer.Write(request);
						var response = reader.Read(rs);
						if (response.IsRedirect) {
							request.Uri = response.RedirectUri;
							return Call(request);
						}
						response.Cookies = response.Cookies ?? Cookies;
						if (response.Headers.ContainsKey("Cookie")){
							foreach (var cookie in HttpUtils.ParseCookies(response.Headers["Cookie"])){
								response.Cookies.Add(cookie);
							}
						}
						return response;
					}
				}
			}
			
		}

		private bool UserCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){
			return true;
		}

		private IDictionary<string,IPEndPoint> _dnsCache  = new Dictionary<string, IPEndPoint>(); 
		private IPEndPoint GetEndpoint(Uri uri){
			var url = uri.ToString();
			if (!_dnsCache.ContainsKey(url)){
				var host = uri.Host;
				var ip = Dns.GetHostAddresses(host)[0];
				var port = uri.Port;
				if (0 == port){
					port = uri.Scheme.StartsWith("https") ? 443 : 80;
				}
				var ep = new IPEndPoint(ip, port);
				_dnsCache[url] = ep;
			}
			return _dnsCache[url];
		}
	}
}