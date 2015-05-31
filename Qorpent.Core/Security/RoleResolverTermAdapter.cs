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
// PROJECT ORIGIN: Qorpent.Core/RoleResolverTermAdapter.cs
#endregion
using System;
using System.Collections.Generic;
using System.Security.Principal;
using qorpent.v2.security.authorization;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;

namespace Qorpent.Security {
	/// <summary>
    /// 	¬спомогательный класс дл€ использовани€ в качестве обертки <see cref="IRoleResolverService" />
	/// </summary>
	public class RoleResolverTermAdapter : ILogicTermSource {
		/// <summary>
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="principal"> </param>
		/// <param name="customcontext"> </param>
		public RoleResolverTermAdapter(IRoleResolverService resolver, IPrincipal principal, string customcontext) {
			_resolver = resolver;
			_principal = principal;
			_customcontext = customcontext;
		}


		/// <summary>
		/// 	¬озвращает true если переданна€ роль соответствует пользователю, сконфигурированному в адаптере
		/// 	если роль начинаетс€ на exact___ то используетс€ метод соответстви€ "точное соответствие" - ADMIN 
		/// 	в таких случа€х не рассматриваетс€ как "люба€ роль"
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public bool Get(string name) {
			var exact = false;
			if (name.StartsWith("exact___")) {
				exact = true;
				name = name.Substring(8);
			}
			return _resolver.IsInRole(_principal.Identity, name, exact);
		}

	    /// <summary>
	    /// 	ƒл€ данного источника не поддерживаетс€
	    /// </summary>
	    /// <param name="name"> </param>
	    /// <param name="value"> </param>
	    /// <param name="isNumber"></param>
	    /// <returns> </returns>
	    public bool Equal(string name, string value, bool isNumber) {
			throw new NotSupportedException();
		}

		/// <summary>
		/// 	ƒл€ данного источника не поддерживаетс€
		/// </summary>
		/// <returns> </returns>
		public string Value(string name) {
			throw new NotSupportedException();
		}

		/// <summary>
		/// 	ƒл€ данного источника не поддерживаетс€
		/// </summary>
		/// <returns> </returns>
		public IDictionary<string, string> GetAll() {
			throw new NotSupportedException();
		}

		private readonly string _customcontext;

		private readonly IPrincipal _principal;
        private readonly IRoleResolverService _resolver;
	}
}