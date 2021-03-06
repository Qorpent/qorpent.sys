﻿#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/Application.cs
#endregion
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using qorpent.v2.security.authorization;
using Qorpent.Bxl;
using Qorpent.Data;
using Qorpent.Dsl;
using Qorpent.Events;
using Qorpent.IO;
using Qorpent.IO.Resources;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Mvc;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Applications {
	/// <summary>
	/// 	Qorpent system environment
	/// </summary>
	/// <remarks>
	/// </remarks>
	public class Application : IApplication {
		/// <summary>
		/// </summary>
		[ThreadStatic] private static IMvcContext _threadCurrentMvcContext;

#if PARANOID
		static Application() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
		#endif


		private static IApplication _current;

		/// <summary>
		/// 	Access to file system services
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ISerializerFactory Serialization {
			get {
				if (null == _serializerfactory) {
					lock (this) {
						return _serializerfactory = ResolveService<ISerializerFactory>();
					}
				}
				return _serializerfactory;
			}
		}

		/// <summary>
		/// 	Current Application (default context)
		/// </summary>
		public static IApplication Current {
			get {
				if (null == _current) {
					lock (EnvironmentInfo.Sync) {
						_current = new DefaultApplicationBuilder().CreateDefaultApplication();
					}
				}
				return _current;
			}
			set {
				lock (EnvironmentInfo.Sync) {
					_current = value;
				}
			}
		}

		/// <summary>
		/// 	True if current application prepared
		/// </summary>
		public static bool HasCurrent {
			get { return null != _current; }
		}


		/// <summary>
		/// 	Current QWeb Context (based on thread static)
		/// </summary>
		/// <value> The current MVC context. </value>
		/// <remarks>
		/// </remarks>
		public IMvcContext CurrentMvcContext {
			get { return _threadCurrentMvcContext; }
			set { _threadCurrentMvcContext = value; }
		}
#if !EMBEDQPT
        private IHttpContextWrapper _httpContextWrapper;
		/// <summary>
		/// Access to HTTP context wrapper
		/// </summary>
		public IHttpContextWrapper HttpWrapper { 
			get {
				lock(this) {
					if (null == _httpContextWrapper) {
						var wrapper = EnvironmentInfo.GetHttpWrapper();
						_httpContextWrapper = wrapper;
					}
					return _httpContextWrapper;
				}

			}
		}
#else

	    public IHttpContextWrapper HttpWrapper { get; }
#endif
        /// <summary>
        /// 	Access to Bxl service
        /// </summary>
        /// <remarks>
        /// </remarks>
        public IBxlService Bxl {
			get {
				if (null == _bxl) {
					lock (this) {
						return _bxl = ResolveService<IBxlService>();
					}
				}
				return _bxl;
			}
			set { _bxl = value; }
		}

		

		/// <summary>
		/// Служба доступа к ресурсам
		/// </summary>
		public IResourceProvider Resources {
			get {
				if (null == _resources) {
					lock (this) {
						return _resources = ResolveService<IResourceProvider>();
					}
				}
				return _resources;
			}
			set { _resources = value; }
		}

		


		/// <summary>
		/// 	Access to <see cref="IDatabaseConnectionProvider" /> service
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IDatabaseConnectionProvider DatabaseConnections {
			get {
				if (null == _dbconnecitons) {
					lock (this) {
						return _dbconnecitons = ResolveService<IDatabaseConnectionProvider>();
					}
				}
				return _dbconnecitons;
			}
			set { _dbconnecitons = value; }
		}

		/// <summary>
		/// 	Access to <see cref="IAccessProviderService" /> service
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IImpersonationProvider Impersonation {
			get {
				if (null == _impersonation) {
					lock (this) {
						return _impersonation = ResolveService<IImpersonationProvider>();
					}
				}
				return _impersonation;
			}
			set { _impersonation = value; }
		}

		/// <summary>
		/// 	Access to IoC container
		/// </summary>
		/// <value> The container. </value>
		/// <remarks>
		/// </remarks>
		public IContainer Container {
			get {
				if (null == _container) {
					lock (this) {
#if EMBEDQPT
					    return _container = new Container();
#else
                        return _container = ContainerFactory.ResolveWellKnown<IContainer>();
#endif
					}
				}
				return _container;
			}
			set {
				if (null != _container) {
					throw new QorpentException("it's not allowed to attach container to application twise");
				}
				_container = value;
				_container.RegisterExtension(new ApplicationBoundContainerExtension(this));
				var applicationBound = _container as IApplicationBound;
				if (applicationBound != null) {
					applicationBound.SetApplication(this);
				}
			}
		}

		/// <summary>
		/// 	Access to file system services
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IFileService Files {
			get {
				if (null == _files) {
					lock (this) {
						return _files = ResolveService<IFileService>();
					}
				}
				return _files;
			}
		}

		/// <summary>
		/// 	Access to mvc factory
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IMvcFactory MvcFactory {
			get {
				if (null == _mvcfactory) {
					lock (this) {
						return _mvcfactory = ResolveService<IMvcFactory>();
					}
				}
				return _mvcfactory;
			}
		}


		

		/// <summary>
		/// 	Access to Application logger
		/// </summary>
		/// <remarks>
		/// </remarks>
		public ILogManager LogManager {
			get {
				if (null == _log) {
					lock (this) {
						return _log = ResolveService<ILogManager>();
					}
				}
				return _log;
			}
		}

		/// <summary>
		/// 	Access to principal source
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IPrincipalSource Principal {
			get {
				if (null == _principals) {
					lock (this) {
						return _principals = ResolveService<IPrincipalSource>() ?? new DefaultPrincipalSource();
					}
				}
				return _principals;
			}
		}

		/// <summary>
		/// 	Access to role resolver
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IRoleResolverService Roles {
			get {
				if (null == _roles) {
					lock (this) {
                        return _roles = ResolveService<IRoleResolverService>();
					}
				}
				return _roles;
			}
            set { _roles = value; }
		}


		/// <summary>
		/// 	Creates new MVC context
		/// </summary>
		/// <param name="createContext"> </param>
		/// <returns> </returns>
		public IMvcContext CreateContext(object createContext) {
			lock (this) {
				var ctx = Container.Get<IMvcContext>();
				if (null != createContext) {
					if (createContext is string) {
						ctx.Uri = new Uri((string) createContext);
					}
					else if (createContext is Uri) {
						ctx.Uri = (Uri) createContext;
					}
					else {
						ctx.SetNativeContext(createContext);
					}
				}
				return ctx;
			}
		}


		/// <summary>
		/// 	Access to Application event manager
		/// </summary>
		/// <remarks>
		/// </remarks>
		public IEventManager Events {
			get {
				if (null == _events) {
					lock (this) {
						return _events = (ResolveService<IEventManager>());
					}
				}
				return _events;
			}
		}

		/// <summary>
		/// 	Logical root directory for Application Environment.CurrentDir for console and ~/ for web
		/// </summary>
		/// <value> The root directory. </value>
		/// <remarks>
		/// </remarks>
		public string RootDirectory {
			get { return _rootdirectory ?? EnvironmentInfo.RootDirectory; }
			set { _rootdirectory = value; }
		}

		/// <summary>
		/// 	Codebase directory for Application EntryAssembly.Codebase для console and ~/bin for web
		/// </summary>
		/// <value> The bin directory. </value>
		/// <remarks>
		/// </remarks>
		public string BinDirectory {
			get { return _codeDirectory ?? EnvironmentInfo.BinDirectory; }
			set { _codeDirectory = value; }
		}

	

		/// <summary>
		/// 	Web Application root name
		/// </summary>
		/// <value> The name of the application. </value>
		/// <remarks>
		/// </remarks>
		public string ApplicationName {
			get {
				lock (this) {
					if (null == _applicationName) {
						
					}
				}
				return _applicationName;
			}
			set { _applicationName = value; }
		}





		/// <summary>
		/// 	simple synchronization method, waits while Application lock released
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void Synchronize() {
			lock (_appsync) {}
		}

		/// <summary>
		/// 	returns Application wide lock object
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public object GetApplicationLock() {
			return _appsync;
		}

		private Task starter;

		/// <summary>
		/// 	Called by web- infrastructure to execute statrtup - MUST BE ASYNC
		/// </summary>
		public void PerformAsynchronousStartup(){
			starter = Task.Run(() => PerformSynchronousStartup());
		}
		/// <summary>
		/// 
		/// </summary>
		public void WaitStartUp(){
			if (null != starter){
				starter.Wait();
			}else if (IsInStartup){
				while (IsInStartup){
					Thread.Sleep(20);
				}
			}
		}

		/// <summary>
		/// 	Indicates that application is in startup mode
		/// </summary>
		public bool IsInStartup { get; protected set; }

		/// <summary>
		/// 	Indicates that current application was startupped
		/// </summary>
		public bool StartupProcessed { get; protected set; }

		/// <summary>
		/// 	Error, occured during startup
		/// </summary>
		public Exception StartupError { get; protected set; }

		/// <summary>
		/// 	Called by console application - must execute some startup logic in SYNCHRONOUS
		/// </summary>
		public void PerformSynchronousStartup() {
			if (StartupProcessed) {
				return;
			}
			if (StartupError != null) {
				throw StartupError;
			}
			lock (this) {
				if (StartupProcessed) {
					return;
				}
				if (StartupError != null) {
					throw StartupError;
				}
				IsInStartup = true;
				try {
					var startups = Container.All<IApplicationStartup>().ToArray();
					var log = LogManager.GetLog(GetType().FullName + ";statup", this);
					log.Info("start startup", this);
					try {
						foreach (var applicationStartuper in startups) {
							log.Debug("startup: " + applicationStartuper, this);
							applicationStartuper.Execute(this);
							log.Trace("startup-finish: " + applicationStartuper, this);
						}
						StartupProcessed = true;
						log.Info("finish startup", this);
						StartTime = DateTime.Now;
					}
					catch (Exception ex) {
						log.Fatal("error in application startup");
						StartupError = ex;

						//throw;
					}
				}
				catch (Exception ex) {
					StartupError = ex;
				}
				finally {
					IsInStartup = false;
				}
			}
		}

		/// <summary>
		/// 	Время реального старта приложения (может использоваться для установки различных меток времени)
		/// </summary>
		public DateTime StartTime { get; set; }


		/// <summary>
		/// 	resolves application-wide service both from container and wellknown registry
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		protected T ResolveService<T>() where T : class {

            return Container.Get<T>() ?? ContainerFactory.ResolveWellKnown<T>(this);

		}

		/// <summary>
		/// </summary>
		private readonly object _appsync = new object();

		private string _applicationName;

		/// <summary>
		/// </summary>
		private IBxlService _bxl;

		/// <summary>
		/// </summary>
		private string _codeDirectory;

		/// <summary>
		/// </summary>
		private IContainer _container;


		/// <summary>
		/// </summary>
		private IEventManager _events;

		/// <summary>
		/// </summary>
		private IFileService _files;

		private IImpersonationProvider _impersonation;

		/// <summary>
		/// </summary>
		private bool? _isweb;

		/// <summary>
		/// </summary>
		private bool? _iswebUtility;

		/// <summary>
		/// </summary>
		private ILogManager _log;

		private IMvcFactory _mvcfactory;
		private IPrincipalSource _principals;
		private IRoleResolverService _roles;

		/// <summary>
		/// </summary>
		private string _rootdirectory;

		private ISerializerFactory _serializerfactory;
		private IDatabaseConnectionProvider _dbconnecitons;
		private IResourceProvider _resources;
	}
}