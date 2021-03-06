﻿using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
	/// <summary>
	///     Константы настройки хоста
	/// </summary>
	public static class HostUtils{
		/// <summary>
		///     Папка с конфигурацией по умолчанию
		/// </summary>
		public const string DefaultConfigFolder = ".config";

		/// <summary>
		///     Папка с исполнимым кодом по умолчанию
		/// </summary>
		public const string DefaultDllFolder = "bin";

		/// <summary>
		///     Папка для временных файлов по умолчанию
		/// </summary>
		public const string DefaultTmpFolder = ".tmp";

		/// <summary>
		///     Папка для журналов по умолчанию
		/// </summary>
		public const string DefaultLogFolder = ".log";

		/// <summary>
		///     Интерфейс для привязки по умолчанию
		/// </summary>
		public const string DefaultBindingInterface = "*";

		/// <summary>
		///     Порт для привязки по умолчанию
		/// </summary>
		public const int DefaultBindingPort = 8091;

		/// <summary>
		///     Имя параметра корневой папки в схеме XML
		/// </summary>
		public const string RootFolderXmlName = "RootFolder";

		/// <summary>
		///     Имя параметра папки конфига в схеме XML
		/// </summary>
		public const string ConfigFolderXmlName = "ConfigFolder";

		/// <summary>
		///     Имя параметра папки DLL в схеме XML
		/// </summary>
		public const string DllFolderXmlName = "DllFolder";

		/// <summary>
		///     Имя параметра папки временных файлов в схеме XML
		/// </summary>
		public const string TmpFolderXmlName = "TmpFolder";

		/// <summary>
		///     Имя параметра папки журналов в схеме XML
		/// </summary>
		public const string LogFolderXmlName = "LogFolder";

		/// <summary>
		///     Имя для элемента биндинга в схеме XML
		/// </summary>
		public const string BindingXmlName = "Binding";

		/// <summary>
		/// </summary>
		public const string IncludeConfigXmlName = "IncludeConfig";

		/// <summary>
		/// </summary>
		public const string ExcludeConfigXmlName = "ExcludeConfig";

		/// <summary>
		///     Имя для атрибута порта биндинга в схеме XML
		/// </summary>
		public const string PortXmlName = "port";

		/// <summary>
		///     Имя для атрибута интерфейса биндинга в схеме XML
		/// </summary>
		public const string InterfaceXmlName = "interface";

		/// <summary>
		///     Имя для атрибута схемы биндинга в схеме XML
		/// </summary>
		public const string SchemaXmlName = "schema";

		/// <summary>
		///     Значение для атрибута схемы HTTPS в схеме XML
		/// </summary>
		public const string HttpsXmlValue = "https";

		/// <summary>
		///     Путь к сертификату SSL в схеме HTTPS
		/// </summary>
		public const string CertifityPathXmlName = "certifity";

		/// <summary>
		///     Количество потоков прослушивания запросов Web по умолчанию
		/// </summary>
		public const int DefaultThreadCount = 3;

		/// <summary>
		/// </summary>
		public const string LogLevelXmlName = "LogLevel";

		/// <summary>
		///     Использовать имя приложения
		/// </summary>
		public const string UseApplicationName = "UseAppName";

		/// <summary>
		///     Имя куки аутентификации
		/// </summary>
		public const string AuthCookieName = "AuthCookie";

		/// <summary>
		///     Домен куки аутентификации
		/// </summary>
		public const string AuthCookieDomain = "AuthDomain";

		/// <summary>
		///     Базис ключа аутентификации
		/// </summary>
		public const string EncryptBasis = "EncryptBasis";

		/// <summary>
		///   Папки с контентом
		/// </summary>
		public const string ContentFolder = "ContentFolder";

		/// <summary>
		///   Папки с контентом
		/// </summary>
		public const string ExContentFolder = "ExContentFolder";

		/// <summary>
		///   Страница по умолчанию
		/// </summary>
		public const string DefaultPage = "DefaultPage";

        /// <summary>
        /// Базовый порт Qorpent
        /// </summary>
	    public const int DefaultQorpentStartPort = 14000;
        /// <summary>
        /// Смещение номера приложения по портам
        /// </summary>
	    public const int DefaultQorpentApplicationPortOffset = 10;

	    /// <summary>
	    /// Рассчитать базовый порт для приложения
	    /// </summary>
	    /// <param name="appId"></param>
	    /// <returns></returns>
	    public static int GetBasePort(int appId) {
	        var baseport = DefaultQorpentStartPort +
	                       appId*DefaultQorpentApplicationPortOffset;
	        return baseport;
	    }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
	    public static string ParseUrl(string src) {
	        if (!src.Contains("://")) {
	            return ParseConnecitonString(src);
	        }
            return src;
        }

	    public static IRequestHandler GetHandler(this IHostServer server, string url) {
	        Uri uri;
	        if (!url.StartsWith("http")) {
	            uri = new Uri(new Uri("http://localhost"), new Uri(url, UriKind.Relative));
	        }
	        else {
	            uri=new Uri(url);
	        }
	        return server.Factory.GetHandler(server, uri, null);

	    }

	    public static string Call(this IHostServer host, string command) {
            var h = host.GetHandler(command);
            var ms = new MemoryStream();
            var rs = new HttpResponseDescriptor { Stream = ms, NoCloseStream = true };
            var rq = new HttpRequestDescriptor { Uri = new Uri("http://localhost" + command) };

	        var ctx = new WebContext{Request = rq,Response=rs};
            h.Run(host, ctx, null, new CancellationToken());
            var len = ms.Position;
            ms.Position = 0;
            var result = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)len);
            return result;
	    }   

	    private static string ParseConnecitonString(string src) {
	        var parts = src.ReadAsDictionary();
	        if (!parts.ContainsKey("appid")) {
	            throw new Exception("appid is required connection field");
	        }
	        bool secure = parts.ContainsKey("secure") && parts["secure"].ToBool();
	        string server;
	        if (parts.ContainsKey("server")) {
	            var srv = parts["server"];
	            if (srv == ".") {
	                server = "127.0.0.1";
	            }
	            else {
	                server = srv;
	            }
	        }
	        else {
	            server = "127.0.0.1";
	        }
	        var port = GetBasePort(parts["appid"].ToInt());
	        if (secure) port++;
           
	        return string.Format("http{0}://{1}:{2}",secure?"s":"",server,port);
	    }
	}
}