#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : IRoleResolverExtension.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Security.Principal;
using Qorpent.Model;
using Qorpent.Mvc;

namespace Qorpent.Security {
	/// <summary>
	/// 	Extension point interface to extend DefaultRoleResolver with custom logic of resolution
	/// </summary>
	public interface IRoleResolverExtension : IWithIdx {
		/// <param name="principal"> </param>
		/// <param name="role"> </param>
		/// <param name="exact"> </param>
		/// <param name="callcontext"> </param>
		/// <param name="customcontext"> </param>
		/// <returns> </returns>
		bool IsInRole(IPrincipal principal, string role, bool exact = false, IMvcContext callcontext = null,
		              object customcontext = null);

		/// <summary>
		/// Returns registered roles of this extension
		/// </summary>
		/// <returns></returns>
		[Obsolete("обеспечение совместимости с Comdiv.Core")]
		IEnumerable<string> GetRoles();

		/// <summary>
		/// ¬озвращает признак обслуживани€ учетных записей суперпользовател€
		/// </summary>
		[Obsolete("обеспечение совместимости с Comdiv.Core")]
		bool IsExclusiveSuProvider { get; }

	}
}