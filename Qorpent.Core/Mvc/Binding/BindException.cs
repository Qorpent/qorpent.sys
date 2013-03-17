﻿#region LICENSE
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
// PROJECT ORIGIN: Qorpent.Core/BindException.cs
#endregion
using System;
using System.Runtime.Serialization;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// 	General binding exception
	/// </summary>
	[Serializable]
	public class BindException : MvcExecption {
		/// <summary>
		/// </summary>
		/// <param name="bindInfo"> </param>
		/// <param name="context"> </param>
		/// <param name="val"> </param>
		public BindException(BindAttribute bindInfo, IMvcContext context, object val)
			: base(bindInfo.ErrorMessage, context) {
			_bindInfo = bindInfo;
			_val = val;
		}

		/// <summary>
		/// </summary>
		public BindAttribute BindInfo {
			get { return _bindInfo; }
		}


		/// <summary>
		/// </summary>
		public object Val {
			get { return _val; }
		}

		/// <summary>
		/// 	При переопределении в производном классе задает сведения об исключении для <see
		/// 	 cref="T:System.Runtime.Serialization.SerializationInfo" />.
		/// </summary>
		/// <param name="info"> Объект <see cref="T:System.Runtime.Serialization.SerializationInfo" /> , содержащий сериализованные данные объекта о выбрасываемом исключении. </param>
		/// <param name="context"> Объект <see cref="T:System.Runtime.Serialization.StreamingContext" /> , содержащий контекстные сведения об источнике или назначении. </param>
		/// <exception cref="T:System.ArgumentNullException">Параметр
		/// 	<paramref name="info" />
		/// 	— указатель NULL (Nothing в Visual Basic).</exception>
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
			info.AddValue("_Val", Val);
			info.AddValue("_BindInfo", BindInfo);
		}

		private readonly BindAttribute _bindInfo;

		private readonly object _val;
	}
}