using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Упрощенная, ускоренная версия респонза
	/// </summary>
	public class HttpResponse2{
		private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
		private Encoding _encoding;

		/// <summary>
		///     Статус
		/// </summary>
		public int State { get; set; }

		/// <summary>
		///     Название статуса
		/// </summary>
		public string StateName { get; set; }

		/// <summary>
		///     Заголовки
		/// </summary>
		public IDictionary<string, string> Headers
		{
			get
			{
				return _headers;
			}
		}

		/// <summary>
		///     Коллекция куки
		/// </summary>
		public CookieCollection Cookies { get; set; }

		/// <summary>
		///     Длина контента в байтах
		/// </summary>
		public int ContentLength { get; set; }

		/// <summary>
		///     Исходный content-Type
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		///     Исходное определение кодировки
		/// </summary>
		public string Charset { get; set; }



		/// <summary>
		///     Считанные данные
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		///     Кодировка
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				if (null == _encoding)
				{
					if (string.IsNullOrWhiteSpace(Charset))
					{
						_encoding = Encoding.UTF8;
					}
					else
					{
						_encoding = Encoding.GetEncoding(Charset);
					}
				}
				return _encoding;
			}
		}

		/// <summary>
		///     Возвращает строчное значение
		/// </summary>
		public string StringData
		{
			get
			{
				if (null == Data) return "";
				return Encoding.GetString(Data);
			}
		}
		/// <summary>
		/// Осуществляет процессинг хидера
		/// </summary>
		/// <param name="headerName"></param>
		/// <param name="headerValue"></param>
		public void AddHeader(string headerName, string headerValue){
			Headers[headerName] = headerValue;
		}
	}
}