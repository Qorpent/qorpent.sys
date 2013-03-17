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
// PROJECT ORIGIN: Qorpent.Core/ContainerObjectWrapper.cs
#endregion
using System;
using Qorpent.Applications;

namespace Qorpent.IoC {
	/// <summary>
	/// 	Disposable wrapper to use with IoC container (releases object on dispose)
	/// </summary>
	/// <typeparam name="T"> </typeparam>
	public class ContainerObjectWrapper<T> : IDisposable where T : class {
		/// <summary>
		/// 	Creates object wrapper from default container with no special options
		/// </summary>
		public ContainerObjectWrapper(string name = null) : this(Application.Current.Container, name) {}

		/// <summary>
		/// 	Creates object wrapper from given container with options
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="name"> </param>
		public ContainerObjectWrapper(IContainer container, string name = null) {
			Container = container;
			Name = name;
			Object = container.Get<T>(name);
		}

		/// <summary>
		/// 	Name of object
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// 	Object from container
		/// </summary>
		public T Object { get; protected set; }


		/// <summary>
		/// 	Frees object to container
		/// </summary>
		public void Dispose() {
			Dispose(true);
		}

		/// <summary>
		/// 	MS recomendation
		/// </summary>
		/// <param name="all"> </param>
		protected virtual void Dispose(bool all) {
			Container.Release(Object);
		}


		/// <summary>
		/// 	wrapped container
		/// </summary>
		protected IContainer Container;
	}

	/// <summary>
	/// 	Non generic object wrapper
	/// </summary>
	public class ContainerObjectWrapper : ContainerObjectWrapper<object> {
		/// <summary>
		/// 	Creates object wrapper from default container with no special options
		/// </summary>
		public ContainerObjectWrapper(Type type, string name = null)
			: this(type, Application.Current.Container, name) {}


		/// <summary>
		/// 	Creates object wrapper from given container with options
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="container"> </param>
		/// <param name="name"> </param>
		public ContainerObjectWrapper(Type type, IContainer container, string name = null) {
			Container = container;
			Name = name;
			Object = container.Get(type, name);
		}
	}
}