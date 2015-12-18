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

        public static HttpRequest Head(string url) {
            return new HttpRequest {Uri = new Uri(url), Method = HttpMethod.Head};
        }

        public static HttpRequest Get(string url) {
            return new HttpRequest { Uri = new Uri(url), Method = HttpMethod.Get };
        }
        public static HttpRequest Delete(string url)
        {
            return new HttpRequest { Uri = new Uri(url), Method = HttpMethod.Delete };
        }
        public static HttpRequest Post(string url, string data)
        {
            return new HttpRequest { Uri = new Uri(url), Method = HttpMethod.Post, PostData = data};
        }

        /// <summary>
        /// Строковое представление POST
        /// </summary>
        public string PostData { get; set; }
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
		///		Принимаемый язык
		/// </summary>
		public string AcceptLanguage {
			get {
				if (!Headers.ContainsKey("Accept-Language")) {
					Headers["Accept-Language"] = "en-US";
				}
				return Headers["Accept-Language"];
			}
			set { Headers["Accept-Language"] = value; }
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

	    /// <summary>
	    /// Запретить редирект 
	    /// </summary>
	    public bool PreventRedirect { get; set; }
	}
}
