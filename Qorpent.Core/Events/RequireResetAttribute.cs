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
// PROJECT ORIGIN: Qorpent.Core/RequireResetAttribute.cs
#endregion
using System;
using Qorpent.Applications;

namespace Qorpent.Events {
	/// <summary>
	/// 	�������� ����� ��� �������������� ��������� Reset-��������������, ����� ������ ���� ����������� �� <see
	/// 	 cref="IResetable" />
	/// </summary>
	/// <remarks>
	/// 	<see cref="ServiceBase" /> ������������ �������������� ��������� �� Reset ������ �� ����� �������� � 
	/// 	���������� ��� � ����������� <see cref="IApplicationBound.SetApplication" />
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class RequireResetAttribute : Attribute {
		/// <summary>
		/// 	����������� ����������� ��������
		/// </summary>
		public RequireResetAttribute() {
			All = true;
			UseClassNameAsOption = true;
		}

		/// <summary>
		/// 	���� ����������� ������ ����, �� ��� ���������� ������� �������� ��������� ������� ���� ��� ������
		/// </summary>
		public string Role { get; set; }

		/// <summary>
		/// 	True (�� ���������) - ������ ����� ����������� �� ����� ������ "���"
		/// </summary>
		public bool All { get; set; }

		/// <summary>
		/// 	True (�� ���������) - ��� ������ �������� ��������� ������ ��� ������ ����� ������ (� ������� ��������)
		/// </summary>
		public bool UseClassNameAsOption { get; set; }

		/// <summary>
		/// 	�������������� �����, �� ������� ����� ����������� ����� (����� ���� ������)
		/// </summary>
		public string[] Options { get; set; }
	}
}