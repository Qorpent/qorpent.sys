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
// PROJECT ORIGIN: Qorpent.Data/Entity.cs
#endregion
using System;
using Qorpent.Model;

namespace Zeta.Data.Model {
	/// <summary>
	/// ������� ����� ��������
	/// </summary>
	public class Entity : IEntity {
		/// <summary>
		/// 	������������� ���������� �������������
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	��������� ���������� �������������
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	��������/���
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// 	������ �����
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// 	�����������
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 	��������
		/// </summary>
		public DateTime Version { get; set; }
	}
}