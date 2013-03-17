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
// PROJECT ORIGIN: Qorpent.Data/FileBasedConnectionRegistryPersister.cs
#endregion
using System.IO;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Data.Connections {
	/// <summary>
	/// Сохраняет строки подключения к БД в локальном каталоге /usr приложения
	/// </summary>
	[ContainerComponent(Lifestyle.Extension, "file.based.connection.registry-persister",
		ServiceType = typeof (IDatabaseConnectionRegistryExtension))]
	public class FileBasedConnectionRegistryPersister : ServiceBase, IDatabaseConnectionRegistryExtension {
		/// <summary>
		/// Снимает строку подключения с регистрации (удаляет файл)
		/// </summary>
		/// <param name="name"></param>
		public void Unregister(string name) {
			var filename = GetFileName(name);
			if(File.Exists(filename)) {
				File.Delete(filename);
			}
		}

		private string GetFileName(string name) {
			var filename =
				Container.Get<IFileNameResolver>().Resolve(FileSearchQuery.Leveled("~/usr/" + name + ".fbcrp.db-connection"));
			return filename;
		}

		/// <summary>
		/// Зарегистрировать соединение с БД (создает файл)
		/// </summary>
		public void Register(ConnectionDescriptor connectionDescriptor) {
			var filename = GetFileName(connectionDescriptor.Name);
			Directory.CreateDirectory(Path.GetDirectoryName(filename));
			var str = string.Format("{0} '{1}' '{2}'", connectionDescriptor.Name, connectionDescriptor.ConnectionType.AssemblyQualifiedName,
			                        connectionDescriptor.ConnectionString);
			File.WriteAllText(filename,str);
		}
	}
}