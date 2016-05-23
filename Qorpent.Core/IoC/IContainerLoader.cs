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
// PROJECT ORIGIN: Qorpent.Core/IContainerLoader.cs
#endregion
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

namespace Qorpent.IoC {
	/// <summary>
	/// 	interface for container loader
	/// </summary>
	public interface IContainerLoader {
		/// <summary>
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		IEnumerable<IComponentDefinition> LoadDefaultManifest(bool allowErrors);

		/// <summary>
		/// </summary>
		/// <param name="manifest"> </param>
		/// <param name="allowErrors"> </param>
		/// <returns> </returns>
		IEnumerable<IComponentDefinition> LoadManifest(XElement manifest, bool allowErrors);

		/// <summary>
		/// 	Configures assembly to container if ContainerExport attribute defined
		/// </summary>
		/// <param name="assembly"> </param>
		/// <param name="requreManifest"> </param>
		/// <param name="context"></param>
		/// <returns> </returns>
		IEnumerable<IComponentDefinition> LoadAssembly(Assembly assembly, bool requreManifest = false,object context = null, bool autoonly=false);

		/// <summary>
		/// 	Читает манифетсы приложения и конструирует единый
		/// </summary>
		/// <returns> </returns>
		/// <exception cref="Exception"></exception>
		XElement ReadDefaultManifest();

		/// <summary>
		/// 	Loads all components defined on type
		/// </summary>
		/// <param name="type"> </param>
		/// <returns> </returns>
		IEnumerable<IComponentDefinition> LoadType(Type type);

		/// <summary>
		/// 	Loads all components defined on type
		/// </summary>
		/// <returns> </returns>
		IEnumerable<IComponentDefinition> Load<T>();
	}
}