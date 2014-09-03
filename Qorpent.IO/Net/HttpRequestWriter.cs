﻿using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Объкт для записи запроса в поток
	/// </summary>
	public class HttpRequestWriter{
		private Stream _stream;

		/// <summary>
		/// Создает врайтер, привязанный к потоку
		/// </summary>
		/// <param name="stream"></param>
		public HttpRequestWriter(Stream stream){
			_stream = stream;
		}
		/// <summary>
		/// Записывает запрос в поток
		/// </summary>
		/// <param name="request"></param>
		public void Write(HttpRequest request){
			using (var bwriter = new BinaryWriter(_stream, Encoding.ASCII, true)){
				bwriter.Write(request.Method.ToCharArray());
				bwriter.Write(' ');
				bwriter.Write(request.Uri.PathAndQuery.ToCharArray());
				bwriter.Write(' ');
				bwriter.Write("HTTP/1.1\r\n".ToCharArray());
				bwriter.Write("Host: ".ToCharArray());
				bwriter.Write(request.Uri.Host.ToCharArray());
				bwriter.Write("\r\n".ToCharArray());
				bool cookiesWasWritten = false;
				foreach (var header in request.Headers.OrderBy(_ => _.Key)){
					bwriter.Write(header.Key.ToCharArray());
					bwriter.Write(": ".ToCharArray());
					bwriter.Write(header.Value.ToCharArray());
					bwriter.Write("\r\n".ToCharArray());
					if (header.Key == "Cookie"){
						cookiesWasWritten = true;
					}
				}
				if (null != request.Cookies && !cookiesWasWritten){
					var cookies = string.Join(", ",
											  request.Cookies.OfType<Cookie>()
													 .Where(_ => HttpUtils.IsCookieMatch(_, request.Uri))
													 .Select(_ => _.ToString()));
					if (!string.IsNullOrWhiteSpace(cookies)){
						bwriter.Write("Cookie: ".ToCharArray());
						bwriter.Write(cookies.ToCharArray());
						bwriter.Write("\r\n".ToCharArray());	
					}
					
				}
				bwriter.Write("\r\n".ToCharArray());
				bwriter.Flush();
			}
		}
	}
}