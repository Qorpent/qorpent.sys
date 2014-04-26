using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.Host.Qweb;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc;

namespace Qorpent.Host
{
	/// <summary>
	/// Http сервер Qorpent
	/// </summary>
	public class HostServer : IHostServer
	{
		/// <summary>
		/// Создает новый экземпляр сервера
		/// </summary>
		/// <param name="config"></param>
		public HostServer(HostConfig config)
		{
			Config = config;
		}
		/// <summary>
		/// Создает сервер на определенном порту
		/// </summary>
		/// <param name="port"></param>
		public HostServer(int port)
		{
			var cfg = new HostConfig();
			cfg.AddDefaultBinding();
			cfg.Bindings[0].Port = port;
			Config = cfg;
		
		}
		/// <summary>
		/// Состояние сервера
		/// </summary>
		public HostServerState State { get; private set; }

		/// <summary>
		/// Конфигурация
		/// </summary>
		public HostConfig Config { get; set; }
		/// <summary>
		/// 
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
		/// Запускает сервер
		/// </summary>
		public void Start()
		{
			lock (this)
			{
				Initialize();
				
				if (State == HostServerState.Run)
				{
					return;
				}
				if (State == HostServerState.Fail)
				{
					throw new Exception("Cannot run server after unhandled error");
				}
				State = HostServerState.Run;
				Run();
			}

		}
		/// <summary>
		/// Непосредственно запускает сервер
		/// </summary>
		private void Run()
		{
			_cancelSrc = new CancellationTokenSource();
			_cancel = CancellationTokenSource.Token;
			Task.Run(
				() =>
				{
					_listener.Start();
					StartRequestThread();
					
				}, _cancel
			);
		}

		internal void StartRequestThread()
		{
			_listener.GetContextAsync().ContinueWith(OnRequest);
		}

		private void OnRequest(Task<HttpListenerContext> task)
		{
			if (!_cancel.IsCancellationRequested)
			{
				StartRequestThread();
			}
			if (Application.IsInStartup)
			{
				task.Result.Response.Finish("application is in startup", status: 500);
				return;
			}
			if (Application.StartupError != null)
			{
				task.Result.Response.Finish("startup error \r\n" + Application.StartupError, status: 500);
				return;
			}
			new HostRequestHandler(this, task.Result).Execute();
		}

		private HttpListener _listener;
		internal CancellationToken _cancel;
		/// <summary>
		/// Контекст приложения
		/// </summary>
		public IApplication Application { get; private set; }

		/// <summary>
		/// Инициализирует сервер
		/// </summary>
		private void Initialize()
		{
			if (State == HostServerState.Initial)
			{
				InitializeLibraries();
				InitializeApplication();
				InitializeHttpServer();
				InitializeDefaultHandlers();
				State = HostServerState.Initalized;
			}
		}

		private void InitializeDefaultHandlers()
		{
			this.OnContext("/_stat", _ => _.Response.Finish(string.Format("{{\"requestCount\":{0}}}", RequestCount), "application/json"));
			this.OnContext("/_static/cache/drop", _ =>{
				
			});
			this.OnContext("/toxml", _ => new SmartXmlHandler().Process(_));
		}

		/// <summary>
		/// Загружает библиотеки
		/// </summary>
		private void InitializeLibraries()
		{
			if (Directory.Exists(Config.DllFolder))
			{
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			}
		}

		Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			var name = args.Name.Split(',')[0];
			if (!name.EndsWith(".dll")) name += ".dll";
			var file = Path.Combine(Config.DllFolder, name);
			if (File.Exists(file)) {
			    var pdb = file.Replace(".dll", ".pdb");
                if (File.Exists(pdb)) {
                    return Assembly.Load(File.ReadAllBytes(file),File.ReadAllBytes(pdb));
                }
				return Assembly.Load(File.ReadAllBytes(file));
			}
			return null;
		}

		/// <summary>
		/// Инициализирует IoC
		/// </summary>
		private void InitializeApplication()
		{
			
			Application = Config.ApplicationMode==HostApplicationMode.Shared? Applications.Application.Current : new Application();

			if (Config.ApplicationMode == HostApplicationMode.Standalone)
			{
				Applications.Application.Current = Application;
				EnvironmentInfo.RootDirectory = Config.RootFolder;
				EnvironmentInfo.BinDirectory = Config.DllFolder;
				EnvironmentInfo.ConfigDirectory = Config.ConfigFolder;
				EnvironmentInfo.TmpDirectory = Config.TmpFolder;
				EnvironmentInfo.LogDirectory = Config.LogFolder;
				EnvironmentInfo.IsWeb = true;
				EnvironmentInfo.IsWebUtility = false;
			}
			if (Config.ApplicationMode != HostApplicationMode.Shared)
			{

				_container = ContainerFactory.CreateEmpty();
				ContainerFactory.SetupWellKnownContainerServices(_container);
				Application.Container = _container;

			}
			else
			{
				_container = Application.Container;
			}
			LoadContainer();
			_InitializeDefaultServices();
			if (Config.ApplicationMode != HostApplicationMode.Shared){
				_InitializeForStandaloneApplication();
			}
			foreach (var i in _container.All<IHostServerInitializer>()){
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

		private  void _InitializeDefaultServices(){
			
			if (null == _container.FindComponent(typeof (IRequestHandlerFactory), null)){
				_container.Register(_container.NewComponent<IRequestHandlerFactory, DefaultRequestHandlerFactory>(Lifestyle.Singleton));
			}
			if (null == _container.FindComponent(typeof(IHostServerStaticResolver), null))
			{
				_container.Register(_container.NewComponent<IHostServerStaticResolver, HostServerStaticResolver>(Lifestyle.Singleton));
			}
			Factory = _container.Get<IRequestHandlerFactory>();
			Static = _container.Get<IHostServerStaticResolver>();
		}

		/// <summary>
		/// Фабрика хэндлеров
		/// </summary>
		public IRequestHandlerFactory Factory
		{
			get
			{
				if (null == _factory)
				{
					Initialize();	
				}
				return _factory;
			}
			private set { _factory = value; }
		}

		private void LoadContainer()
		{
			if (Directory.Exists(Config.ConfigFolder))
			{
				var xml = new XElement("result");
				var bxl = new BxlParser();
				foreach (var file in Directory.GetFiles(Config.ConfigFolder, "*.ioc-manifest.bxl"))
				{
					if (Config.IsConfigFileMatch(file))
					{
						var filexml = bxl.Parse(File.ReadAllText(file));
						xml.Add(filexml.Elements());
					}
				}
				var loader = _container.GetLoader();
				loader.LoadManifest(xml, true);
				foreach (var assemblyName in Config.AutoconfigureAssemblies){
					var assembly = Assembly.Load(assemblyName);
					loader.LoadAssembly(assembly);
				}
			}
		}

		private void InitializeHttpServer()
		{
			
			_listener = new HttpListener{AuthenticationSchemes = AuthenticationSchemes.Anonymous | AuthenticationSchemes.Basic};
			
			if (0 == Config.Bindings.Count)
			{
				Config.AddDefaultBinding();
			}
			foreach (var binding in Config.Bindings)
			{
				
				_listener.Prefixes.Add(binding.ToString());
			}
		}

		/// <summary>
		/// Завершает работу веб-сервера
		/// </summary>
	
		public void Stop()
		{
			lock (this)
			{
				if (State == HostServerState.Run)
				{
					Close();
					State = HostServerState.Stopped;
				}
			}
		}

		/// <summary>
		/// Непосредственно закрывает сервер
		/// </summary>
		private void Close()
		{
			CancellationTokenSource.Cancel();
			_listener.Stop();
		}

		
		private IRequestHandlerFactory _factory;
		private CancellationTokenSource _cancelSrc;
		private IContainer _container;
		private IHostServerStaticResolver _static;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetStatisticsString()
		{
			return string.Format("Request Count: {0}\r\nRequest Time: {1}", RequestCount, RequestTime);
		}
		/// <summary>
		/// Суммарное время обработки
		/// </summary>
		public TimeSpan RequestTime { get; set; }

		/// <summary>
		/// Количество обработанных запросов
		/// </summary>
		public int RequestCount { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public CancellationTokenSource CancellationTokenSource{
			get { return _cancelSrc; }
		}
	}
	
}
