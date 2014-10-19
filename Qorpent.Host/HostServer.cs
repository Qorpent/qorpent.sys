using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.BSharp.Preprocessor;
using Qorpent.Bxl;
using Qorpent.Host.Handlers;
using Qorpent.Host.Qweb;
using Qorpent.Host.Security;
using Qorpent.Host.Static;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc;

namespace Qorpent.Host{
	/// <summary>
	///     Http сервер Qorpent
	/// </summary>
	public class HostServer : IHostServer{
		private IAuthenticationProvider _auth;
		internal CancellationToken _cancel;
		private CancellationTokenSource _cancelSrc;
		private IContainer _container;
		private IEncryptProvider _encryptor;
		private IRequestHandlerFactory _factory;
		private HttpListener _listener;
		private IHostServerStaticResolver _static;

		/// <summary>
		///     Создает новый экземпляр сервера
		/// </summary>
		/// <param name="config"></param>
		public HostServer(HostConfig config){
			Config = config;
		}

		/// <summary>
		///     Создает сервер на определенном порту
		/// </summary>
		/// <param name="port"></param>
		public HostServer(int port){
			var cfg = new HostConfig();
			cfg.AddDefaultBinding();
			cfg.Bindings[0].Port = port;
			Config = cfg;
		}

		/// <summary>
		///     Суммарное время обработки
		/// </summary>
		public TimeSpan RequestTime { get; set; }

		/// <summary>
		///     Количество обработанных запросов
		/// </summary>
		public int RequestCount { get; set; }

		/// <summary>
		/// </summary>
		public CancellationTokenSource CancellationTokenSource{
			get { return _cancelSrc; }
		}

		/// <summary>
		/// </summary>
		public IEncryptProvider Encryptor{
			get { return _encryptor; }
			private set{
				if (null != value && _encryptor != value){
					_encryptor = value;
					_encryptor.Initialize(this);
				}
			}
		}


		/// <summary>
		///     Состояние сервера
		/// </summary>
		public HostServerState State { get; private set; }

		/// <summary>
		///     Конфигурация
		/// </summary>
		public HostConfig Config { get; set; }

		/// <summary>
		/// </summary>
		public IHostServerStaticResolver Static{
			get { return _static; }
			private set{
				if (null != value && _static != value){
					_static = value;
					_static.Initialize(this);
				}
			}
		}

		/// <summary>
		/// </summary>
		public IAuthenticationProvider Auth{
			get { return _auth; }
			private set{
				if (null != value && _auth != value){
					_auth = value;
					_auth.Initialize(this);
				}
			}
		}

		/// <summary>
		///     Запускает сервер
		/// </summary>
		public void Start(){
			lock (this){
				Initialize();

				if (State == HostServerState.Run){
					return;
				}
				if (State == HostServerState.Fail){
					throw new Exception("Cannot run server after unhandled error");
				}
				State = HostServerState.Run;
				Run();
			}
		}

		/// <summary>
		///     Контекст приложения
		/// </summary>
		public IApplication Application { get; private set; }

		/// <summary>
		///     Фабрика хэндлеров
		/// </summary>
		public IRequestHandlerFactory Factory{
			get{
				if (null == _factory){
					Initialize();
				}
				return _factory;
			}
			private set { _factory = value; }
		}

		/// <summary>
		///     Завершает работу веб-сервера
		/// </summary>
		public void Stop(){
			lock (this){
				if (State == HostServerState.Run){
					Close();
					State = HostServerState.Stopped;
				}
			}
		}

		/// <summary>
		///     Непосредственно запускает сервер
		/// </summary>
		private void Run(){
			_cancelSrc = new CancellationTokenSource();
			_cancel = CancellationTokenSource.Token;
			Task.Run(
				() =>{
					_listener.Start();
					StartRequestThread();
				}, _cancel
				);
		}

		internal void StartRequestThread(){
			_listener.GetContextAsync().ContinueWith(OnRequest);
		}

		private void OnRequest(Task<HttpListenerContext> task){
            StartWaitNextRequest();
			if (CheckInvalidStartupConditions(task)) return;
			PrepareForCrossSiteScripting(task);
			if (CheckOptionsMethodIsCalled(task)) return;
		    new HostRequestHandler(this, task.Result).Execute();
		}


		private bool CheckInvalidStartupConditions(Task<HttpListenerContext> task){
			if (Application.IsInStartup){
				task.Result.Response.Finish("application is in startup", status: 500);
				return true;
			}
			if (Application.StartupError != null){
				task.Result.Response.Finish("startup error \r\n" + Application.StartupError, status: 500);
				return true;
			}
			return false;
		}

		private void StartWaitNextRequest(){
			if (!_cancel.IsCancellationRequested){
				StartRequestThread();
			}
		}

		private  void PrepareForCrossSiteScripting(Task<HttpListenerContext> task){
			if (!string.IsNullOrWhiteSpace(Config.AccessAllowOrigin)){
				task.Result.Response.AddHeader("Access-Control-Allow-Origin",Config.AccessAllowOrigin);
				if (!string.IsNullOrWhiteSpace(Config.AccessAllowHeaders)){
					
					if ("*" == Config.AccessAllowHeaders){
						if (task.Result.Request.Headers.AllKeys.Contains("Access-Control-Request-Headers")){
							task.Result.Response.AddHeader("Access-Control-Allow-Headers",
							                               task.Result.Request.Headers["Access-Control-Request-Headers"]);
						}
					}
					else{
						task.Result.Response.AddHeader("Access-Control-Allow-Headers",Config.AccessAllowHeaders);
					}
				}
				
				task.Result.Response.AddHeader("Access-Control-Allow-Credentials",Config.AccessAllowCredentials.ToString().ToLowerInvariant());
				task.Result.Response.AddHeader("Access-Control-Allow-Methods",Config.AccessAllowMethods);
				
			}
		}

		private static bool CheckOptionsMethodIsCalled(Task<HttpListenerContext> task){
			if (task.Result.Request.HttpMethod == "OPTIONS"){
				task.Result.Response.AddHeader("Allow", "GET,POST,OPTIONS");
				task.Result.Response.AddHeader("Public", "GET,POST,OPTIONS");
				task.Result.Response.StatusCode = 200;
				task.Result.Response.StatusDescription = "OK";
				task.Result.Response.ContentLength64 = 0;
				task.Result.Response.Close();

				return true;
			}
			return false;
		}

		/// <summary>
		///     Инициализирует сервер
		/// </summary>
		private void Initialize(){
			if (State == HostServerState.Initial){
				InitializeLibraries();
				InitializeApplication();
				InitializeHttpServer();
				InitializeDefaultHandlers();
				State = HostServerState.Initalized;
			}
		}

		private void InitializeDefaultHandlers(){
			this.OnContext("/_stat",
			               _ => _.Response.Finish(string.Format("{{\"requestCount\":{0}}}", RequestCount), "application/json"));
			this.OnContext("/_static/cache/drop", _ => { });
			this.OnContext("/toxml", _ => new SmartXmlHandler().Process(_));
			this.OnContext("/logon", _ => Auth.Logon(_));
			this.OnContext("/logout", _ => Auth.Logout(_));
			this.OnContext("/isauth", _ => Auth.IsAuth(_));
		}

		/// <summary>
		///     Загружает библиотеки
		/// </summary>
		private void InitializeLibraries(){
			if (Directory.Exists(Config.DllFolder)){
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args){
			string name = args.Name.Split(',')[0];
			if (!name.EndsWith(".dll")) name += ".dll";
			string file = Path.Combine(Config.DllFolder, name);
			if (File.Exists(file)){
				string pdb = file.Replace(".dll", ".pdb");
				if (File.Exists(pdb)){
					return Assembly.Load(File.ReadAllBytes(file), File.ReadAllBytes(pdb));
				}
				return Assembly.Load(File.ReadAllBytes(file));
			}
			return null;
		}

		/// <summary>
		///     Инициализирует IoC
		/// </summary>
		private void InitializeApplication(){
			Application = Config.ApplicationMode == HostApplicationMode.Shared
				              ? Applications.Application.Current
				              : new Application();

			if (Config.ApplicationMode == HostApplicationMode.Standalone){
				Applications.Application.Current = Application;
				EnvironmentInfo.RootDirectory = Config.RootFolder;
				EnvironmentInfo.BinDirectory = Config.DllFolder;
				EnvironmentInfo.ConfigDirectory = Config.ConfigFolder;
				EnvironmentInfo.TmpDirectory = Config.TmpFolder;
				EnvironmentInfo.LogDirectory = Config.LogFolder;
				EnvironmentInfo.IsWeb = true;
				EnvironmentInfo.IsWebUtility = false;
			}
			if (Config.ApplicationMode != HostApplicationMode.Shared){
				_container = ContainerFactory.CreateEmpty();
				ContainerFactory.SetupWellKnownContainerServices(_container);
				Application.Container = _container;
			}
			else{
				_container = Application.Container;
			}
			if (Config.ApplicationMode == HostApplicationMode.FullStandalone){
				_container.GetLoader().LoadDefaultManifest(true);
			}
			LoadContainer();
			_InitializeDefaultServices();
			if (Config.ApplicationMode != HostApplicationMode.Shared){
				_InitializeForStandaloneApplication();
			}
			foreach (IHostServerInitializer i in _container.All<IHostServerInitializer>()){
				i.Initialize(this);
			}
		}

		private void _InitializeForStandaloneApplication(){
			if (Config.ApplicationMode != HostApplicationMode.Shared){
				_container.Register(_container.NewComponent<IMvcContext, HostMvcContext>());
				_container.Unregister(_container.FindComponent(typeof (IAction), "_sys.login.action"));
				_container.Unregister(_container.FindComponent(typeof (IAction), "_sys.logout.action"));
				try{
					_container.FindComponent(typeof (IFileNameResolver), null).Parameters["Root"] = Config.RootFolder;
				}
				catch{
				}
				var logger = (BaseLogger) ConsoleLogWriter.CreateLogger(
					level: Config.LogLevel,
					customFormat: "${Level} ${Time} ${Message}"
					                          );
				logger.Mask = "*";

				Application.Container.Register(_container.NewComponent<ILogger, BaseLogger>(implementation: logger));

				Application.PerformAsynchronousStartup();
			}
		}

		private void _InitializeDefaultServices(){
			if (null == _container.FindComponent(typeof (IRequestHandlerFactory), null)){
				_container.Register(
					_container.NewComponent<IRequestHandlerFactory, DefaultRequestHandlerFactory>(Lifestyle.Transient));
			}
			if (null == _container.FindComponent(typeof (IHostServerStaticResolver), null)){
				_container.Register(_container.NewComponent<IHostServerStaticResolver, HostServerStaticResolver>(Lifestyle.Transient));
			}

			if (null == _container.FindComponent(typeof (IAuthenticationProvider), null)){
				_container.Register(
					_container.NewComponent<IAuthenticationProvider, DefaultAuthenticationProvider>(Lifestyle.Transient));
			}
			if (null == _container.FindComponent(typeof (IEncryptProvider), null)){
				_container.Register(_container.NewComponent<IEncryptProvider, Encryptor>(Lifestyle.Transient));
			}
			Factory = _container.Get<IRequestHandlerFactory>();
			Static = _container.Get<IHostServerStaticResolver>();
			Auth = _container.Get<IAuthenticationProvider>();
			Encryptor = _container.Get<IEncryptProvider>();
			foreach (var map in Config.StaticContentMap){
				Static.SetRoot(map.Key,map.Value);
			}
		}

		private void LoadContainer(){
			IContainerLoader loader = _container.GetLoader();
			if (Directory.Exists(Config.ConfigFolder)){
				var xml = new XElement("result");
				var bxl = new BxlParser();
				foreach (string file in Directory.GetFiles(Config.ConfigFolder, "*.ioc-manifest.bxl")){
					if (Config.IsConfigFileMatch(file)){
						XElement filexml = bxl.Parse(File.ReadAllText(file));
						xml.Add(filexml.Elements());
					}
				}
				loader.LoadManifest(xml, true);
			}
			
			
			foreach (string assemblyName in Config.AutoconfigureAssemblies)
			{
				Assembly assembly = Assembly.Load(assemblyName);
				loader.LoadAssembly(assembly);
			}
		}

		private void InitializeHttpServer(){
			_listener = new HttpListener{AuthenticationSchemes = AuthenticationSchemes.Anonymous | AuthenticationSchemes.Basic};


			if (0 == Config.Bindings.Count){
				Config.AddDefaultBinding();
			}
			foreach (HostBinding binding in Config.Bindings){
				_listener.Prefixes.Add(binding.ToString());
			}
		}

		/// <summary>
		///     Непосредственно закрывает сервер
		/// </summary>
		private void Close(){
			CancellationTokenSource.Cancel();
			_listener.Stop();
		}


		/// <summary>
		/// </summary>
		/// <returns></returns>
		public string GetStatisticsString(){
			return string.Format("Request Count: {0}\r\nRequest Time: {1}", RequestCount, RequestTime);
		}
	}
}