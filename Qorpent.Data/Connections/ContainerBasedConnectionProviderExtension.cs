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
// PROJECT ORIGIN: Qorpent.Data/ContainerBasedConnectionProviderExtension.cs
#endregion
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// —читывает информацию из контейнера по типу IDbConnection
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "container.based.connection.provider",
		ServiceType = typeof (IDatabaseConnectionProviderExtension))]
	public class ContainerBasedConnectionProviderExtension : ServiceBase, IDatabaseConnectionProviderExtension {
		/// <summary>
		/// ќбходит компоненты контейнера и находит все IDbConnection
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> GetConnections() {
			foreach (var source in Container.GetComponents().Where(x=>x.ServiceType==typeof(IDbConnection))) {
				if(source.Name.IsEmpty()) continue;
				var con = new ConnectionDescriptor();
				con.Name = source.Name.Split('.', '/')[0];
				con.InstantiateWithContainer = true;
				con.Container = this.Container;
				con.ContainerName = source.Name;
				con.Evidence = "app.container";
				yield return con;
			}
		}
	}
}