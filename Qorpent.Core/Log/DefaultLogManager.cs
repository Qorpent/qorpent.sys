#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : DefaultLogManager.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Qorpent.IoC;

namespace Qorpent.Log {
	/// <summary>
	/// 	Default container-bound UserLog manager
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class DefaultLogManager : ServiceBase, ILogManager {
		/// <summary>
		/// </summary>
		public DefaultLogManager() {
			ErrorBehavior = InternalLoggerErrorBehavior.Log | InternalLoggerErrorBehavior.AutoDisable;
		}

		/// <summary>
		/// 	Loggers array to be used in logging
		/// </summary>
		[Inject] public IList<ILogger> Loggers {
			get {
				if (null == _loggers) {
					lock (Sync) {
						//тут нельзя использовать напрямую свойство Container, так как LogManager -
						//инфраструктурный сервис с созависимостями на время загрузки
						_loggers = new List<ILogger>();
						if (null != SourceContainer) {
							_loggers = SourceContainer.All<ILogger>().ToList();
						}
					}
				}
				return _loggers;
			}
			set {
				lock (Sync) {
					_loggers = value;
					if (null != _loggers) {}
				}
			}
		}


		/// <summary>
		/// 	Default error behavior
		/// </summary>
		public InternalLoggerErrorBehavior ErrorBehavior { get; set; }


		/// <summary>
		/// 	Fail safe UserLog gate to write internal exception without main logging context, but with max guarantee of regestering
		/// 	synchronous
		/// </summary>
		public void LogFailSafe(LogMessage message) {
			if (null == message) {
				return;
			}
			if (string.IsNullOrEmpty(message.Name)) {
				message.Name = "FAILSAFE";
			}
			try {
				StreamWriter wr = null;
				for (var i = 0; i < 100; i++) {
					try {
						wr = new StreamWriter(Path.Combine(EnvironmentInfo.RootDirectory, "failsafelog" + i + ".txt"), true, Encoding.UTF8);
						if (null != wr) {
							break;
						}
					}
					catch {}
				}
				if (null == wr) {
					var tmpfile = Path.GetTempFileName() + ".failsafelog." +
					              DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss", CultureInfo.InvariantCulture) + ".txt";
					wr = new StreamWriter(tmpfile, true, Encoding.UTF8);
				}
				using (wr) {
					wr.WriteLine(message.ToString());
					wr.Flush();
				}
			}
			catch (Exception ex) {
				throw new LogException("even safe mode fails to be executed", ex);
			}
		}

		/// <summary>
		/// 	returns UserLog listener, applyable for given context
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="host"> </param>
		/// <returns> </returns>
		public IUserLog GetLog(string context, object host) {
			lock (Sync) {
				var usableloggers = Loggers.Where(x => x.IsApplyable(context));
				if (usableloggers.Any()) {
					return new LoggerBasedUserLog(usableloggers.ToArray(), this, (context ?? "NONAME")) {HostObject = host};
				}
				else {
					return new StubUserLog {HostObject = host};
				}
			}
		}

		/// <summary>
		/// 	Synchronizes all internal loggers - calling code assumed here that all files/streams are now free
		/// 	use it for direct access for logging files and so on
		/// </summary>
		public void Join() {
			foreach (var logger in Loggers) {
				logger.Join();
			}
		}


		/// <summary>
		/// 	called on object after creation in IoC with current component context
		/// 	object can perform container bound logic here
		/// </summary>
		/// <param name="container"> The container. </param>
		/// <param name="component"> The component. </param>
		/// <remarks>
		/// </remarks>
		public override void SetContainerContext(IContainer container, IComponentDefinition component) {
			base.SetContainerContext(container, component);
			container.RegisterExtension(new LogManagerContainerExtension(this));
		}

		#region Nested type: LogManagerContainerExtension

		private class LogManagerContainerExtension : IContainerExtension {
			public LogManagerContainerExtension(DefaultLogManager target) {
				manager = target;
			}


			public IContainer Container { get; set; }

			public ContainerOperation SupportedOperations {
				get { return ContainerOperation.AfterRegisterComponent; }
			}

			public void Process(ContainerContext context) {
				if (context.Component.ServiceType == typeof (ILogger)) {
					if (context.Component.Lifestyle == Lifestyle.Extension && null != context.Component.Implementation) {
						manager.Loggers.Add((ILogger) context.Component.Implementation);
					}
					else if (!string.IsNullOrEmpty(context.Component.Name)) {
						manager.Loggers.Add(Container.Get<ILogger>(context.Component.Name));
					}
				}
			}

			public int Order { get; private set; }
			private readonly DefaultLogManager manager;
		}

		#endregion

		private IList<ILogger> _loggers;
	}
}