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
// PROJECT ORIGIN: Qorpent.Data/ConfigBasedConnectionProviderExtension.cs
#endregion
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// —читывает информацию из штатного конфига
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "config.based.connection.provider",ServiceType = typeof (IDatabaseConnectionProviderExtension))]
	public class ConfigBasedConnectionProviderExtension : ServiceBase, IDatabaseConnectionProviderExtension {
		/// <summary>
		/// —читывает соединени€ из штатного конфига
		/// </summary>
		/// <returns></returns>
		public IEnumerable<ConnectionDescriptor> GetConnections() {
			var connections = ConfigurationManager.ConnectionStrings;
			foreach (ConnectionStringSettings connection in connections) {
				var con = new ConnectionDescriptor
					{Name = connection.Name, ConnectionString = connection.ConnectionString, ConnectionType = typeof (SqlConnection)};
				con.Evidence = "config:" + connection.ElementInformation.Source + ":" + connection.ElementInformation.LineNumber;
				yield return con;
			}
		}
	}
}