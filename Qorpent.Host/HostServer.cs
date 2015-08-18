using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using qorpent.v2.security.authentication;
using qorpent.v2.security.authorization;
using Qorpent.Applications;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Data;
using Qorpent.Experiments;
using Qorpent.Host.Handlers;
using Qorpent.Host.Qweb;
using Qorpent.Host.Security;
using Qorpent.Host.Static;
using Qorpent.IoC;
using Qorpent.IO;
using Qorpent.IO.Http;
using Qorpent.IO.Net;
using Qorpent.Log;
using Qorpent.Mvc;
using Qorpent.Utils.Extensions;
using Qorpent.v2.security.authorization;

namespace Qorpent.Host {
    /// <summary>
    ///     Http сервер Qorpent
    /// </summary>
    public class HostServer : IHostServer, IHostConfigProvider,IConfigProvider {
        internal CancellationToken _cancel;
        private IEncryptProvider _encryptor;
        private IRequestHandlerFactory _factory;
        private HttpListener _listener;
        private IHostServerStaticResolver _static;
        private IHttpAuthenticator _auth;
        private INotAuthProcessProvider _notAuth;
        private IHttpAuthorizer _httpAuthorizer;

        /// <summary>
        ///     Создает новый экземпляр сервера
        /// </summary>
        /// <param name="config"></param>
        public HostServer(HostConfig config) {
            Config = config;
        }

        /// <summary>
        ///     Создает сервер на определенном порту
        /// </summary>
        /// <param name="port"></param>
        public HostServer(int port) {
            var cfg = new HostConfig();
            cfg.AddDefaultBinding();
            cfg.Bindings[0].Port = port;
            Config = cfg;
        }

        /// <summary>
        /// </summary>
        public HostServer() {
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
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        ///     Test option to check queue behavior
        /// </summary>
        public bool SingleWaiter { get; set; }

        /// <summary>
        ///     Получить конфигурацию
        /// </summary>
        /// <returns></returns>
        public HostConfig GetConfig() {
            return Config;
        }

        public IBSharpContext GetContext() {
            return Config.BSharpContext;
        }

        /// <summary>
        ///     Акцессор к контейнеру
        /// </summary>
        public IContainer Container { get; private set; }

        /// <summary>
        /// </summary>
        public IEncryptProvider Encryptor {
            get { return _encryptor; }
            private set {
                if (null != value && _encryptor != value) {
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
        /// 
        /// </summary>
        public IHttpAuthenticator Authenticator {
            get { return _auth ??(_auth=Container.Get<IHttpAuthenticator>() ?? new HttpAuthenticator()); }
            set { _auth = value; }
        }

        public IHttpAuthorizer Authorizer {
            get { return _httpAuthorizer ?? (_httpAuthorizer = Container.Get<IHttpAuthorizer>() ?? new HttpAuthorizer()); }
            set { _httpAuthorizer = value; }
        }


        /// <summary>
        ///     Конфигурация
        /// </summary>
        public HostConfig Config { get; set; }

        /// <summary>
        /// </summary>
        public IHostServerStaticResolver Static {
            get { return _static; }
            private set {
                if (null != value && _static != value) {
                    _static = value;
                    _static.Initialize(this);
                }
            }
        }

        

        /// <summary>
        ///     Запускает сервер
        /// </summary>
        public void Start() {
            lock (this) {
                Initialize();

                if (State == HostServerState.Run) {
                    return;
                }
                if (State == HostServerState.Fail) {
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
        public IRequestHandlerFactory Factory {
            get {
                if (null == _factory) {
                    Initialize();
                }
                return _factory;
            }
            private set { _factory = value; }
        }

        /// <summary>
        ///     Завершает работу веб-сервера
        /// </summary>
        public void Stop() {
            lock (this) {
                if (State == HostServerState.Run) {
                    Close();
                    State = HostServerState.Stopped;
                }
            }
        }

        /// <summary>
        ///     Инициализирует сервер
        /// </summary>
        public void Initialize() {
            if (State == HostServerState.Initial) {
                Config.Initialize();
                InitializeLibraries();
                InitializeApplication();
                InitializeHttpServer();
                InitializeDefaultHandlers();
                InitializeCommands();
                State = HostServerState.Initalized;
            }
        }

        /// <summary>
        ///     Непосредственно запускает сервер
        /// </summary>
        private void Run() {
            CancellationTokenSource = new CancellationTokenSource();
            _cancel = CancellationTokenSource.Token;
	        foreach (var prefix in _listener.Prefixes) {
		        Console.WriteLine("Prefix: " + prefix);
	        }
            Task.Run(
                () => {
                    _listener.Start();
                    StartRequestThread();
                   if (!SingleWaiter) {
                        StartRequestThread();
                    }
                }, _cancel
                );
        }

        internal void StartRequestThread() {
            _listener.GetContextAsync().ContinueWith(OnRequest, _cancel);
        }

        private void OnRequest(Task<HttpListenerContext> task) {
            WebContext wc = null;
            try {
                StartWaitNextRequest();
                wc = task.Result;
                if (CheckInvalidStartupConditions(wc)) {
                    return;
                }
                PrepareForCrossSiteScripting(task);
                if (CheckOptionsMethodIsCalled(task)) {
                    return;
                }
                if (wc.Request.ContentLength > Config.MaxRequestSize) {
                    throw    new Exception("Exceed max request size");
                }
                CopyCookies(wc);
                Authenticator.Authenticate(wc.Request,wc.Response);
                if (Applications.Application.HasCurrent)
                {
                    Applications.Application.Current.Principal.SetCurrentUser(wc.User);
                }
                var authorization = Authorizer.Authorize(wc.Request);
               
                if (BeforeHandlerProcessed(wc,authorization)) {
                    if (!wc.Response.WasClosed) {
                        wc.Response.Close();
                    }
                    return;
                }

                new HostRequestHandler(this, wc).Execute();
            }
            catch (Exception ex) {
                if (!wc.Response.WasClosed) {
                    wc.Finish("some error occured " + ex, status: 500);
                }
            }
        }

        private void CopyCookies(WebContext _context)
        {
            foreach (Cookie cookie in _context.Cookies)
            {
                cookie.Path = "/";
                cookie.HttpOnly = true;
                cookie.Secure = true;
                cookie.Domain = HttpUtils.AdaptCookieDomain(cookie.Domain);
                _context.Response.Cookies.Add(cookie);
            }
            _context.Response.Cookies = _context.Request.Cookies;
        }


        private bool BeforeHandlerProcessed(WebContext wc,AuthorizationReaction authorization) {

           
            if (authorization.Process) return false;
            if (!string.IsNullOrWhiteSpace(authorization.Redirect)) {
                wc.Redirect(authorization.Redirect);
                return true;
            }
            wc.Finish(new{error="not auth"}.stringify(),status:500);
            return true;
        }

        private bool CheckInvalidStartupConditions(WebContext ctx) {
            if (Application.IsInStartup) {
                ctx.Finish("application is in startup", status: 500);
                return true;
            }
            if (Application.StartupError != null) {
                ctx.Finish("startup error \r\n" + Application.StartupError, status: 500);
                return true;
            }
            return false;
        }

        private void StartWaitNextRequest() {
            if (!_cancel.IsCancellationRequested) {
                StartRequestThread();
            }
        }

        private void PrepareForCrossSiteScripting(Task<HttpListenerContext> task) {
            if (!string.IsNullOrWhiteSpace(task.Result.Request.Headers["QHPROXYORIGIN"])) {
                return;
            }
            if (!string.IsNullOrWhiteSpace(Config.AccessAllowOrigin)) {
                if (Config.AccessAllowOrigin == "*" && Config.AccessAllowCredentials) {
                    var origin = task.Result.Request.Headers["Origin"];
                    if (!string.IsNullOrWhiteSpace(origin)) {
                        task.Result.Response.AddHeader("Access-Control-Allow-Origin", origin);
                    }
                }
                else {
                    task.Result.Response.AddHeader("Access-Control-Allow-Origin", Config.AccessAllowOrigin);
                }
                if (!string.IsNullOrWhiteSpace(Config.AccessAllowHeaders)) {
                    if ("*" == Config.AccessAllowHeaders) {
                        if (task.Result.Request.Headers.AllKeys.Contains("Access-Control-Request-Headers")) {
                            task.Result.Response.AddHeader("Access-Control-Allow-Headers",
                                task.Result.Request.Headers["Access-Control-Request-Headers"]);
                        }
                    }
                    else {
                        task.Result.Response.AddHeader("Access-Control-Allow-Headers", Config.AccessAllowHeaders);
                    }
                }

                task.Result.Response.AddHeader("Access-Control-Allow-Credentials",
                    Config.AccessAllowCredentials.ToString().ToLowerInvariant());
                task.Result.Response.AddHeader("Access-Control-Allow-Methods", Config.AccessAllowMethods);
            }
        }

        private static bool CheckOptionsMethodIsCalled(Task<HttpListenerContext> task) {
            if (task.Result.Request.HttpMethod == "OPTIONS") {
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

        private void InitializeCommands() {
            if (null != Config.Definition) {
                var commands = Config.Definition.Elements("command").ToArray();
                foreach (var element in commands) {
                    var commandname = element.Attr("code");
                    var type = element.Attr("name");
                    var htype = Type.GetType(type);
                    if (null == htype) {
                        throw new Exception("cannot find command " + type);
                    }
                    var handler = Activator.CreateInstance(htype) as IRequestHandler;
                    if (null == handler) {
                        throw new Exception("Is not IRequestHandler " + type);
                    }
                    Factory.Register("/" + commandname, handler);
                }
            }
        }

        private void InitializeDefaultHandlers() {
            this.OnResponse("/_stat",
                _ => _.Finish(string.Format("{{\"requestCount\":{0}}}", RequestCount)));
            this.OnResponse("/_static/cache/drop", _ => { });
            this.OnContext("/toxml", _ => new SmartXmlHandler().Process(_));
            this.OnContext("/save", _ => new SaveHandler().Run(this, _, null, CancellationToken.None));
            this.OnContext("/load", _ => new LoadHandler().Run(this, _, null, CancellationToken.None));
            this.On("/js/_plugins.js", BuildPluginsModule(), "text/javascript");

        }

        private string BuildPluginsModule() {
            var result = new StringBuilder();

            var plugins = (Config.Definition ?? new XElement("_stub")).Elements("plugin");
            var pluginlist = string.Join(" , ",
                new[] {"'angular'", "'jquery'"}.Union(plugins.Select(_ => "'" + _.Attr("code") + "'")));
            var arglist = string.Join(" , ",
                new[] {"angular", "$"}.Union(plugins.Select(_ => _.Attr("code").Replace("/", "_").Replace("-", "_"))));
            result.Append("define([");
            result.Append(pluginlist);
            result.Append("], function(");
            result.AppendLine();
            result.Append("\t");
            result.Append(arglist);
            result.Append(") {");
            result.AppendLine();
            result.AppendLine("\tvar plugins={ _set : [], _map: {}, execute : function( name, args, context) {");
            result.AppendLine("\t\targs = args || [];");
            result.AppendLine("\t\tthis._set.forEach(function(_){");
            result.AppendLine("\t\t\tif(name in _){");
            result.AppendLine("\t\tvar _this = context || _;");
            result.AppendLine("\t\t\t\t_[name].apply(_this,args);");
            result.AppendLine("\t\t\t}");
            result.AppendLine("\t\t});");
            result.AppendLine("\t} }");
            foreach (var element in plugins) {
                var name = element.Attr("code").Replace("/", "_").Replace("-", "_");
                result.AppendLine("\tplugins._map['" + name + "'] = " + name + ";");
                result.AppendLine("\tplugins._set.push(" + name + ");");
            }
            result.AppendLine("\tvar module = angular.module('plugins',[]);");
            result.AppendLine("\tmodule.factory('plugins',function(){return plugins;});");
            result.AppendLine("\treturn plugins;");
            result.AppendLine("});");
            return result.ToString();
        }

        /// <summary>
        ///     Загружает библиотеки
        /// </summary>
        private void InitializeLibraries() {
            if (Directory.Exists(Config.DllFolder)) {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
            var name = args.Name.Split(',')[0];
            if (!name.EndsWith(".dll")) {
                name += ".dll";
            }
            var file = Path.Combine(Config.DllFolder, name);
            if (File.Exists(file)) {
                var pdb = file.Replace(".dll", ".pdb");
                if (File.Exists(pdb)) {
                    return Assembly.Load(File.ReadAllBytes(file), File.ReadAllBytes(pdb));
                }
                return Assembly.Load(File.ReadAllBytes(file));
            }
            return null;
        }

        /// <summary>
        ///     Инициализирует IoC
        /// </summary>
        private void InitializeApplication() {
            Application = Config.ApplicationMode == HostApplicationMode.Shared
                ? Applications.Application.Current
                : new Application();
            EnvironmentInfo.Constants = Config.Constants;
            if (Config.ApplicationMode == HostApplicationMode.Standalone) {
                Applications.Application.Current = Application;
                EnvironmentInfo.RootDirectory = Config.RootFolder;
                EnvironmentInfo.BinDirectory = Config.DllFolder;
                EnvironmentInfo.ConfigDirectory = Config.ConfigFolder;
                EnvironmentInfo.TmpDirectory = Config.TmpFolder;
                EnvironmentInfo.LogDirectory = Config.LogFolder;
                EnvironmentInfo.IsWeb = true;
                EnvironmentInfo.IsWebUtility = false;
            }
            if (Config.ApplicationMode != HostApplicationMode.Shared) {
                Container = ContainerFactory.CreateEmpty();
                ContainerFactory.SetupWellKnownContainerServices(Container);
            }
            else {
                Container = Application.Container;
            }
            if (Config.ApplicationMode == HostApplicationMode.FullStandalone) {
                Container.GetLoader().LoadDefaultManifest(true);
            }
            LoadContainer();
            _InitializeDefaultServices();


            if (Config.ApplicationMode != HostApplicationMode.Shared) {
                _InitializeForStandaloneApplication();
            }


            foreach (var i in Container.All<IHostServerInitializer>()) {
                i.Initialize(this);
            }
            foreach (var initializer in Config.Initializers) {
                var type = Type.GetType(initializer);
                if (null == type) {
                    throw new Exception("cannot find initializator " + initializer);
                }
                var init = Activator.CreateInstance(type) as IHostServerInitializer;
                if (null == init) {
                    throw new Exception("invalid class to use as initializer: " + initializer);
                }
                init.Initialize(this);
            }
            /*
            if (Config.ApplicationMode != HostApplicationMode.Shared)
            {
                Application.Container = Container;
            }
             */
        }

        private void _InitializeForStandaloneApplication() {
            if (Config.ApplicationMode != HostApplicationMode.Shared) {
                Container.Unregister(Container.FindComponent(typeof (IFileNameResolver), ""));
                Container.Register(
                    Container.NewComponent<IFileNameResolver, HostFileNameResolver>(
                        implementation: new HostFileNameResolver(this)));
                Container.Register(Container.NewComponent<IMvcContext, HostMvcContext>());
                Container.Unregister(Container.FindComponent(typeof (IAction), "_sys.login.action"));
                Container.Unregister(Container.FindComponent(typeof (IAction), "_sys.logout.action"));
                Container.Register(Container.NewComponent<IHostConfigProvider, HostServer>(implementation: this));

                var logger = (BaseLogger) ConsoleLogWriter.CreateLogger(
                    level: Config.LogLevel,
                    customFormat: "${Level} ${Time} ${Message}"
                    );
                logger.Mask = "*";

                Application.Container.Register(Container.NewComponent<ILogger, BaseLogger>(implementation: logger));

                Application.PerformAsynchronousStartup();
            }
        }

        public event Action<IContainer> OnBeforeInitializeServices;
        public event Action<IContainer> OnAfterInitializeSerives;

        private void _InitializeDefaultServices() {
            if (null != OnBeforeInitializeServices) {
                OnBeforeInitializeServices(Container);
            }
           
           
            if (null == Container.FindComponent(typeof (IRequestHandlerFactory), null)) {
                Container.Register(
                    Container.NewComponent<IRequestHandlerFactory, DefaultRequestHandlerFactory>(Lifestyle.Transient));
            }
            if (null == Container.FindComponent(typeof (IHostServerStaticResolver), null)) {
                Container.Register(
                    Container.NewComponent<IHostServerStaticResolver, HostServerStaticResolver>(Lifestyle.Transient));
            }


            if (null == Container.FindComponent(typeof (IEncryptProvider), null)) {
                Container.Register(Container.NewComponent<IEncryptProvider, Encryptor>(Lifestyle.Transient));
            }
            Factory = Container.Get<IRequestHandlerFactory>();
            Static = Container.Get<IHostServerStaticResolver>();
            Encryptor = Container.Get<IEncryptProvider>();
            foreach (var map in Config.StaticContentMap) {
                Static.SetRoot(map.Key, map.Value);
            }
            foreach (var map in Config.StaticContentCacheMap) {
                Static.SetCachedRoot(map.Key, map.Value);
            }
            Container.Register(Container.NewExtension(Config.Log, "mainlog"));
            if (null != OnAfterInitializeSerives)
            {
                OnAfterInitializeSerives(Container);
            }
        }

        private void LoadContainer() {
            var loader = Container.GetLoader();
            if (Directory.Exists(Config.ConfigFolder)) {
                var xml = new XElement("result");
                var bxl = new BxlParser();
                foreach (var file in Directory.GetFiles(Config.ConfigFolder, "*.ioc-manifest.bxl")) {
                    if (Config.IsConfigFileMatch(file)) {
                        var filexml = bxl.Parse(File.ReadAllText(file));
                        xml.Add(filexml.Elements());
                    }
                }
                loader.LoadManifest(xml, true);
            }
            if (null == Container.FindComponent(typeof(IHostConfigProvider), null))
            {
                Container.Register(Container.NewComponent<IHostConfigProvider, HostServer>(implementation: this));
            }
            if (null == Container.FindComponent(typeof(IConfigProvider), null))
            {
                Container.Register(Container.NewComponent<IConfigProvider, HostServer>(implementation: this));
            }
            if (0 != Config.ConnectionStrings.Count) {
                var connections = Container.Get<IDatabaseConnectionProvider>();
                if (null == connections) {
                    throw new Exception("No connection provider");
                }
                foreach (var cs in Config.ConnectionStrings) {
                    var dsc = new ConnectionDescriptor {
                        ConnectionString = cs.Value,
                        Name = cs.Key,
                        PresereveCleanup = true
                    };
                    connections.Register(dsc, false);
                }
            }
            loader.LoadAssembly(typeof (HostServer).Assembly);
            loader.LoadAssembly(typeof (HttpAuthenticator).Assembly);
            foreach (var assemblyName in Config.AutoconfigureAssemblies) {
                var assembly = Assembly.Load(assemblyName);
                loader.LoadAssembly(assembly);
            }
        }

        private void InitializeHttpServer() {
            _listener = new HttpListener {
                AuthenticationSchemes = AuthenticationSchemes.Anonymous | AuthenticationSchemes.Basic
            };


            if (0 == Config.Bindings.Count) {
                Config.AddDefaultBinding();
            }
            foreach (var binding in Config.Bindings) {
                _listener.Prefixes.Add(binding.ToString());
            }
        }

        /// <summary>
        ///     Непосредственно закрывает сервер
        /// </summary>
        private void Close() {
            CancellationTokenSource.Cancel();
            if (_listener.IsListening) {
                _listener.Stop();
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string GetStatisticsString() {
            return string.Format("Request Count: {0}\r\nRequest Time: {1}", RequestCount, RequestTime);
        }

        XElement IConfigProvider.GetConfig() {
            return Config.Definition;
        }
    }
}