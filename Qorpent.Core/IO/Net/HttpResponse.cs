﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Упрощенная, ускоренная версия респонза
	/// </summary>
	public class HttpResponse{
		private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
		private Encoding _encoding;
		/// <summary>
		///		Запрос адреса
		/// </summary>
		public HttpRequest Request { get; set; }
		/// <summary>
		///     Статус
		/// </summary>
		public int State { get; set; }
		/// <summary>
		///		Признак перенаправления
		/// </summary>
		public bool IsRedirect {
			get { return State >= 300 && State < 400; }
		}
		/// <summary>
		///		Результирующай адрес документа
		/// </summary>
		public Uri Uri {
			get { return _uri??Request.Uri; }
			set { _uri = value; }
		}
		/// <summary>
		///		Адрес перенаправления
		/// </summary>
		public Uri RedirectUri {
			get {
				if (Headers["Location"] == null) return  null;
				if (Uri == null) return null;
				Uri redirectUri;
				Uri.TryCreate(Uri, Headers["Location"], out redirectUri);
				return redirectUri;
			}
		}
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
        /// 
        /// </summary>
	    public IList<string> RawCookies {
	        get {
	            return _cookies;
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
		public string ContentType {
			get { return Headers.ContainsKey("Content-Type") ? Headers["Content-Type"] : ""; }
		}

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
					    if (Charset.Contains("charset=")) {
					        _encoding = Encoding.GetEncoding(Regex.Match(Charset, "charset=(.+)").Groups[1].Value);
					    }
					    else {
                            _encoding = Encoding.GetEncoding(Charset);    
					    }
						
					}
				}
				return _encoding;
			}
		}

		private string _directdata;
		private Uri _uri;
	    private IList<string> _cookies = new List<string>();

	    /// <summary>
		///     Возвращает строчное значение
		/// </summary>
		public string StringData
		{
			get{
				if (null != _directdata) return _directdata;
				if (null == Data) return "";
				return _directdata = Encoding.GetString(Data);
			}
			set { _directdata = value; }
		}
		/// <summary>
		/// Ошибка респонза
		/// </summary>
		public IOException Error { get; set; }
		/// <summary>
		/// Признак сырых не обработанных данных в контенте
		/// </summary>
		public bool IsRawData { get; set; }
		/// <summary>
		/// Признак завершенного запроса
		/// </summary>
		public bool Success { get; set; }
		/// <summary>
		/// Признак отклика на чанках
		/// </summary>
		public bool Chunked { get; set; }

		/// <summary>
		/// Признак наличия конента
		/// </summary>
		public bool HasContent{
		    get {
		        return ContentLength != 0 || Chunked || (null != Data && 0!=Data.Length) ;
		    }
		}
		/// <summary>
		/// Использован GZip
		/// </summary>
		public bool GZip { get; set; }
		/// <summary>
		/// Использован Deflate
		/// </summary>
		public bool Deflate { get; set; }

		private const string HEADER_TRANSFER_ENCODING = "Transfer-Encoding";
		private const string CHUNKED_MARK = "chunked";
		private const string HEADER_CONTENT_LENGTH = "Content-Length";
		private const string HEADER_CONTENT_TYPE = "Content-Type";
		private const string HEADER_CONTENT_ENCODING = "Content-Encoding";
		private const string HEADER_SETCOOKIE = "Set-Cookie";
		private const string GZIP_MARK = "gzip";
		private const string DEFLATE_MARK = "deflate";

		/// <summary>
		/// Осуществляет процессинг хидера
		/// </summary>
		/// <param name="headerName"></param>
		/// <param name="headerValue"></param>
		public void AddHeader(string headerName, string headerValue){
			if (headerName == HEADER_TRANSFER_ENCODING){
				Chunked = headerValue.Contains(CHUNKED_MARK);
			}else if (headerName == HEADER_CONTENT_LENGTH){
				ContentLength = Convert.ToInt32(headerValue);
			}
			else if (headerName == HEADER_CONTENT_TYPE){
				var csidx = headerValue.IndexOf("charset=");
				if (csidx != -1){
					Charset = headerValue.Substring(csidx + 8);
				}
				
			}else if (headerName == HEADER_CONTENT_ENCODING){
				GZip = headerValue.Contains("gzip");
				Deflate = headerValue.Contains("deflate");
            } else if (headerName == HEADER_SETCOOKIE) {
                RawCookies.Add(headerValue);
            }

			Headers[headerName] = headerValue;
		}

	    
	    /// <summary>
	    /// Строковое представление Headers
	    /// </summary>
	    /// <returns></returns>
	    public string StringHeaders() {
	        var sb = new StringBuilder();
	        foreach (var header in Headers) {
	            sb.Append(header.Key);
	            sb.Append("=");
	            sb.Append(header.Value);
	        }
	        return sb.ToString();
	    }



	}
}