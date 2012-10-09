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
// Original file : RoleBasedAccessProviderForIWithRole.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Security.Principal;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.IoC;
using Qorpent.Model;

namespace Qorpent.Security {
	/// <summary>
	/// 	Использует <see cref="IWithRole" /> и <see cref="IRoleResolver" /> для определения соответвия
	/// 	Поддерживается
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,ServiceType = typeof(IAccessProviderExtension))]
	public class RoleBasedAccessProviderForIWithRole : IAccessProviderExtension {
		/// <summary>
		/// </summary>
		[Inject] public ILogicalExpressionEvaluator FormulaEvaluator { get; set; }


		/// <summary>
		/// 	Возвращает True - если в объекте объявлены роли и пользователь им соответствует
		/// 	NoDef - если у объекта не указана роль
		/// 	False - если у объекта указаны роли, но пользователь име не соответствует
		/// </summary>
		/// <param name="target"> целевой объект (должен быть IWithRole) </param>
		/// <param name="accessRole"> игнорируется, на данный нет понятия соответствия роли доступа и ролииспользования </param>
		/// <param name="principal"> пользователь (если не указан - использовать системного) </param>
		/// <param name="resolver"> подсистема разрешения ролей (системная, если не указано) </param>
		/// <returns> true - указанный пользователь имеет указанные права доступа к объекту </returns>
		public AccessResult IsAccessible(object target, AccessRole accessRole, IPrincipal principal, IRoleResolver resolver) {
			var role = ((IWithRole) target).Role.Trim();
			if (string.IsNullOrEmpty(role)) {
				return "no role defined";
			}
			if (role == "DEFAULT") {
				return "Allow DEFAULT role is always true";
			}
			var adapter = new RoleResolverTermAdapter(resolver, principal);
			if ((role.Contains("&") || role.Contains("!") || role.Contains("|")) && !(role.Contains(","))) {
				if (null == FormulaEvaluator) {
					throw new Exception(
						"нет возможности работы с формульными ролями в связи с отсутствием сконфигурированной подсистемы изучения формул");
				}
				return FormulaEvaluator.Eval(role, adapter);
			}
			var roles = role.Split(',', ';', '/');
			foreach (var r in roles) {
				if (r == "DEFAULT") {
					return "Allow разрешено для роли DEFAULT";
				}
				var r_ = r;
				if (r_.StartsWith("!")) {
					//special deny role, must be first
					r_ = "exact___" + r_.Substring(1);
					if (adapter.Get(r_)) {
						return "Deny явно назначен запрет на " + r_;
					}
				}
				else if (adapter.Get(r_)) {
					return "Allow соответствие роли " + r_;
				}
			}
			return "Deny нет соответствия";
		}

		/// <summary>
		/// 	True - если объект поддерживает <see cref="IWithRole" /> и роль непустая
		/// </summary>
		/// <param name="obj"> </param>
		/// <returns> </returns>
		public bool IsSupported(object obj) {
			return obj is IWithRole && !string.IsNullOrEmpty(((IWithRole) obj).Role);
		}
	}
}