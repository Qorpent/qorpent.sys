﻿using System.Net;
using System.Text;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Базовая реализация конфгурации запроса и самого ресурса
	/// </summary>
	public class ResourceConfig : Scope, IResourceConfig {
		private static IResourceConfig _default = new ResourceConfig();
		/// <summary>
		/// Конфигурация ресурсов и их загрузки по умолчанию
		/// </summary>
		public static IResourceConfig Default {
			get { return _default; }
			protected set { _default = value; }
		}

		/// <summary>
		/// Имя параметра данных для закачки POST
		/// </summary>
		public const string REQUEST_POST_DATA = "request_post_data";
		/// <summary>
		/// Метод веб для запроса
		/// </summary>
		public const string REQUEST_METHOD = "request_method";

		/// <summary>
		/// Кодировка результатов загрузки
		/// </summary>
		public const string RESPONSE_ENCODING = "response_encoding";
	/// <summary>
		/// Тип использования прокси
		/// </summary>
		public const string PROXY_USAGE = "proxy_usage";

		/// <summary>
		/// Адрес (адреса прокси)
		/// </summary>
		public const string PROXY_ADDRESSES = "proxy_addresses";
		
		/// <summary>
		/// Адрес (адреса прокси)
		/// </summary>
		public const string PROXY_EXCLUDES = "proxy_excludes";

		/// <summary>
		/// Адрес (адреса прокси)
		/// </summary>
		public const string ACCEPT_ALL_CERTIFICATES = "accept_all_certificates";

		/// <summary>
		/// Сведения о пользователе (адреса прокси)
		/// </summary>
		public const string CREDENTIALS = "credentials";
		/// <summary>
		/// Куки (адреса прокси)
		/// </summary>
		public const string COOKIES = "cookies";

		/// <summary>
		/// Использование аутентификации QWEB
		/// </summary>
		public const string QWEBAUTH = "qweb_auth";
		/// <summary>
		/// Данные для формы
		/// </summary>
		public const string REQUEST_FORM_STRING = "post_data";

		/// <summary>
		/// Данные для отсылки конечной точке(для вебоподобных ресурсов)
		/// </summary>
		public byte[] RequestPostData {
			get { return Get<byte[]>(REQUEST_POST_DATA); }
			set { Set(REQUEST_POST_DATA, value); }
		}

		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		public string Method {
			get { return Get<string>(REQUEST_METHOD); }
			set { Set(REQUEST_METHOD, value); }
		}

		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		public Encoding ResponseEncoding
		{
			get { return Get<Encoding>(RESPONSE_ENCODING); }
			set { Set(RESPONSE_ENCODING, value); }
		}

		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		public ProxyUsage ProxyUsage
		{
			get { return Get(PROXY_USAGE,ProxyUsage.Default); }
			set { Set(PROXY_USAGE, value); }
		}
		/// <summary>
		/// Адреса прокси в формате 
		/// TYPE=scheme://[USER:PASSWORD@]url, разделенные пробелами, вместо TYPE может использоваться 
		/// звездочка, а имя/пароль могут опускаться (будут использоваться DefaultCredentials)
		/// например 
		/// *=http://myproxy.com
		/// </summary>
		public string ProxyAddress {
			get { return Get(PROXY_ADDRESSES, ""); }
			set { Set(PROXY_ADDRESSES, value); }
		}
		/// <summary>
		/// Space-delimited list of proxy excludes
		/// </summary>
		public string ProxyExcludes
		{
			get { return Get(PROXY_EXCLUDES, ""); }
			set { Set(PROXY_EXCLUDES, value); }
		}
		/// <summary>
		/// True, чтобы принимать самоподписные или недоверенные сертификаты
		/// </summary>
		public bool AcceptAllCeritficates {
			get { return Get(ACCEPT_ALL_CERTIFICATES,false); }
			set { Set(ACCEPT_ALL_CERTIFICATES, value); }
		}

		/// <summary>
		/// Сведения о пользователе
		/// </summary>
		public ICredentials Credentials {
			get { return Get<ICredentials>(CREDENTIALS, null); }
			set { Set(CREDENTIALS, value); }
		}
		/// <summary>
		/// Контейнер кук
		/// </summary>
		public CookieContainer Cookies {
			get { return Get<CookieContainer>(COOKIES, null); }
			set { Set(COOKIES, value); }
		}

		/// <summary>
		/// Контейнер кук
		/// </summary>
		public bool UseQwebAuthentication
		{
			get { return Get(QWEBAUTH, false); }
			set { Set(QWEBAUTH, value); }
		}
		/// <summary>
		/// Данные формы для POST
		/// </summary>
		public string RequestFormString {
			get { return Get(REQUEST_FORM_STRING, ""); }
			set { Set(REQUEST_FORM_STRING, value); }
		}
	}
}