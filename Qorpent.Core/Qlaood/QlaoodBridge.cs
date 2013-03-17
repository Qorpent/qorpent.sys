#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Core/QlaoodBridge.cs
#endregion
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Qlaood bridge
	/// </summary>
	public static class QlaoodBridge {
		private static readonly object Sync = new object();
		private static IAssemblyLoader _assemblyLoader;
		private static bool _assemblyLoaderChecked;
		private static ApplicationQlaoodMode _qlaoodMode = ApplicationQlaoodMode.Undefined;
		private static IQlaoodHost _qlaoodHost;

		static QlaoodBridge() {
			AppDomain.CurrentDomain.AssemblyResolve +=
				(sender, args) => ResolveAssembly(args.Name, AssemblyLoaderBehavior.Default);
		}

		/// <summary>
		/// 	Qlaood mode of current domain
		/// </summary>
		public static ApplicationQlaoodMode QlaoodMode {
			get {
				lock (Sync) {
					if (_qlaoodMode == ApplicationQlaoodMode.Undefined) {
						_qlaoodMode = AutoDetectQlaoodMode();
					}
					return _qlaoodMode;
				}
			}
			set {
				lock (Sync) {
					_qlaoodMode = value;
				}
			}
		}

		/// <summary>
		/// Access to configuration
		/// </summary>
		public static QlaoodConfigurationWrapper Configuration { get; set; }

		/// <summary>
		/// 	Access to AssemblyLoader
		/// </summary>
		private static IAssemblyLoader AssemblyLoader {
			get {
				lock (Sync) {
					if (!_assemblyLoaderChecked) {
						_assemblyLoader = GetDefaultAssemblyLoader();
						_assemblyLoaderChecked = true;
					}
					return _assemblyLoader;
				}
			}

			set {
				lock (Sync) {
					_assemblyLoader = value;
				}
			}
		}

		/// <summary>
		/// 	Access to QlaoodHost in master domain
		/// </summary>
		public static IQlaoodHost QlaoodHost {
			get {
				lock (Sync) {
					return _qlaoodHost;
				}
			}
			set {
				lock (Sync) {
					_qlaoodHost = value;
					if (null != _qlaoodHost) {
						QlaoodMode = ApplicationQlaoodMode.Hosted;
					}
				}
			}
		}

		private static ApplicationQlaoodMode AutoDetectQlaoodMode() {
			if (IsQlaoodCoreAvailable()) {
				return ApplicationQlaoodMode.Available;
			}
			return ApplicationQlaoodMode.None;
		}

		private static bool IsQlaoodCoreAvailable() {
			if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name.Contains("Qorpent.Qlaood.Core"))) {
				{
					return true;
				}
			}
			if (File.Exists(Path.Combine(EnvironmentInfo.BinDirectory, "Qorpent.Qlaood.Core.dll"))) {
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 	Resolves assembly if Qlaood assembly loader is setted up
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="options"> </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		public static Assembly ResolveAssembly(string name, AssemblyLoaderBehavior options) {
			lock (Sync) {
				if (QlaoodMode == ApplicationQlaoodMode.Hosted) {
					return ResolveAssemblyInHostedMode(name, options);
				}
				else {
					return ResolveAssemblyInNonHostedMode(name, options);
				}
			}
		}

		private static Assembly ResolveAssemblyInHostedMode(string name, AssemblyLoaderBehavior options) {
			if (null == AssemblyLoader) {
				return LoadAssemblyFromHost(name, options);
			}
			try {
				return AssemblyLoader.GetAssembly(name, options);
			}
			catch {
				return LoadAssemblyFromHost(name, options);
			}
		}

		private static Assembly LoadAssemblyFromHost(string name, AssemblyLoaderBehavior options) {
			var data = QlaoodHost.AssemblyLoader.GetAssemblyData(name, options);
			if (null == data.Assembly) {
				if (options.HasFlag(AssemblyLoaderBehavior.ErrorOnNotFound)) {
					throw new QlaoodException("Assembly loader not defined");
				}
				return null;
			}
			if (null == data.SymbolInfo) {
				return Assembly.Load(data.Assembly);
			}
			return Assembly.Load(data.Assembly, data.SymbolInfo);
		}

		private static Assembly ResolveAssemblyInNonHostedMode(string name, AssemblyLoaderBehavior options) {
			if (null == AssemblyLoader) {
				if (options.HasFlag(AssemblyLoaderBehavior.ErrorOnNotFound)) {
					throw new QlaoodException("Assembly loader not defined");
				}
				return null;
			}
			return AssemblyLoader.GetAssembly(name, options);
		}

		private static IAssemblyLoader GetDefaultAssemblyLoader() {
			if (IsQlaoodCoreAvailable()) {
				var type = Type.GetType("Qorpent.Qlaood.AssemblyLoader, Qorpent.Qlaood.Core", true);
				var result = (IAssemblyLoader) Activator.CreateInstance(type);
				return result;
			}
			return null;
		}

		/// <summary>
		/// Creates hosted and setup it
		/// </summary>
		/// <returns></returns>
		public static AppDomain CreateHostedDomain(string name, IQlaoodHost host, string root = null) {
			if (string.IsNullOrWhiteSpace(root)) {
				root = EnvironmentInfo.BinDirectory;
			}
			
			var setup = new AppDomainSetup {ApplicationBase = root};
			var domain = AppDomain.CreateDomain(name, new Evidence(), setup);
			var interop = (QlaoodBridgeInterop) 
				domain.CreateInstanceAndUnwrap("Qorpent.Core", 
				typeof (QlaoodBridgeInterop).FullName);

			host.SetInterop(interop);
			interop.SetHost(host);

			return domain;
		}

		static byte[] ReloadAssembly(AssemblyName a) {
			var ass = Assembly.Load(a);
			return File.ReadAllBytes(ass.GetName().CodeBase.Replace("file:///", ""));
		}
	}
}