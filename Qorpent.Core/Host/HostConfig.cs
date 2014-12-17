using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
	/// <summary>
	///     Конфигурация хоста
	/// </summary>
	[Serialize]
	public class HostConfig{
		private readonly IList<HostBinding> _bindings;
		private string _configFolder;
		private string _dllFolder;
		private string _logFolder;
		private ConfigBase _parameters;
		private string _rootFolder;
		private int _threadCount;
		private string _tmpFolder;
		private bool _useApplicationName;
	    private IUserLog _log;

	    /// <summary>
	    ///     Формирует конфиг из XML
	    /// </summary>
	    /// <param name="xml"></param>
	    /// <param name="context"></param>
	    public HostConfig(XElement xml,IBSharpContext context = null) : this(){
			if (null != xml){
				LoadXmlConfig(xml,context);
			}
		}

		/// <summary>
		///     Формирует конфиг по умолчанию
		/// </summary>
		public HostConfig(){
			RootFolder = EnvironmentInfo.GetResolvedRootDirectory();
			_bindings = new List<HostBinding>();
			IncludeConfigMasks = new List<string>();
			ExcludeConfigMasks = new List<string>();
			AutoconfigureAssemblies = new List<string>();
			LogLevel = LogLevel.Info;
			ContentFolders = new List<string>();
			ExtendedContentFolders = new List<string>();
			Cached = new List<string>();
			AuthCookieName = "QHAUTH";
			AuthCookieDomain = "";
			AccessAllowOrigin = "";
			AccessAllowHeaders = "*";
			AccessAllowMethods = "GET, POST, OPTIONS";
			AccessAllowCredentials = true;
			StaticContentMap = new Dictionary<string, string>();
            Proxize = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
		}
		/// <summary>
		/// Разрешение Cookie при работе с Cross-Site
		/// </summary>
		public bool AccessAllowCredentials { get; set; }

		/// <summary>
		/// Методы, разрешнные для Cross-Site-Scripting
		/// </summary>
		public string AccessAllowMethods { get; set; }

		/// <summary>
		/// Настройка доступа для Cross-Site-Scripting по хидеру 
		/// </summary>
		public string AccessAllowHeaders { get; set; }

		/// <summary>
		/// Настройка доступа для Cross-Site-Scripting по происхождению запроса
		/// </summary>
		public string AccessAllowOrigin { get; set; }

		/// <summary>
		/// </summary>
		public string AuthCookieDomain { get; set; }

		/// <summary>
		/// </summary>
		public bool UseApplicationName{
			get { return _useApplicationName || Bindings.Any(_ => _.AppName != "/"); }
			set { _useApplicationName = value; }
		}

		/// <summary>
		///     Список путей для поиска статического контента
		/// </summary>
		public IList<string> ContentFolders { get; private set; }

		/// <summary>
		///     Список сборок для автоматической конфигурации
		/// </summary>
		public IList<string> AutoconfigureAssemblies { get; private set; }


		/// <summary>
		/// </summary>
		[Serialize]
		public IList<string> IncludeConfigMasks { get; private set; }

		/// <summary>
		/// </summary>
		[Serialize]
		public IList<string> ExcludeConfigMasks { get; private set; }

		/// <summary>
		///     Корневая папка
		/// </summary>
		[Serialize]
		public string RootFolder{
			get { return _rootFolder; }
		    set {
		        if (value.Contains("@")) {
		            _rootFolder = EnvironmentInfo.ResolvePath(value);
		        }
		        else {
		            _rootFolder = Path.GetFullPath(value);
		        }
		    }
		}

		/// <summary>
		///     Папка с конфигами
		/// </summary>
		[Serialize]
		public string ConfigFolder{
			get { return _configFolder = NormalizeFolder(_configFolder, HostUtils.DefaultConfigFolder); }
			set { _configFolder = value; }
		}

		/// <summary>
		///     Папка с DLL
		/// </summary>
		[Serialize]
		public string DllFolder{
			get { return _dllFolder = NormalizeFolder(_dllFolder, HostUtils.DefaultDllFolder); }
			set { _dllFolder = value; }
		}

		/// <summary>
		///     Директория для временных файлов
		/// </summary>
		[Serialize]
		public string TmpFolder{
			get { return _tmpFolder = NormalizeFolder(_tmpFolder, HostUtils.DefaultTmpFolder); }
			set { _tmpFolder = value; }
		}

		/// <summary>
		///     Папка для файлов журналов
		/// </summary>
		[Serialize]
		public string LogFolder{
			get { return _logFolder = NormalizeFolder(_logFolder, HostUtils.DefaultLogFolder); }
			set { _logFolder = value; }
		}

		/// <summary>
		///     Коллекция привязок
		/// </summary>
		public IList<HostBinding> Bindings{
			get { return _bindings; }
		}

		/// <summary>
		///     Дополнительные параметры
		/// </summary>
		[Serialize]
		public ConfigBase Parameters{
			get { return _parameters ?? (_parameters = new ConfigBase()); }
			set { _parameters = value; }
		}

		/// <summary>
		///     Количество тредов
		/// </summary>
		[Serialize]
		public int ThreadCount{
			get{
				if (0 >= _threadCount){
					_threadCount = HostUtils.DefaultThreadCount;
				}
				return _threadCount;
			}
			set { _threadCount = value; }
		}

		/// <summary>
		///     режим приложения
		/// </summary>
		public HostApplicationMode ApplicationMode { get; set; }

		/// <summary>
		/// </summary>
		public LogLevel LogLevel { get; set; }

		/// <summary>
		///     Список путей для поиска статического контента
		/// </summary>
		public IList<string> ExtendedContentFolders { get; private set; }

		/// <summary>
		/// </summary>
		public string DefaultPage { get; set; }

		/// <summary>
		/// </summary>
		public bool ForceNoCache { get; set; }

		/// <summary>
		/// </summary>
		public IList<string> Cached { get; private set; }

		/// <summary>
		///     Имя куки аутентификации
		/// </summary>
		public string AuthCookieName { get; set; }

		/// <summary>
		/// </summary>
		public string EncryptBasis { get; set; }
		/// <summary>
		/// Мапинг юрлов в диретории для Static Handler
		/// </summary>
		public IDictionary<string, string> StaticContentMap { get; private set; }
		/// <summary>
		///		Набор строк подключения
		/// </summary>
		public IDictionary<string, string> ConnectionStrings { get; private set; } 
	    /// <summary>
	    /// Журнал
	    /// </summary>
	    public IUserLog Log
	    {
	        get { return _log ?? (_log = StubUserLog.Default); }
	        set { _log = value; }
	    }


	    /// <summary>
	    ///     Загружает конфигурационный файл из XML
	    /// </summary>
	    /// <param name="xml"></param>
	    /// <param name="context"></param>
	    public void LoadXmlConfig(XElement xml,IBSharpContext context = null) {
	        this.Definition = xml;
            RootFolder = xml.ResolveValue("root", RootFolder);
	        RootFolder = xml.ResolveValue(HostUtils.RootFolderXmlName, RootFolder);	      
	        ConfigFolder = xml.ResolveValue(HostUtils.ConfigFolderXmlName, ConfigFolder);
	        DllFolder = xml.ResolveValue(HostUtils.DllFolderXmlName, DllFolder);
	        LogFolder = xml.ResolveValue(HostUtils.LogFolderXmlName, LogFolder);
	        TmpFolder = xml.ResolveValue(HostUtils.TmpFolderXmlName, TmpFolder);
	        LogLevel = xml.ResolveValue(HostUtils.LogLevelXmlName, "Info").To<LogLevel>();
			UseApplicationName = xml.ResolveValue(HostUtils.UseApplicationName, "false").To<bool>();
	        AuthCookieName = xml.ResolveValue(HostUtils.AuthCookieName, AuthCookieName);
	        AuthCookieDomain = xml.ResolveValue(HostUtils.AuthCookieDomain, AuthCookieDomain);
	        EncryptBasis = xml.ResolveValue(HostUtils.EncryptBasis, Guid.NewGuid().ToString());
	        DefaultPage = xml.ResolveValue(HostUtils.DefaultPage, "default.html");
	        foreach (XElement bind in xml.Elements(HostUtils.BindingXmlName)){
				var hostbind = new HostBinding();
				hostbind.Port = bind.Attr(HostUtils.PortXmlName).ToInt();
				hostbind.Interface = bind.Attr(HostUtils.InterfaceXmlName);
				string schema = bind.Attr(HostUtils.SchemaXmlName);
				if (!string.IsNullOrWhiteSpace(schema)){
					if (schema == HostUtils.HttpsXmlValue){
						hostbind.Schema = HostSchema.Https;
					}
				}
				if (hostbind.Port == 0){
					hostbind.Port = HostUtils.DefaultBindingPort;
				}
				if (string.IsNullOrWhiteSpace(hostbind.Interface)){
					hostbind.Interface = HostUtils.DefaultBindingInterface;
				}
				Bindings.Add(hostbind);
				
			}

			foreach (XElement e in xml.Elements(HostUtils.ContentFolder))
			{
				
				ContentFolders.Add(e.Attr("code"));
			}
            ReadModules(xml);
		    foreach (var e in xml.Elements("connection")) {
			    var name = e.Attr("code");
			    var cstr = e.Attr("name");
				if (string.IsNullOrWhiteSpace(name)) continue;
				if (string.IsNullOrWhiteSpace(cstr)) continue;
			    ConnectionStrings[name] = cstr;
		    }
	        foreach (var e in xml.Elements("static")) {
	            var name = e.Attr("code");
	            var folder = EnvironmentInfo.ResolvePath(e.Attr("name"));
	            if (!name.StartsWith("/")) {
	                name = "/" + name;
	            }
	            if (!name.EndsWith("/")) {
	                name += "/";
	            }
	            this.StaticContentMap[name] = folder;
	        }
			foreach (XElement e in xml.Elements(HostUtils.ExContentFolder))
			{

				ExtendedContentFolders.Add(e.Attr("code"));
			}
			foreach (XElement e in xml.Elements(HostUtils.IncludeConfigXmlName)){
				IncludeConfigMasks.Add(e.Describe().GetEfficienValue());
			}
			foreach (XElement e in xml.Elements(HostUtils.ExcludeConfigXmlName)){
				ExcludeConfigMasks.Add(e.Describe().GetEfficienValue());
			}
			foreach (XElement e in xml.Elements("cache")){
				Cached.Add(e.Value);
			}
            foreach (XElement e in xml.Elements("proxize")) {
                var key = e.Attr("code");
                var url = e.Attr("url");
                if (string.IsNullOrWhiteSpace(url)) {
                    if (!string.IsNullOrWhiteSpace(e.Attr("appid"))) {
                        url += "appid=" + e.Attr("appid")+";";
                    }
                    if (!string.IsNullOrWhiteSpace(e.Attr("secure")))
                    {
                        url += "secure=" + e.Attr("secure")+";";
                    }
                    if (!string.IsNullOrWhiteSpace(e.Attr("server")))
                    {
                        url += "server=" + e.Attr("server") + ";";
                    }
                }
                Proxize[key] = url;
            }
            foreach (XElement e in xml.Elements("lib"))
            {
                AutoconfigureAssemblies.Add(e.AttrOrValue("code"));
            }
			ForceNoCache = xml.Attr("forcenocache").ToBool();

	        var appid = xml.ResolveValue("appid", "0").ToInt();
	        if (appid != 0) {
	            AddQorpentBinding(appid);
	        }
                
            this.AccessAllowOrigin = xml.ResolveValue("origin", "");

	        foreach (var e in xml.Elements("require")) {
	            var appname = e.Attr("code")+e.Attr("suffix");
    
	            var proxize = e.GetSmartValue("proxize").ToBool() || e.Attr("name")=="proxize";
	            if (proxize) {
                    if (null == context)
                    {
                        this.Log.Error("context not provi " + appname);
                    }
	                var cls = context[appname];
	                if (null == cls) {
	                    this.Log.Error("cannot find application for proxize " + appname);
	                }
	                else {
	                    var sappid = cls.Compiled.ResolveValue("appid");
	                    var services = cls.Compiled.Elements("service");
	                    foreach (var srv in services) {
	                        var root = srv.Attr("code");
	                        this.Proxize[root] = "appid=" + sappid+";";
	                    }

	                }
	            }
	        }
	    }
        /// <summary>
        /// Обратная ссылка на XML- определение
        /// </summary>
	    public XElement Definition { get; set; }
        /// <summary>
        /// Мапинг прокси адресов для их обработки на другом хосте
        /// </summary>
	    public IDictionary<string,string> Proxize { get; private set; }

	    private void ReadModules(XElement xml) {
	        foreach (XElement e in xml.Elements("module")) {
	            var fname = e.Attr("code");

	            if (Regex.IsMatch(fname, @"^[\w\d\-]+$")) {
	                //name only
	                bool found = false;
	                foreach (var d in Directory.GetDirectories(EnvironmentInfo.GetRepositoryRoot())) {
	                    var test = Path.Combine(d, fname + ".webmodule", "dist");
	                    if (Directory.Exists(test)) {
	                        fname = test;
	                        found = true;
	                        break;
	                    }
	                }
	                if (!found) {
	                    throw new Exception("module " + fname + " not found");
	                }
	            }
	            else {
	                if (!fname.StartsWith("~")) {
	                    fname = "@repos@/" + fname.Substring(1) + ".webmodule/dist";
	                }
	                else {
	                    fname = fname.Substring(1);
	                }
	            }

	            if (fname.Contains("@")) {
	                fname = EnvironmentInfo.ResolvePath(fname);
	            }


	            ContentFolders.Add(fname);
	        }
	    }

	    /// <summary>
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public bool IsConfigFileMatch(string filename){
			if (0 != IncludeConfigMasks.Count){
				if (IncludeConfigMasks.All(_ => !Regex.IsMatch(filename, _))) return false;
			}
			if (0 != ExcludeConfigMasks.Count){
				if (ExcludeConfigMasks.Any(_ => Regex.IsMatch(filename, _))) return false;
			}
			return true;
		}

	    private string NormalizeFolder(string current, string def){
			if (!string.IsNullOrWhiteSpace(current) && Path.IsPathRooted(current)) return current;
			return Path.Combine(RootFolder, def);
		}

		/// <summary>
		/// </summary>
		public void AddDefaultBinding(){
			_bindings.Add(
				new HostBinding{
					Interface = HostUtils.DefaultBindingInterface,
					Port = HostUtils.DefaultBindingPort,
					Schema = HostSchema.Http
				}
				);
			//_bindings.Add(

			//			new HostBinding { Interface = HostUtils.DefaultBindingInterface, Port = HostUtils.DefaultBindingPort+1, Schema = HostSchema.Https }
			//	);
		}

        /// <summary>
        /// Добавляет биндинги по стандартным смещениям Qorpent
        /// </summary>
        /// <param name="appId"></param>
	    public void AddQorpentBinding(int appId) {
            var baseport = HostUtils.GetBasePort(appId);
            Bindings.Add(new HostBinding { Port = baseport });
            Bindings.Add(new HostBinding { Port = baseport+1, Schema = HostSchema.Https });
            Bindings.Add(new HostBinding { Interface = "127.0.0.1", Port = baseport + 5 });
            Bindings.Add(new HostBinding { Interface = "127.0.0.1", Port = baseport + 6, Schema = HostSchema.Https });
	    }
	}
}