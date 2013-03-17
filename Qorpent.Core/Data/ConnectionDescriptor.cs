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
	/// 	�������� ����������
	/// </summary>
	[Serialize]
	public class ConnectionDescriptor {
		/// <summary>
		/// </summary>
		public ConnectionDescriptor() {
			ConnectionType = typeof (SqlConnection);
		}

		/// <summary>
		/// 	��� ����������
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	������ �����������
		/// </summary>
		[SerializeNotNullOnly] public string ConnectionString { get; set; }

		/// <summary>
		/// 	��� �����������
		/// </summary>
		public Type ConnectionType { get; set; }

		/// <summary>
		/// 	�������� ��� ������������
		/// </summary>
		public string ConnectionTypeName {
			get { return ConnectionType.AssemblyQualifiedName; }
		}

		/// <summary>
		/// 	��� ����������� � ����������
		/// </summary>
		[SerializeNotNullOnly] public string ContainerName { get; set; }

		/// <summary>
		/// 	True - ������������ ��������� ��� ��������
		/// </summary>
		public bool InstantiateWithContainer { get; set; }

		/// <summary>
		/// 	������ �� ���������
		/// </summary>
		[SerializeNotNullOnly] public IContainer Container { get; set; }

		/// <summary>
		/// 	������������� ������ � ����������
		/// </summary>
		public string Evidence { get; set; }
		/// <summary>
		/// �������� ���������� �� ������� ��� ������������ ���������� �������
		/// </summary>
		public bool PresereveCleanup { get; set; }
	}
}