using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO.Net
{
	/// <summary>
	/// Объект запроса HTTP
	/// </summary>
	public class HttpRequest
	{
		/// <summary>
		/// Конструктор запроса по умолчанию
		/// </summary>
		public HttpRequest(){
			Headers = new Dictionary<string, string>();
			AcceptEncoding = "gzip, deflate";
			Method = "GET";
			UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
		}
		/// <summary>
		/// Коллекция куки
		/// </summary>
		public CookieCollection Cookies { get; set; }
		/// <summary>
		/// Заголовки
		/// </summary>
		public IDictionary<string, string> Headers { get; private set; } 
		/// <summary>
		/// Uri запроса
		/// </summary>
		public Uri Uri { get; set; }
		/// <summary>
		/// Принимаемые типы кодирования
		/// </summary>
		public string AcceptEncoding{
			get{
				if (!Headers.ContainsKey("Accept-Encoding")){
					Headers["Accept-Encoding"] = "gzip, deflate";
				}
				return Headers["Accept-Encoding"];
			}
			set{
				Headers["Accept-Encoding"] = value;
			}
		}
		/// <summary>
		/// Принимаемые типы кодирования
		/// </summary>
		public string UserAgent
		{
			get
			{
				if (!Headers.ContainsKey("User-Agent"))
				{
					Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:31.0) Gecko/20100101 Firefox/31.0";
				}
				return Headers["User-Agent"];
			}
			set
			{
				Headers["User-Agent"] = value;
			}
		}
		/// <summary>
		/// Метод HTTP
		/// </summary>
		public string Method { get; set; }
	}
}
