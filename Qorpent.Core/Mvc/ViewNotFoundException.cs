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
// PROJECT ORIGIN: Qorpent.Core/ViewNotFoundException.cs
#endregion
using System;
using System.Runtime.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Occures where QView was not found
	/// </summary>
	[Serializable]
	public class ViewNotFoundException : MvcExecption {
		/// <summary>
		/// </summary>
		/// <param name="viewname"> </param>
		/// <param name="context"> </param>
		public ViewNotFoundException(string viewname, IMvcContext context = null)
			: base("view not found " + viewname, context) {
			ViewName = viewname;
		}

		/// <summary>
		/// 	Name of view havenot found
		/// </summary>
		public string ViewName { get; protected set; }

		/// <summary>
		/// 	��� ��������������� � ����������� ������ ������ �������� �� ���������� ��� <see
		/// 	 cref="T:System.Runtime.Serialization.SerializationInfo" />.
		/// </summary>
		/// <param name="info"> ������ <see cref="T:System.Runtime.Serialization.SerializationInfo" /> , ���������� ��������������� ������ ������� � ������������� ����������. </param>
		/// <param name="context"> ������ <see cref="T:System.Runtime.Serialization.StreamingContext" /> , ���������� ����������� �������� �� ��������� ��� ����������. </param>
		/// <exception cref="T:System.ArgumentNullException">��������
		/// 	<paramref name="info" />
		/// 	� ��������� NULL (Nothing � Visual Basic).</exception>
		/// <filterpriority>2</filterpriority>
		/// <PermissionSet>
		/// 	<IPermission
		/// 		class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
		/// 		version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
		/// 	<IPermission
		/// 		class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
		/// 		version="1" Flags="SerializationFormatter" />
		/// </PermissionSet>
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("_ViewName", ViewName);
		}
	}
}