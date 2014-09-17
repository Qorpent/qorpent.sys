using System;
using System.Collections.Concurrent;
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
	public class HttpClient:IContentSource{
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public byte[] GetData(string url){
			var resp = Call(url);
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

		readonly HttpResponseReader _reader = new HttpResponseReader();
		/// <summary>
		/// Выполнить запрос
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public HttpResponse Call(HttpRequest request){
			try{
				var secure = request.Uri.Scheme.StartsWith("https");
				request.Cookies = request.Cookies ?? Cookies;
				var endpoint = GetEndpoint(request.Uri);
				using (var socket = new Socket(SocketType.Stream, ProtocolType.Tcp)){
					socket.Connect(endpoint);
					using (var ns = new NetworkStream(socket)){
						using (var rs = secure ? (Stream) new SslStream(ns, false, UserCertificateValidationCallback) : ns){
							if (secure){
								((SslStream) rs).AuthenticateAsClient(request.Uri.Host);
							}
							var writer = new HttpRequestWriter(rs);
							writer.Write(request);
							var response = _reader.Read(rs);
							if (response.IsRedirect){
								request.Uri = response.RedirectUri;
								return Call(request);
							}
							response.Cookies = response.Cookies ?? Cookies;
							if (response.Headers.ContainsKey("Cookie")){
								foreach (var cookie in HttpUtils.ParseCookies(response.Headers["Cookie"])){
									response.Cookies.Add(cookie);
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
				var ip = Dns.GetHostAddresses(host)[0];
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