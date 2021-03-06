﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
    /// <summary>
	///     Конфигурация хоста
	/// </summary>
	[Serialize]
    public partial class HostConfig{
		private readonly IList<HostBinding> _bindings;
		private string _configFolder;
		private string _dllFolder;
		private string _logFolder;
		private string _machineName;
		private Scope _parameters;
		private string _rootFolder;
		private int _threadCount;
		private string _tmpFolder;
		private bool _useApplicationName;
	    private IUserLog _log;
	    private bool _wasInitialized;

	    /// <summary>
	    ///     Формирует конфиг из XML
	    /// </summary>
	    /// <param name="xml"></param>
	    /// <param name="context"></param>
	    /// <param name="machineName"></param>
	    /// <param name="log"></param>
	    public HostConfig(XElement xml,IBSharpContext context = null, string machineName = null, IUserLog log = null) : this() {

		    if (!string.IsNullOrWhiteSpace(machineName)) MachineName = machineName;
		    if (null != log) Log = log;
			if (null != xml){
				LoadXmlConfig(xml,context);
			}
            
	    }


	    public void Initialize() {
	        if (!_wasInitialized) {
                CreateLogger();
	            _wasInitialized = true;
	        }
	        
	    }

	    private void CreateLogger() {
	        if (LoggerHost != "" && LoggerPort != 0 && LoggerName != "") {
                Console.WriteLine("LOGLEVEL " + LogLevel);
	            var def = Loggy.Get();
	            def.Level = LogLevel;
                def.Appenders.Add(new UdpAppender(LoggerHost,LoggerPort){AutoFlushSize = 1,Format = LoggerFormat});
                def.Debug("debug test тест");
                def.Trace("trace test тест");
                def.Info("info test тест");
                def.Warn("warning test тест");
                def.Error("error test тест", new Exception("Test exception"));
                def.Fatal("fatal test тест");
            }
	    }

	    public string LoggerFormat { get; set; }
        public bool RequireLogin { get; set; }
        public string LoginPage { get; set; }

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
            StaticContentCacheMap  = new ConcurrentDictionary<string, XElement>();
			Cached = new List<string>();
			AuthCookieName = "QHAUTH";
			AuthCookieDomain = "";
			AccessAllowOrigin = "";
			AccessAllowHeaders = "*";
			AccessAllowMethods = "GET, POST, OPTIONS";
			AccessAllowCredentials = true;
			StaticContentMap = new Dictionary<string, StaticFolderDescriptor>();
            Proxize = new Dictionary<string, string>();
            ConnectionStrings = new Dictionary<string, string>();
			Constants = new Dictionary<string, string>();
			Modules = new Dictionary<string, string>();
			Initializers = new List<string>();
			MachineName = Environment.MachineName;
	        MaxRequestSize = 10000000;
	        LoginPage = "/login.html";
	    }
		/// <summary>
		///		Имя машины
		/// </summary>
		public string MachineName {
			get { return _machineName; }
			set {
				if (value != null) value = value.ToLowerInvariant();
				_machineName = value;
			}
		}
        /// <summary>
        /// Перечень классов для инициализации
        /// </summary>
	    public IList<string> Initializers { get; set; }

	    /// <summary>
        /// 
        /// </summary>
	    public IDictionary<string, string> Modules { get; private set; }

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
		/// Имя логгера
		/// </summary>
        public string LoggerName { get; set; }
		/// <summary>
		/// Хост логгера
		/// </summary>
        public string LoggerHost { get; set; }
		/// <summary>
		/// Порт логгера
		/// </summary>
        public int LoggerPort { get; set; }

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
		        if (string.IsNullOrWhiteSpace(value)) return;
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
		public Scope Parameters{
			get { return _parameters ?? (_parameters = new Scope()); }
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
		/// 
		/// </summary>
		public int Port { get; set; }
		/// <summary>
		/// </summary>
		public string EncryptBasis { get; set; }
		/// <summary>
		///		Набор констант
		/// </summary>
		public IDictionary<string, string> Constants { get; private set; }
		/// <summary>
		/// Мапинг юрлов в диретории для Static Handler
		/// </summary>
		public IDictionary<string, StaticFolderDescriptor> StaticContentMap { get; private set; }
		/// <summary>
		///		Набор строк подключения
		/// </summary>
		public IDictionary<string, string> ConnectionStrings { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<string> SqlScripts {
			get {
				if (Definition == null) yield break;
				var scripts = Definition.Elements("sqlscript");
				var codes = scripts.Select(_ => _.Attr("code")).Where(_ => !string.IsNullOrWhiteSpace(_));
				foreach (var code in codes) {
					yield return EnvironmentInfo.ResolvePath(code);
				}
			}
		}
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
			foreach (var condition in xml.Elements("machine")) {
			    var machine = condition.Attr("code").ToLowerInvariant();
				var not = false;
				if (machine == "not" && !string.IsNullOrWhiteSpace(condition.Attr("name"))) {
					not = true;
					machine = condition.Attr("name").ToLowerInvariant();
				}
				if ((machine == MachineName && !not) || (not && machine != MachineName )) {
					var target = condition.Attr("use");
					if (context == null) throw new Exception("Cannot resolve machine-related config cause context is null");
					var config = context[target];
					if (config == null) throw new Exception("Cannot resolve machine-related config");
					xml = config.Compiled;
					Log.Info("Usage config " + target +" because machine name is " + (not ? "not " : "") + MachineName);
					break;
				}
		    }
	        this.BSharpContext = context;
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
	        MaxRequestSize = xml.ResolveValue("maxrequestsize", "10000000").ToInt();
	        RequireLogin = xml.ResolveValue("requirelogin").ToBool();
	        foreach (XElement bind in xml.Elements(HostUtils.BindingXmlName)) {
	            var excludehost = bind.Attr("excludehost").SmartSplit();
	            bool process = true;
	            if (0 != excludehost.Count) {
	                var machine = Environment.MachineName.ToUpperInvariant();
	                foreach (var h in excludehost) {
	                    if (machine == h.ToUpperInvariant().Trim()) {
	                        process = false;
                            break;
	                        
	                    }
	                }
	            }
                if(!process)continue;
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
		    foreach (var constant in xml.Elements("constant")) {
				if (string.IsNullOrWhiteSpace(constant.Attr("code"))) continue;
			    Constants[constant.Attr("code").ToLowerInvariant()] = constant.Attr("name");
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
	            var role = e.Attr("role");
	            if (!name.StartsWith("/")) {
	                name = "/" + name;
	            }
	            if (!name.EndsWith("/")) {
	                name += "/";
	            }
	            if (e.Attr("cache").ToBool()) {
	                this.StaticContentCacheMap[name] = e;
	            }
	            else {
	                this.StaticContentMap[name] = new StaticFolderDescriptor{Key=name,Path=folder,Role=role};
	            }
            } 
            foreach (var e in xml.Elements("startup"))
            {
                var name = e.Attr("code");
                Initializers.Add(name);
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
                Loggy.Info(string.Concat("AppId is [", appid , "]"));
	        }

            LoggerName = xml.ResolveValue("loggername", "");
            LoggerHost = xml.ResolveValue("loggerhost", "");
            LoggerPort = xml.ResolveValue("loggerport", "0").ToInt();
            LoggerFormat = xml.ResolveValue("loggerformat", "").Replace("%{","${");

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
	                        var server = e.Attr("server");
	                        var cp = "appid=" + sappid + ";";
	                        if (!string.IsNullOrWhiteSpace(server)) {
	                            cp += "server=" + server;
	                        }
	                        this.Proxize[root] = cp;
	                    }

	                }
	            }
	        }
	    }

	    public int MaxRequestSize { get; set; }

	    /// <summary>
        /// Обратная ссылка на XML- определение
        /// </summary>
	    public XElement Definition { get; set; }
        /// <summary>
        /// Мапинг прокси адресов для их обработки на другом хосте
        /// </summary>
	    public IDictionary<string,string> Proxize { get; private set; }
        /// <summary>
        /// Мапинг локальных кэшей (в виде файлов конфигурации)
        /// </summary>
	    public IDictionary<string,XElement> StaticContentCacheMap { get; private set; }

	    public IBSharpContext BSharpContext { get; set; }

	    private void ReadModules(XElement xml) {
	        foreach (XElement e in xml.Elements("module")) {
	            var fname = e.Attr("code");
	            var code = fname;
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
	            Modules[code] = fname;

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
					Port = Port == 0 ? HostUtils.DefaultBindingPort : Port,
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