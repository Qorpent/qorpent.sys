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
// PROJECT ORIGIN: Qorpent.Core/SimpleContainer.cs
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Qorpent.IoC {
	/// <summary>
	/// 	very basic and simple container implementation, sealed is not valid base for real-world containers
	/// 	but can be used in very simple scenarios,
	/// 	can resolve types as transients with configured from 'components',
	/// 	can use BeforeCreate, AfterCreate, BeforeGet,AfterGet, BeforeAll,AfterAll extensions to override this core behavior
	/// 	don't support valid component's registy
	/// 	don't support valid name resolution
	/// 	so this container can be used only in basic scenario - Type->Impl
	/// 	GetAll() works only from extensions
	/// 	GetComponents() always return empty
	/// 	Unregister(component) can remove type mapping
	/// 	thread safe
	/// </summary>
	public sealed class SimpleContainer : IContainer {
		/// <summary>
		/// Приоритет при разрешении типов
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		/// 	Returns single object of given type (generic) - NAME IGNORED IN SIMPLE CONTAINER (EXTENSION STILL CAN USE)
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public T Get<T>(string name = null, params object[] ctorArgs) where T : class {
			lock (this) {
				return (T) Get(typeof (T), name);
			}
		}

		/// <summary>
		/// 	Returns single object of given type (native) - NAME IGNORED IN SIMPLE CONTAINER  (EXTENSION STILL CAN USE)
		/// </summary>
		/// <param name="type"> type to be returned </param>
		/// <param name="name"> </param>
		/// <param name="ctorArgs"> </param>
		/// <returns> </returns>
		public object Get(Type type, string name = null, params object[] ctorArgs) {
			lock (this) {
				var context = new ContainerContext
					{Operation = ContainerOperation.BeforeGet, RequestedType = type, RequestedName = name};
				Process(context);
				object o1;
				if (PrepareResolvedObject(context, out o1)) {
					return o1;
				}
				if (!_typemap.ContainsKey(type)) {
					if (ThrowErrorOnNotExistedComponent) {
						throw new ContainerException("cannot find service type " + type.Name + " in map");
					}

					if (type.IsValueType) {
						context.Object = Activator.CreateInstance(type);
					}

					context.Operation = ContainerOperation.AfterGet;
					Process(context);
					return context.Object;
				}

				context.ResolvedType = _typemap[type];
				if (PrepareResolvedObject(context, out o1)) {
					return o1;
				}
				return null;
			}
		}


		/// <summary>
		/// 	Returns all objects of given type (generic)  WORKS ONLY FROM EXTENSION
		/// </summary>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		public IEnumerable<T> All<T>(string name = null, params object[] ctorArgs) where T : class {
			lock (this) {
				return All(typeof (T), name).OfType<T>();
			}
		}

		/// <summary>
		/// 	Returns all objects of given type (native) WORKS ONLY FROM EXTENSION
		/// </summary>
		/// <param name="type"> type to be returned </param>
		/// <param name="name"> </param>
		/// <param name="ctorArguments"> </param>
		/// <returns> </returns>
		public IEnumerable All(Type type, string name = null, params object[] ctorArguments) {
			var context = new ContainerContext
				{Operation = ContainerOperation.BeforeAll, RequestedType = type, RequestedName = name};
			Process(context);
			if (null != context.Object) {
				return (IEnumerable) context.Object;
			}
			return Enumerable.Empty<object>();
		}

		/// <summary>
		/// 	empty method - do nothing
		/// </summary>
		/// <param name="obj"> </param>
		public void Release(object obj) {}

		/// <summary>
		/// 	Register new component
		/// </summary>
		/// <param name="component"> </param>
		public void Register(IComponentDefinition component) {
			_typemap[component.ServiceType] = component.ImplementationType;
		}

		/// <summary>
		/// 	Unregesters component
		/// </summary>
		/// <param name="component"> </param>
		public void Unregister(IComponentDefinition component) {
			if (!_typemap.ContainsKey(component.ServiceType)) {
				return;
			}
			//do not unregister not component's types
			if (!(_typemap[component.ServiceType] == component.ImplementationType)) {
				return;
			}
			_typemap.Remove(component.ServiceType);
		}

		/// <summary>
		/// 	Call cleanup logic (pools, caches and so on to free memory) - not clean singletons and components
		/// </summary>
		public void CleanUp() {}

		/// <summary>
		/// 	Get all registered components
		/// </summary>
		/// <returns> </returns>
		public IEnumerable<IComponentDefinition> GetComponents() {
			yield break;
		}

		/// <summary>
		/// 	True - behaior on Get if no component found will be Exception (default is return null)
		/// </summary>
		public bool ThrowErrorOnNotExistedComponent { get; set; }

		/// <summary>
		/// 	loader for simple container is STUB - no real work will be performed with this loader
		/// </summary>
		/// <returns> </returns>
		public IContainerLoader GetLoader() {
			return new EmptyLoader();
		}

		/// <summary>
		/// 	registers extension for this container
		/// </summary>
		/// <param name="extension"> </param>
		public void RegisterExtension(IContainerExtension extension) {
			if (!_extensions.Contains(extension)) {
				extension.Container = this;
				_extensions.Add(extension);
			}
		}

		/// <summary>
		/// 	unregisters extension from this container
		/// </summary>
		/// <param name="extension"> </param>
		public void UnRegisterExtension(IContainerExtension extension) {
			_extensions.Remove(extension);
			extension.Container = null;
		}

		/// <summary>
		/// Регистрирует дочерний резольвер типов, может использоваться для объединения нескольких IOC
		/// </summary>
		/// <param name="resolver"></param>
		public void RegisterSubResolver(ITypeResolver resolver) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	returns array of all configured extensions
		/// </summary>
		/// <returns> </returns>
		public IEnumerable<IContainerExtension> GetExtensions() {
			return _extensions.ToArray();
		}

		/// <summary>
		/// 	returns empty component definition to manual setup
		/// </summary>
		/// <returns> </returns>
		public IComponentDefinition EmptyComponent() {
			return new BasicComponentDefinition();
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
		                                                                    TImplementation implementation =
			                                                                    default(TImplementation)) where TService : class
			where TImplementation : class, TService, new() {
			var result = EmptyComponent();
			result.ServiceType = typeof (TService);
			result.ImplementationType = typeof (TImplementation);
			result.Lifestyle = lifestyle;
			result.Name = name;
			result.Priority = priority;
			result.Implementation = implementation;
			return result;
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
			var result = EmptyComponent();
			result.ServiceType = typeof (TService);
			result.ImplementationType = implementation.GetType();
			result.Lifestyle = Lifestyle.Extension;
			result.Name = name;
			result.Priority = priority;
			result.Implementation = implementation;
			return result;
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
			throw new NotImplementedException();
		}


		private bool PrepareResolvedObject(ContainerContext context, out object o1) {
			o1 = null;
			if (null != context.ResolvedType) {
				context.Operation = ContainerOperation.BeforeCreate;
				Process(context);
				if (null == context.Object) {
					context.Object = Activator.CreateInstance(context.ResolvedType);
				}
				if (context.Object != null) {
					context.Operation = ContainerOperation.AfterCreate;
					Process(context);
				}
			}
			//if Object fully created from facility - we do not call any creation 
			if (null != context.Object) {
				context.Operation = ContainerOperation.BeforeActivate;
				Process(context);
				context.Operation = ContainerOperation.AfterGet;
				{
					o1 = context.Object;
					return true;
				}
			}
			return false;
		}

		private void Process(ContainerContext context) {
			foreach (var containerExtension in InternalGetExtensions(context.Operation)) {
				containerExtension.Process(context);
			}
		}

		private IEnumerable<IContainerExtension> InternalGetExtensions(ContainerOperation operation) {
			return _extensions.Where(x => 0 != (x.SupportedOperations & operation)).OrderBy(x => x.Order);
		}

		#region Nested type: EmptyLoader

		private class EmptyLoader : IContainerLoader {
			public IEnumerable<IComponentDefinition> LoadDefaultManifest(bool allowErrors) {
				yield break;
			}

			public IEnumerable<IComponentDefinition> LoadManifest(XElement manifest, bool allowErrors) {
				yield break;
			}

			public IEnumerable<IComponentDefinition> LoadAssembly(Assembly assembly, bool requreManifest = false) {
				yield break;
			}


			public XElement ReadDefaultManifest() {
				throw new NotImplementedException();
			}

			public IEnumerable<IComponentDefinition> LoadType(Type type) {
				throw new NotImplementedException();
			}

			public IEnumerable<IComponentDefinition> Load<T>() {
				throw new NotImplementedException();
			}
		}

		#endregion

		private readonly IList<IContainerExtension> _extensions = new List<IContainerExtension>();
		private readonly IDictionary<Type, Type> _typemap = new Dictionary<Type, Type>();
	}
}