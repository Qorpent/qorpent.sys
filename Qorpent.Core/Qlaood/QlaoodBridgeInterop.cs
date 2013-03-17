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
// PROJECT ORIGIN: Qorpent.Core/QlaoodBridgeInterop.cs
#endregion
using System;
using System.Linq;
using Qorpent.Applications;

namespace Qorpent.Qlaood {
	/// <summary>
	/// 	Interop (between domains) wrapper fo QlaoodBridge
	/// </summary>
	public class QlaoodBridgeInterop : MarshalByRefObject {
		
		/// <summary>
		/// 	Setup qlaood host for current domain
		/// </summary>
		/// <param name="host"> </param>
		public void SetHost(IQlaoodHost host) {
			QlaoodBridge.QlaoodHost = host;
		}

		/// <summary>
		/// Order to load assembly in hosted environment
		/// </summary>
		/// <param name="name"></param>
		public void LoadAssembly(string name) {
			if(AppDomain.CurrentDomain.GetAssemblies().All(x => x.GetName().Name != name)) {
				AppDomain.CurrentDomain.Load(name);
			}
		}

		/// <summary>
		/// Hook to create classes in hosted domain
		/// </summary>
		/// <param name="typename"></param>
		/// <returns></returns>
		public object Create(string typename) {
			var type = Type.GetType(typename, true);
			return Activator.CreateInstance(type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public object GetService<T>() where T : class {
			return Application.Current.Container.Get<T>();
		}
	
	}
}