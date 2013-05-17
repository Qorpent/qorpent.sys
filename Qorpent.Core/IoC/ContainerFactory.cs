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
// PROJECT ORIGIN: Qorpent.Core/ContainerFactory.cs
#endregion
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.Log;
using Qorpent.Wiki;

namespace Qorpent.IoC {
	///<summary>
	///	Статическая фабрика создания контейнеров IoC. Поддерживает формирование пустого контенера сконфигурированного класса,
	///	а также загрузку сервисов по умолчанию. Кроме этого предоставляется сервис загрузки компонентов из сборок при помощи
	///	атрибута <see cref="ContainerComponentAttribute" />
	///</summary>
	///<remarks>
	///	В прикладной разработке <see cref="ContainerFactory" /> используется для дозагрзки собственных библиотек в контейнер, 
	///	также он может использоваться для создания суб-приложений и встроенных автономных решений, DSL, для формирования 
	///	среды для тестирования
	///</remarks>
	///<source>Qorpent/Qorpent.Core/IoC/ContainerFactory.cs</source>
	public static class ContainerFactory {
#if PARANOID
		static ContainerFactory() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// </summary>
		private static WellKnownService[] _registry;

		/// <summary>
		/// </summary>
		private static readonly object Sync = new object();

		/// <summary>
		/// 	Gets the registry.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public static WellKnownService[] WellKnownRegistry {
			get {
				if (null == _registry) {
					lock (Sync) {
						_registry =
							new WellKnownService[]
								{
									//CORE SERVICES
									new WellKnownService<IContainer>("Qorpent.IoC.Container, Qorpent.IoC", typeof (SimpleContainer),
									                                 Lifestyle.Singleton),
									new WellKnownService<ILogManager>(null, typeof (DefaultLogManager), Lifestyle.Singleton),
									new WellKnownService<IEventManager>(null, typeof (EventManager), Lifestyle.Singleton),
									new WellKnownService<Assembly>("Qorpent.Core"),
									new WellKnownService<Assembly>("Qorpent.Data"),
									new WellKnownService<Assembly>("Qorpent.Log"),
									new WellKnownService<Assembly>("Qorpent.IO"),
									new WellKnownService<Assembly>("Qorpent.Serialization"),
									new WellKnownService<Assembly>("Qorpent.Bxl"),
									new WellKnownService<Assembly>("Qorpent.Dsl"),
									new WellKnownService<Assembly>("Qorpent.Mvc"),
									new WellKnownService<Assembly>("Qorpent.Security")
								};
					}
				}
				return _registry;
			}
		}

		/// <summary>
		/// 	Создает контейнер и конфигурирует его сервисами по умолчанию, 
		/// 	сервисами из файлов манифеста и вызывает все <see cref="IContainerSetup" /> расширения.
		/// </summary>
		/// <param name="throwerrors"> if set to <c>true</c> [throwerrors]. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static IContainer CreateDefault(bool throwerrors = true) {
			var result = CreateEmpty();
			InitializeContainer(result, throwerrors);
			return result;
		}

		/// <summary>
		/// 	Конфигурирует указанный контейнер сервисами по умолчанию, 
		/// 	сервисами из файлов манифеста и вызывает все <see cref="IContainerSetup" /> расширения.
		/// </summary>
		/// <param name="result"> </param>
		/// <param name="throwerrors"> если <c>true</c> то генерирует ошибки. </param>
		public static void InitializeContainer(this IContainer result, bool throwerrors = true) {
			SetupWellKnownContainerServices(result);

			result.GetLoader().LoadDefaultManifest(throwerrors);

			ApplyContainerSetupers(result);
		}

		/// <summary>
		/// 	Создает пустой контейнер, настроенного в системе типа.
		/// </summary>
		/// <returns> </returns>
		public static IContainer CreateEmpty() {
			var containertype = GetContainerType();
			var result = (IContainer) Activator.CreateInstance(containertype);
			return result;
		}

		/// <summary>
		/// 	executes all IContainerSetup(s)
		/// </summary>
		/// <param name="result"> </param>
		/// <exception cref="NotImplementedException"></exception>
		private static void ApplyContainerSetupers(IContainer result) {
			foreach (var setuper in result.All<IContainerSetup>().ToArray()) {
				setuper.Setup(result);
			}
		}

		/// <summary>
		/// 	Возвращает сконфигурированный тип контейнера
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static Type GetContainerType() {
			var configuredType = ConfigurationManager.AppSettings.Get(QorpentConst.Config.IocContainerTypeAppSetting);
#if !SQL2008
			if (!String.IsNullOrWhiteSpace(configuredType)) {
#else
			if (!string.IsNullOrEmpty(configuredType))
			{
#endif
				try {
					return Type.GetType(configuredType);
				}
				catch (Exception e) {
					throw new ContainerException("cannot load configured type :" + configuredType, e);
				}
			}
			var result = Type.GetType(QorpentConst.DefaultIocContainerTypeName, false);
			if (null == result) {
				return typeof (SimpleContainer);
			}
			return result;
		}

		/// <summary>
		/// 	DUMP all cantainer component info to file
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="filename"> </param>
		public static void DumpContainer(IContainer container, string filename = "./container.dump") {
			var file = Path.GetFullPath(filename);
			var dir = Path.GetDirectoryName(file);
			Debug.Assert(!string.IsNullOrWhiteSpace(dir), "dir != null");
			Directory.CreateDirectory(dir);
			using (var s = new StreamWriter(file)) {
				foreach (var componentDefinition in container.GetComponents()) {
					s.Write(componentDefinition);
					s.WriteLine();
				}
				s.Flush();
			}
		}

		/// <summary>
		/// 	Загружает все сконфигурированные при помощи <see cref="ContainerComponentAttribute" /> компоненты из сборки в указанный контейнер
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="assembly"> </param>
		public static void RegisterAssembly(this IContainer container, Assembly assembly) {
			var defaultAttribute =
				assembly.GetCustomAttributes(typeof (ContainerAttribute), false).OfType<ContainerAttribute>().FirstOrDefault();
			defaultAttribute = defaultAttribute ?? new ContainerComponentAttribute(Lifestyle.Transient);
			var components = from t in assembly.GetTypes()
			                 where !t.IsAbstract
			                 let a =
				                 t.GetCustomAttributes(typeof (ContainerComponentAttribute), true).OfType
				                 <ContainerComponentAttribute>().ToArray()
			                 where 0 != a.Length
			                 select new {t, a};
			foreach (var c in components) {
				foreach (var a in c.a) {
					var component = container.EmptyComponent();
					if (c.t != null && c.t.BaseType != null) {
						var stype = (a.ServiceType ??
// ReSharper disable PossibleNullReferenceException
						             c.t.GetInterfaces().Except(c.t.BaseType.GetInterfaces()).FirstOrDefault(
							             x => x != typeof (IContainerBound))) ??
// ReSharper restore PossibleNullReferenceException
						            c.t;
						component.ServiceType = stype;
					}
					component.ImplementationType = c.t;
					component.Lifestyle = a.Lifestyle;
					if (a.Lifestyle == Lifestyle.Default) {
						component.Lifestyle = defaultAttribute.Lifestyle;
					}
					component.Name = a.Name;
					component.Priority = a.Priority;
					if (component.Priority == -1) {
						component.Priority = defaultAttribute.Priority;
					}
					if (component.Priority == -1) {
						component.Priority = 1000;
					}
					container.Register(component);
				}
			}
		}

		/// <summary>
		/// 	Resolves this instance.
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public static T ResolveWellKnown<T>(IApplication context = null) where T : class {
			var record = WellKnownRegistry.FirstOrDefault(x => x.ServiceType == typeof (T));
			if (null != record) {
				T result = null;
				try {
					var wktype = record.ResolvedWellKnownType;
					if (null != wktype) {
						result = Activator.CreateInstance(wktype) as T;
					}
				}
// ReSharper disable EmptyGeneralCatchClause
				catch (Exception) {}
// ReSharper restore EmptyGeneralCatchClause
				if (null == result) {
					if (null != record.DefaultType) {
						result = Activator.CreateInstance(record.DefaultType) as T;
					}
				}
				if (result is IApplicationBound && null != context) {
					((IApplicationBound) result).SetApplication(context);
				}
				return result;
			}
			return null;
		}

		/// <summary>
		/// 	Setups the well known container services.
		/// </summary>
		/// <param name="result"> The result. </param>
		/// <remarks>
		/// 	applys only if container don't contains same typed services
		/// </remarks>
		public static void SetupWellKnownContainerServices(IContainer result) {
			foreach (var c in (
				                  from wn in WellKnownRegistry
				                  join cc in result.GetComponents()
					                  on wn.ServiceType.FullName + (wn.Name ?? "") equals
					                  cc.ServiceType.FullName + (cc.Name ?? "")
					                  into components
				                  where
					                  typeof (Assembly) != wn.ServiceType &&
					                  typeof (IContainer) != wn.ServiceType &&
					                  null != wn.ResolvedWellKnownType && !components.Any()
				                  select new WellKnownAttachedComponentDefinition(wn))
				) {
				result.Register(c);
			}

			foreach (var plugAssembly in WellKnownRegistry.Where(x => x.ServiceType == typeof (Assembly))) {
				Assembly assembly = null;
				try {
					assembly = Assembly.Load(plugAssembly.WellKnownTypeName);
				}
// ReSharper disable EmptyGeneralCatchClause
				catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
					//No means NO
				}
				if (null != assembly) {
					RegisterAssembly(result, assembly);
				}
			}
		}
	}
}