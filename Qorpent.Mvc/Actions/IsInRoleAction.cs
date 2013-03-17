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
// PROJECT ORIGIN: Qorpent.Mvc/IsInRoleAction.cs
#endregion
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// Проверяет наличие роли у указанного пользователя
	/// </summary>
	[Action("_sys.isinrole",Role="SECURITYMANAGER",Help = "Проверка роли у пользователя")]
	public class IsInRoleAction: ActionBase {
		[Bind(Required = true)] private string usr= "";
		[Bind(Required = true)]private string role= "";
		[Bind] private bool exact = false;
		/// <summary>
		/// Производит вызов IsInRole Application.Roles
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			return Application.Roles.IsInRole(usr, role, exact);
		}
	}
}