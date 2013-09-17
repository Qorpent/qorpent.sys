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
// PROJECT ORIGIN: Qorpent.IoC/Container.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.Log;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

[assembly:
	InternalsVisibleTo(
		"Qorpent.IoC.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010073ab8db929e2266f7a0d902dab115fb18e69b929d8b51cb07c3b2b0f89e5878ff85356fd0c4ac30edeb4e8e2f3c5dd6991155268ac2aab61b0b9dc4fe95b5b58ff567e1df74fd4ab9431baf86c08f37c19192c7d156755d87f933680026d456180e616fecbd31dbaaef6cc14ff30cc7aab46a6e4b5fbb8f0ddf41c7e3b8d4ab8"
		)]
//need reset InternalsVisible attribute due to Resharper limitations

namespace Qorpent.IoC {
	/// <summary>
	/// 	Основная реализация контейна классов и зависимостей
	/// </summary>
	/// <isdefaultimpl cref="IContainer" />
	/// <remarks>
	/// </remarks>
	[RequireReset(Role = "DEVELOPER", All = false, Options = new[] {QorpentConst.ContainerResetCode})]
	public class Container : IContainer, IApplicationBound, IResetable {
#if PARANOID
		static Container() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	Журнал контейнера
		/// </summary>
		public IUserLog Log {
			get {
				lock (this) {
					if (null == _log) {
						var manager = Get<ILogManager>();
						_log = null != manager ? manager.GetLog(GetType().FullName + ";" + GetType().Assembly.GetName().Name, this) : new StubUserLog();
					}
					return _log;
				}
			}
		}


		/// <summary>
		/// 	Call service defined Application bound - setups reset event and so on
		/// </summary>
		/// <param name="app"> The app. </param>
		/// <remarks>
		/// </remarks>
		public void SetApplication(IApplication app) {
			if (null != Get<IEventManager>()) {
				Get<IEventManager>().Add(new StandardResetHandler(this));
			}
		}


		/// <summary>
		/// 	Возвращает сервис указанного типа. (обобщенный)
		/// </summary>
		/// <typeparam name="T"> Тип сервиса </typeparam>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <returns> Сервис указанного типа или <c>null</c> , если подходящий компонент не обнаружен </returns>
		/// <exception cref="ContainerException">При ошибках в конструировании объектов</exception>
		public T Get<T>(string name = null, params object[] ctorArguments) where T : class {
			lock (this) {
				return (T) Get(typeof (T), name, ctorArguments);
			}
		}

		/// <summary>
		/// Приоритет при разрешении типов
		/// </summary>
		public int Idx { get; set; }
		/// <summary>
		/// 	Возвращает сервис указанного типа. (прямое указание типа)
		/// </summary>
		/// <param name="type"> Тип сервиса </param>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <returns> Сервис указанного типа или <c>null</c> , если подходящий компонент не обнаружен </returns>
		/// <exception cref="ContainerException">При ошибках в конструировании объектов</exception>
		public object Get(Type type, string name = null, params object[] ctorArguments) {
			lock (this) {
				type = type ?? typeof (object);
				if (type.IsValueType) {
					throw new ArgumentException("type must be interface or reference type");
				}

				var preresolved = PreResolveWithSubResolvers(type, name, ctorArguments);
				if(null!=preresolved) return preresolved;


				var component = FindComponent(type, name);
				if (null == component) {
					var postresolved = PostResolveWithSubResolvers(type, name, (object[]) ctorArguments);
					if(postresolved!=null) {
						return postresolved;
					}
					if (ThrowErrorOnNotExistedComponent) {
						throw new ContainerException(string.Format("cannot find component for {0} {1}", type, name));
					}
					return null;
				}
				component.ActivationCount++;
				object result = null;
				switch (component.Lifestyle) {
					case Lifestyle.Default:
						goto case Lifestyle.Transient;
					case Lifestyle.Extension:
						if (null != component.Implementation) {
							result = component.Implementation;
							break;
						}
						result = CreateInstance(component, ctorArguments);
						if (component.CacheInstanceOfExtension) {
							component.Implementation = result;
						}
						break;
					case Lifestyle.Transient:
						result = CreateInstance(component, ctorArguments);
						break;
					case Lifestyle.Pooled:
						if (Pool.ContainsKey(component) && Pool[component].Count != 0) {
							result = Pool[component].Pop();
						}
						else {
							result = CreateInstance(component, ctorArguments);
						}
						OutgoingCache[result] = component;
						break;
					case Lifestyle.PerThread:
						if (!ThreadOutgoingCache.ContainsKey(Thread.CurrentThread)) {
							ThreadOutgoingCache[Thread.CurrentThread] = new Dictionary<object, IComponentDefinition>();
						}
						if (ThreadOutgoingCache[Thread.CurrentThread].ContainsValue(component)) {
							result = ThreadOutgoingCache[Thread.CurrentThread].First(x => Equals(x.Value, component)).Key;
						}
						else {
							result = CreateInstance(component, ctorArguments);
							ThreadOutgoingCache[Thread.CurrentThread][result] = component;
						}
						OutgoingCache[result] = component;
						break;
					case Lifestyle.Singleton:
						if (null == component.Implementation) {
							component.Implementation = CreateInstance(component, ctorArguments);
						}
						result = component.Implementation;
						if (!OutgoingCache.ContainsKey(result)) {
							OutgoingCache[result] = component;
						}
						break;
				}
				var context = new ContainerContext
					{
						Component = component,
						Operation = ContainerOperation.AfterActivate,
						Object = result,
					};
				ProcessExtensions(context);
				return result;
			}
		}

		private object PreResolveWithSubResolvers(Type type, string name, object[] ctorArguments) {
			return SubResolvers.Where(x => x.Idx <= this.Idx).OrderBy(x => x.Idx).Select(preresolver => preresolver.Get(type, name, (object[]) ctorArguments)).FirstOrDefault(resolved => null != resolved);
		}
		private object PostResolveWithSubResolvers(Type type, string name, object[] ctorArguments)
		{
			return SubResolvers.Where(x => x.Idx > this.Idx).OrderBy(x => x.Idx).Select(preresolver => preresolver.Get(type, name, (object[])ctorArguments)).FirstOrDefault(resolved => null != resolved);
		}

		/// <summary>
		/// 	returns empty component definition to manual setup
		/// </summary>
		/// <returns> </returns>
		public IComponentDefinition EmptyComponent() {
			return new ComponentDefinition();
		}

		/// <summary>
		/// 	Creates (but not register) new component definition
		/// </summary>
		/// <param name="lifestyle"> </param>
		/// <param name="name"> </param>
		/// <param name="priority"> </param>
		/// <param name="implementation"> </param>
		/// <typeparam name="TService"> </typeparam>
		/// <typeparam name="TImplementation"> </typeparam>
		/// <returns> </returns>
		public IComponentDefinition NewComponent<TService, TImplementation>(Lifestyle lifestyle = Lifestyle.Transient,
		                                                                    string name = "", int priority = 10000,
		                                                                    TImplementation implementation = null)
			where TService : class where TImplementation : class, TService, new() {
			if (lifestyle == Lifestyle.Transient && null != implementation) {
				lifestyle = Lifestyle.Extension;
			}
			return new ComponentDefinition<TService, TImplementation>(lifestyle, name, priority, implementation);
		}

		/// <summary>
		/// 	Creates (but not register) new component definition
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="priority"> </param>
		/// <param name="implementation"> </param>
		/// <typeparam name="TService"> </typeparam>
		/// <returns> </returns>
		public IComponentDefinition NewExtension<TService>(TService implementation, string name = "", int priority = 10000)
			where TService : class {
			var result = new ComponentDefinition
				{
					ServiceType = typeof (TService),
					ImplementationType = implementation.GetType(),
					Implementation = implementation,
					Lifestyle = Lifestyle.Extension,
					Priority = priority,
					Name = name
				};
			return result;
		}

		/// <summary>
		/// 	True - behaior on Get if no component found will be Exception (default is return null)
		/// </summary>
		/// <value> <c>true</c> if [throw error on not existed component]; otherwise, <c>false</c> . </value>
		/// <remarks>
		/// </remarks>
		public bool ThrowErrorOnNotExistedComponent { get; set; }

		/// <summary>
		/// 	retrive apropriate version of ContainerLoader (can be difference for different containers)
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IContainerLoader GetLoader() {
			return new ContainerLoader(this);
		}

		/// <summary>
		/// 	registers extension for this container
		/// </summary>
		/// <param name="extension"> The extension. </param>
		/// <remarks>
		/// </remarks>
		public void RegisterExtension(IContainerExtension extension) {
			lock (this) {
				if (!_extensions.Contains(extension)) {
					_extensions.Add(extension);
					extension.Container = this;
				}
			}
		}

		/// <summary>
		/// 	unregisters extension from this container
		/// </summary>
		/// <param name="extension"> The extension. </param>
		/// <remarks>
		/// </remarks>
		public void UnRegisterExtension(IContainerExtension extension) {
			lock (this) {
				_extensions.Remove(extension);
			}
		}

		private IList<ITypeResolver> SubResolvers = new List<ITypeResolver>();
		/// <summary>
		/// Регистрирует дочерний резольвер типов, может использоваться для объединения нескольких IOC
		/// </summary>
		/// <param name="resolver"></param>
		public void RegisterSubResolver(ITypeResolver resolver) {
			SubResolvers.Add(resolver);
		}

		/// <summary>
		/// 	returns array of all configured extensions
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public IEnumerable<IContainerExtension> GetExtensions() {
			lock (this) {
				return _extensions.ToArray();
			}
		}


		/// <summary>
		/// 	Возвращает все объекты указаннго типа (обобщенииый)
		/// </summary>
		/// <typeparam name="T"> тип сервиса </typeparam>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <returns> Все экземпляры указанного сервиса </returns>
		/// <remarks>
		/// 	<invariant>Метод All применим только для компонентов с циклом жизни
		/// 		<see cref="Lifestyle.Transient" />
		/// 		и
		/// 		<see cref="Lifestyle.Extension" />
		/// 		.
		/// 		<note>Не пытайтесь таким образом получить все экземпляры сервисов с другим циклом жизни</note>
		/// 	</invariant>
		/// </remarks>
		public IEnumerable<T> All<T>(string name = null, params object[] ctorArguments) where T : class {
			lock (this) {
				return All(typeof (T), name, ctorArguments).OfType<T>();
			}
		}


		/// <summary>
		/// 	Возвращает все объекты указаннго типа (прямое указание типа)
		/// </summary>
		/// <param name="ctorArguments"> Параметры для вызова конструктора, если не указано - будет использован конструктор по умолчанию. </param>
		/// <param name="type"> тип сервиса </param>
		/// <param name="name"> опциональное имя компонента, если указано - поиск будет производиться только среди с компонентов с указаным именем </param>
		/// <returns> Все экземпляры указанного сервиса </returns>
		/// <remarks>
		/// 	<invariant>Метод All применим только для компонентов с циклом жизни
		/// 		<see cref="Lifestyle.Transient" />
		/// 		и
		/// 		<see cref="Lifestyle.Extension" />
		/// 		.
		/// 		<note>Не пытайтесь таким образом получить все экземпляры сервисов с другим циклом жизни</note>
		/// 	</invariant>
		/// </remarks>
		public IEnumerable All(Type type, string name = null, params object[] ctorArguments) {
			lock (this) {
				if (null == type) {
					throw new ArgumentNullException("type");
				}
				if (type.IsValueType) {
					throw new ArgumentException("type must be interface or reference type");
				}

				foreach (var typeResolver in SubResolvers.Where(x=>x.Idx<=this.Idx).OrderBy(x=>x.Idx)) {
					foreach (var obj in typeResolver.All(type,name,(object[])ctorArguments)) {
						yield return obj;
					}
				}

				var extensions = FindExtensions(type, name);
				if (typeof (IWithIndex).IsAssignableFrom(type)) {
//have to order result
					var result = new List<object>();
					foreach (var component in extensions) {
						component.ActivationCount++;
						result.Add(component.Implementation ?? CreateInstance(component, ctorArguments));
					}
					foreach (var obj in result.OfType<IWithIndex>().OrderBy(x => x.Index)) {
						yield return obj;
					}
				}
				else {
					foreach (var component in extensions) {
						component.ActivationCount++;
						if (component.Implementation != null) {
							yield return component.Implementation;
						}
						else {
							yield return CreateInstance(component, null);
						}
					}
				}


				foreach (var typeResolver in SubResolvers.Where(x => x.Idx > this.Idx).OrderBy(x => x.Idx))
				{
					foreach (var obj in typeResolver.All(type, name, (object[])ctorArguments))
					{
						yield return obj;
					}
				}
			}
		}


		/// <summary>
		/// 	Возвращает объект контейнеру
		/// </summary>
		/// <param name="obj"> Объект, ранее созданнй контейнером </param>
		/// <remarks>
		/// 	<invariant>Метод позволяет контейнеру высвобождать собственные ресурсы и вызывает
		/// 		<see cref="IContainerBound.OnContainerRelease" />
		/// 		метод,
		/// 		работает это только для
		/// 		<see cref="Lifestyle.Pooled" />
		/// 		и
		/// 		<see cref="Lifestyle.PerThread" />
		/// 		, для остальных данный метод игнорируется</invariant>
		/// </remarks>
		public void Release(object obj) {
			lock (this) {
				if (!OutgoingCache.ContainsKey(obj)) {
					return;
				}
				var component = OutgoingCache[obj];
				switch (component.Lifestyle) {
					case Lifestyle.Extension: //extensions not must be released
						goto case Lifestyle.Transient;
					case Lifestyle.Default: // default is transient
						goto case Lifestyle.Transient;
					case Lifestyle.Transient: //transient not have to be released
						return;
					case Lifestyle.Pooled:
						// pool ion release not disposed, but moved to pool
						if (!Pool.ContainsKey(component)) {
							Pool[component] = new Stack<object>();
						}
						Pool[component].Push(obj);
						break;
					case Lifestyle.PerThread:
						//per thread removed from thread cache and OnContainerRelease called
						//and it can be released only in calling thread
						if (!ThreadOutgoingCache.ContainsKey(Thread.CurrentThread)) {
							throw new ContainerException("try to realease thread object from not configured thread");
						}
						if (!ThreadOutgoingCache[Thread.CurrentThread].ContainsKey(obj)) {
							throw new ContainerException("try to release thread object on not it's thread");
						}
						ThreadOutgoingCache[Thread.CurrentThread].Remove(obj);
						DropObject(obj);
						break;

					case Lifestyle.Singleton:
						//singletons cannot be released - they will still alive
						return;
				}
				OutgoingCache.Remove(obj);
			}
		}

		/// <summary>
		/// 	Регистрирует новый компонент в контейнере
		/// </summary>
		public void Register(IComponentDefinition component) {
			lock (this) {
#if PARANOID
				if(component.ServiceType==typeof(IRoleResolver)) {
					if(component.ImplementationType!=typeof(DefaultRoleResolver)) {
						throw new ParanoidException(ParanoidState.InvalidRoleResolver);
					}
				}

				if (component.ServiceType == typeof(IPrincipalSource))
				{
					if (component.ImplementationType != typeof(DefaultPrincipalSource))
					{
						throw new ParanoidException(ParanoidState.InvalidPrincipalSource);
					}
				}
				#endif

				Log.Debug("Start register " + component, this);
				// redirect logic for container extensions - they are processed on their own manner
				// and container extensions don't trigger RegisterComponent event
				if (component.Lifestyle == Lifestyle.ContainerExtension) {
					if (null == _extensions.FirstOrDefault(x => x.GetType() == component.ImplementationType)) {
						RegisterExtension((IContainerExtension) Activator.CreateInstance(component.ImplementationType));
					}
					return;
				}
				// hook for extensions - RegisterComponent extensions can replace/update/Prepare component to be registered
				// it can be used only BEFORE native registration due to index integrity
				component =
					ProcessExtensions(new ContainerContext
						{Operation = ContainerOperation.BeforeRegisterComponent, Component = component}).Component;

				//no need double registration
				if (ComponentExists(component)) {
					return;
				}
				component.ContainerId = CurrentComponentId++;
				Components.Add(component);
				if (!ByTypeCache.ContainsKey(component.ServiceType)) {
					ByTypeCache[component.ServiceType] = new List<IComponentDefinition>();
				}
				ByTypeCache[component.ServiceType].Add(component);
				ByTypeCache[component.ServiceType].Sort(CompareComponents);
				if (component.Name.IsNotEmpty()) {
					if (!ByNameCache.ContainsKey(component.Name)) {
						ByNameCache[component.Name] = new List<IComponentDefinition>();
					}
					ByNameCache[component.Name].Add(component);
					ByNameCache[component.Name].Sort(CompareComponents);
				}

				ProcessExtensions(new ContainerContext
					{Operation = ContainerOperation.AfterRegisterComponent, Component = component});
				Log.Debug("Registered " + component, this);
			}
		}


		/// <summary>
		/// 	Отменяет регистрацию компонента
		/// </summary>
		/// <param name="component"> компонент, который должен быть убран из контейнера </param>
		/// <remarks>
		/// 	<note>Очевидно что, такой метод обязан присутствовать в интерфейсе контейнера, однако его использование в задачах помимо тестирования,
		/// 		обозначает недостатки архитектуры приложения, так как в нормальном варианте использования контейнер меняет свое поведение по принципу наращивания
		/// 		обслуживаемых классов и компонентов, тогда как удаление части компонент может привести к неожиданным эффектам в случае кэширования более
		/// 		ранеей выдвачи клиентской стороной</note>
		/// </remarks>
		public void Unregister(IComponentDefinition component) {
			lock (this) {
				//no need double registration
				if (Components.Contains(component)) {
					return;
				}
				Components.Remove(component);
				ByTypeCache[component.ServiceType].Remove(component);
				if (component.Name.IsNotEmpty()) {
					ByNameCache[component.Name].Remove(component);
				}
				foreach (var componentDefinition in OutgoingCache.ToArray()) {
					if (Equals(componentDefinition.Value, component)) {
						DropObject(componentDefinition.Key);
						OutgoingCache.Remove(componentDefinition.Key);
					}
				}
				if (Lifestyle.PerThread == component.Lifestyle) {
					foreach (var threadCache in ThreadOutgoingCache.ToArray()) {
						foreach (var componentDefinition in threadCache.Value.ToArray()) {
							if (Equals(componentDefinition.Value, component)) {
								DropObject(componentDefinition.Key);
								OutgoingCache.Remove(componentDefinition.Key);
							}
						}
					}
				}
				if (Lifestyle.Pooled == component.Lifestyle) {
					if (Pool.ContainsKey(component)) {
						foreach (var obj in Pool[component]) {
							DropObject(obj);
						}
						Pool.Remove(component);
					}
				}

				// hook for extensions - UnRegisterComponent extensions can do something after it's 
				// native unregistration
				ProcessExtensions(new ContainerContext {Operation = ContainerOperation.UnregisterComponent, Component = component});
			}
		}


		/// <summary>
		/// 	Вызыает очистку контейнера
		/// </summary>
		/// <remarks>
		/// 	<invariant>Очистка очищает внутрненнее состояние и кэши контейнера, регистрации компонент это не касается, компоненты остаются в прежней конфигурации</invariant>
		/// </remarks>
		public void CleanUp() {
			foreach (var objreg in OutgoingCache) {
				DropObject(objreg.Key);
			}
			foreach (var objreg in Pool) {
				foreach (var o in objreg.Value) {
					DropObject(o);
				}
			}
			OutgoingCache.Clear();
			Pool.Clear();
		}


		/// <summary>
		/// 	Получить все зарегистрированные компоненты
		/// </summary>
		/// <returns> Все компоненты контейнера </returns>
		public IEnumerable<IComponentDefinition> GetComponents() {
			return Components.ToArray();
		}


		/// <summary>
		/// 	Сбрасывает содержимое локальных кэшей, не модифицируется по параметрам события
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> </returns>
		public object Reset(ResetEventData data) {
			CleanUp();
			return null;
		}

		/// <summary>
		/// 	Возвращает размер кэшей - важно для сброса
		/// </summary>
		/// <returns> </returns>
		public object GetPreResetInfo() {
			return new {outcache = OutgoingCache.Count, poolsize = Pool.Count};
		}


		/// <summary>
		/// 	creates new instance of component's object, IContainerBound.SetContainerContext called
		/// </summary>
		/// <param name="component"> The component. </param>
		/// <param name="arguments"> аргументы для конструктора, если <c>null</c> , то будет использован конструктор по умолчанию </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected internal object CreateInstance(IComponentDefinition component, object[] arguments) {
			try {
				object result = Activator.CreateInstance(component.ImplementationType,
				                                         BindingFlags.Instance | BindingFlags.CreateInstance | BindingFlags.Public |
				                                         BindingFlags.NonPublic,
				                                         null,
				                                         arguments, CultureInfo.InvariantCulture);

				component.CreationCount++;
				ProcessAttributedInjections(result);
				ProcessParametersInjection(result, component);
				ProcessExplicitInjections(component, result);
				var context = new ContainerContext
					{
						Component = component,
						Operation = ContainerOperation.AfterCreate,
						Object = result,
					};
				ProcessExtensions(context);
				var o = context.Object as IContainerBound;
				if (o != null) {
					o.OnContainerCreateInstanceFinished();
				}
				return context.Object;
			}
			catch (ContainerException ex) {
				Log.Error("CreateInstance of" + component, ex);
				throw;
			}
			catch (Exception ex) {
				Log.Error("CreateInstance of" + component, ex);
				throw new ContainerException("CreateInstance " + component, ex);
			}
		}

		private void ProcessExplicitInjections(IComponentDefinition component, object result) {
			var ca = result as IContainerBound;
			if (null != ca) {
				ca.SetContainerContext(this, component);
			}
		}

		private void ProcessAttributedInjections(object result) {
			var injections = from p in result.GetType().FindAllValueMembers(typeof (InjectAttribute))
			                 let inja = p.Member.GetFirstAttribute<InjectAttribute>()
			                 select new {prop = p, type = p.Type, name = inja.Name, factoryType = inja.FactoryType, required =inja.Required};

			foreach (var i in injections) {
				var current = i.prop.Get(result);

				object val;
				if (i.type.IsArray) {
					var instances = All(i.type.GetElementType(), i.name).OfType<object>().ToArray();
					val = Array.CreateInstance(i.type.GetElementType(), instances.Length);
					Array.Copy(instances, val as Array, instances.Length);
				}
				else if (i.type.IsGenericType &&
				         ((i.type.GetGenericTypeDefinition() == typeof (List<object>).GetGenericTypeDefinition())
				          || ((i.type.GetGenericTypeDefinition() == typeof (IList<object>).GetGenericTypeDefinition())))
					) {
					var instances = All(i.type.GetGenericArguments()[0], i.name).OfType<object>().ToArray();

					if (null == current) {
						val =
							Activator.CreateInstance(
								typeof (List<object>).GetGenericTypeDefinition().MakeGenericType(i.type.GetGenericArguments()[0]));
					}
					else {
						val = current;
					}
					var addm = val.GetType().GetMethod("Add");
					foreach (var instance in instances) {
						addm.Invoke(val, new[] {instance});
					}
				}
				else if (!i.type.IsValueType) {
					//  if(null!=current)continue;
					val = Get(i.type, i.name);
				}
				else {
					throw new ContainerException("cannot inject property " + i.prop.Member.Name + " of type " + i.type);
				}
                if (!i.type.IsValueType && null == val) {
                    if (null != i.factoryType) {
                        var factory = (IFactory) Activator.CreateInstance(i.factoryType);
                        val = factory.Get(i.type);
                    }
                    if (null == val && i.required) {
                        throw new ContainerException("cannot inject required member "+i.name+" of type "+i.type);
                    }
                }
				i.prop.Set(result, val);
			}
		}

		private void ProcessParametersInjection(object result, IComponentDefinition component) {
			if (component.Parameters.IsNotEmpty()) {
				foreach (var parameter in component.Parameters) {
					//applys parameters in very save mode
					result.SetValue(parameter.Key, parameter.Value, ignoreNotFound: true, publicOnly: false,
					                ignoreTypeConversionError: true);
				}
			}
		}

		/// <summary>
		/// 	Find best matched component for type/name or null for
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="name"> The name. </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>
		/// </remarks>
		public IComponentDefinition FindComponent(Type type, string name) {
			type = type ?? typeof (object);
			var preresult = PreFindComponentWithSubResolvers(type, name);
			if(null!=preresult) {
				return preresult;
			}
			if (name.IsEmpty()) {
				//firstly try to direct match
				if (ByTypeCache.ContainsKey(type)) {
					return ByTypeCache[type].FirstOrDefault();
				}

				var key = ByTypeCache.Keys.FirstOrDefault(type.IsAssignableFrom);
				if (null == key) {
					return PostFindComponentWithSubResolvers(type,name);
				}
				return ByTypeCache[key].FirstOrDefault();
			}
			if (!ByNameCache.ContainsKey(name)) {
				return PostFindComponentWithSubResolvers(type,name);
			}
			return ByNameCache[name].FirstOrDefault(x => type.IsAssignableFrom(x.ServiceType));
		}

		private IComponentDefinition PreFindComponentWithSubResolvers(Type type, string name) {
			return SubResolvers.Where(x => x.Idx <= this.Idx).OrderBy(x => x.Idx).OfType<IComponentSource>().Select(csource => csource.FindComponent(type, name)).FirstOrDefault(subcomponent => null != subcomponent);
		}

		private IComponentDefinition PostFindComponentWithSubResolvers(Type type, string name)
		{
			return SubResolvers.Where(x => x.Idx > this.Idx).OrderBy(x => x.Idx).OfType<IComponentSource>().Select(csource => csource.FindComponent(type, name)).FirstOrDefault(subcomponent => null != subcomponent);
		}

		/// <summary>
		/// 	Find best matched extension (extension, transient, default - lifesycle) components for type/name
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="name"> The name. </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>
		/// </remarks>
		public IEnumerable<IComponentDefinition> FindExtensions(Type type, string name) {
			if (name.IsEmpty()) {
				if (!ByTypeCache.ContainsKey(type)) {
					return new IComponentDefinition[] {};
				}
				return
					ByTypeCache[type].Where(
						x =>
						((Lifestyle.Transient | Lifestyle.Extension | Lifestyle.Default) & x.Lifestyle) != 0).Reverse().ToArray();
			}
			//поддержка поиска по тегам
			if (name.StartsWith("tag:")) {
				var regex = new Regex(name.Substring(4));
				return Components.Where(
						x =>
						((Lifestyle.Transient | Lifestyle.Extension | Lifestyle.Default) & x.Lifestyle) != 0
						&&
						(null!=x.Tag && regex.IsMatch(x.Tag))
						).Reverse().ToArray();
			}
			else {
				if (!ByNameCache.ContainsKey(name))
				{
					return new IComponentDefinition[] { };
				}
				return ByNameCache[name].Where(
					x =>
					type.IsAssignableFrom(x.ServiceType) &&
					((Lifestyle.Transient | Lifestyle.Extension | Lifestyle.Default) & x.Lifestyle) != 0
					).Reverse().ToArray();	
			}

			
		}

		/// <summary>
		/// 	comparising method for components - find most preferable component in set
		/// </summary>
		/// <param name="a"> A. </param>
		/// <param name="b"> The b. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected internal int CompareComponents(IComponentDefinition a, IComponentDefinition b) {
			var result = a.Priority.CompareTo(b.Priority); // if priority not equal- less - first
			if (0 == result) {
				result = - a.ContainerId.CompareTo(b.ContainerId); //otherwise last (by containerId) is in priority
			}
			return result;
		}

		/// <summary>
		/// 	Проверяет наличие в реестре компонента по внутренней логике сравнения
		/// </summary>
		/// <param name="component"> </param>
		/// <returns> </returns>
		protected bool ComponentExists(IComponentDefinition component) {
			var existed = FindComponent(component.ServiceType, component.Name);
			if (null == existed) {
				return false;
			}
			if (component.ServiceType != existed.ServiceType) {
				return false; //был найден унаследованный тип
			}
			if (component.Name != existed.Name) {
				return false; //при поиске без  имени был найден именованный компонент
			}
			if (component.Lifestyle != existed.Lifestyle) {
				return false;
			}
			if (component.Priority != existed.Priority) {
				return false;
			}
			if (component.ImplementationType != existed.ImplementationType) {
				return false;
			}
			if (component.Implementation != existed.Implementation) {
				return false;
			}
			if (component.Parameters.Count != existed.Parameters.Count) {
				return false;
			}
			foreach (var p in component.Parameters) {
				if (!existed.Parameters.ContainsKey(p.Key)) {
					return false;
				}
				if (p.Value != existed.Parameters[p.Key]) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// 	Drops the object.
		/// </summary>
		/// <param name="obj"> The obj. </param>
		/// <remarks>
		/// </remarks>
		private void DropObject(object obj) {
			var cb = obj as IContainerBound;
			if (null != cb) {
				cb.OnContainerRelease();
			}
			var context = new ContainerContext
				{
					Component = OutgoingCache[obj],
					Operation = ContainerOperation.Release,
					Object = obj,
				};
			ProcessExtensions(context);
		}

		/// <summary>
		/// 	executes extensions for given context
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected ContainerContext ProcessExtensions(ContainerContext context) {
			foreach (var containerExtension in GetMatchedExtensions(context)) {
				containerExtension.Process(context);
			}
			return context;
		}

		/// <summary>
		/// 	retrieves extensions, bound to given context
		/// </summary>
		/// <param name="context"> The context. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected IEnumerable<IContainerExtension> GetMatchedExtensions(ContainerContext context) {
			return _extensions.Where(x => 0 != (x.SupportedOperations & context.Operation)).OrderBy(x => x.Order);
		}

		/// <summary>
		/// </summary>
		private readonly IList<IContainerExtension> _extensions = new List<IContainerExtension>();

		/// <summary>
		/// 	component-dict Name-&gt;Components
		/// </summary>
		protected internal Dictionary<string, List<IComponentDefinition>> ByNameCache =
			new Dictionary<string, List<IComponentDefinition>>();

		/// <summary>
		/// 	component-dict Type-&gt;Components
		/// </summary>
		protected internal Dictionary<Type, List<IComponentDefinition>> ByTypeCache =
			new Dictionary<Type, List<IComponentDefinition>>();

		/// <summary>
		/// 	plain component list
		/// </summary>
		protected internal List<IComponentDefinition> Components = new List<IComponentDefinition>();

		/// <summary>
		/// 	Внутренний счетчик идентификаторов компонентов
		/// </summary>
		protected internal int CurrentComponentId;

		/// <summary>
		/// 	cache of all exposed objects (except extensions/transients)
		/// </summary>
		protected internal Dictionary<object, IComponentDefinition> OutgoingCache =
			new Dictionary<object, IComponentDefinition>();

		/// <summary>
		/// 	cache for pooled components
		/// </summary>
		protected internal Dictionary<IComponentDefinition, Stack<object>> Pool =
			new Dictionary<IComponentDefinition, Stack<object>>();

		/// <summary>
		/// 	per-thread outgoing cache
		/// </summary>
		protected internal Dictionary<Thread, Dictionary<object, IComponentDefinition>> ThreadOutgoingCache =
			new Dictionary<Thread, Dictionary<object, IComponentDefinition>>();

		private IUserLog _log;
	}
}