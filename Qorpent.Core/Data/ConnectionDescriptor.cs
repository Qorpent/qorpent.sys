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
// PROJECT ORIGIN: Qorpent.Core/ConnectionDescriptor.cs
#endregion
using System;
using System.Data.SqlClient;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Data {
	/// <summary>
	/// 	Описание соединения
	/// </summary>
	[Serialize]
	public class ConnectionDescriptor {
		/// <summary>
		/// </summary>
		public ConnectionDescriptor() {
			ConnectionType = typeof (SqlConnection);
		}

		/// <summary>
		/// 	Имя соединения
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Строка подключения
		/// </summary>
		[SerializeNotNullOnly] public string ConnectionString { get; set; }

		/// <summary>
		/// 	Тип подключения
		/// </summary>
		public Type ConnectionType { get; set; }

		/// <summary>
		/// 	Свойство для сериализации
		/// </summary>
		public string ConnectionTypeName {
			get { return ConnectionType.AssemblyQualifiedName; }
		}

		/// <summary>
		/// 	Имя подключения в контейнере
		/// </summary>
		[SerializeNotNullOnly] public string ContainerName { get; set; }

		/// <summary>
		/// 	True - использовать контейнер для загрузки
		/// </summary>
		public bool InstantiateWithContainer { get; set; }

		/// <summary>
		/// 	Ссылка на контейнер
		/// </summary>
		[SerializeNotNullOnly] public IContainer Container { get; set; }

		/// <summary>
		/// 	Происхождение строки в провайдере
		/// </summary>
		public string Evidence { get; set; }
		/// <summary>
		/// Защищает соединение от очистки при перезагрузке компонента реестра
		/// </summary>
		public bool PresereveCleanup { get; set; }
	}
}