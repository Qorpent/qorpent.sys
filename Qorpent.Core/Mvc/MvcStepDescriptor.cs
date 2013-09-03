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
// PROJECT ORIGIN: Qorpent.Core/MvcStepDescriptor.cs
#endregion
using System;
using Qorpent.Model;
using Qorpent.Security;
using Qorpent.Serialization;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Basis for mvc descriptors
	/// </summary>
	[Serialize]
	public abstract class MvcStepDescriptor : INotModifiedStateProvider, IWithRole, IWithRoleContext {
		/// <summary>
		/// 	Name of step implementation
		/// </summary>
		[SerializeNotNullOnly] public string Name { get; set; }

		/// <summary>
		/// 	Role required to access this step
		/// </summary>
		[SerializeNotNullOnly] public string DirectRole { get; set; }

		/// <summary>
		/// 	Containing factory
		/// </summary>
		public IMvcFactory Factory { get; set; }


		/// <summary>
		/// 	Last modified header wrapper
		/// </summary>
		[IgnoreSerialize] public abstract DateTime LastModified { get; }

		/// <summary>
		/// 	Etag header wrapper
		/// </summary>
		[IgnoreSerialize] public abstract string ETag { get; }


		/// <summary>
		/// 	True if object supports 304 state
		/// </summary>
		[IgnoreSerialize] public abstract bool SupportNotModifiedState { get; }

		/// <summary>
		/// 	Wrapper fo Descritor.Role
		/// </summary>
		[Serialize] public string Role {
			get { return _role ?? (_role = PrepareRole()); }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// 	Дополнительный контекст проверки роли
		/// </summary>
		public string RoleContext { get; set; }

		/// <summary>
		/// 	Подготовка полной строки с описанием роли
		/// </summary>
		/// <returns> </returns>
		protected abstract string PrepareRole();

		private string _role;
	}
}