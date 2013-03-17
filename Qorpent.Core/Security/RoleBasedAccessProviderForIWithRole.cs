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
// PROJECT ORIGIN: Qorpent.Core/RoleBasedAccessProviderForIWithRole.cs
#endregion
using System;
using System.Security.Principal;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.IoC;
using Qorpent.Model;

namespace Qorpent.Security {
	/// <summary>
	/// 	���������� <see cref="IWithRole" /> � <see cref="IRoleResolver" /> ��� ����������� ����������
	/// 	��������������
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof (IAccessProviderExtension))]
	public class RoleBasedAccessProviderForIWithRole : IAccessProviderExtension {
		/// <summary>
		/// </summary>
		[Inject] public ILogicalExpressionEvaluator FormulaEvaluator { get; set; }


		/// <summary>
		/// 	���������� True - ���� � ������� ��������� ���� � ������������ �� �������������
		/// 	NoDef - ���� � ������� �� ������� ����
		/// 	False - ���� � ������� ������� ����, �� ������������ ��� �� �������������
		/// </summary>
		/// <param name="target"> ������� ������ (������ ���� IWithRole) </param>
		/// <param name="accessRole"> ������������, �� ������ ��� ������� ������������ ���� ������� � ����������������� </param>
		/// <param name="principal"> ������������ (���� �� ������ - ������������ ����������) </param>
		/// <param name="resolver"> ���������� ���������� ����� (���������, ���� �� �������) </param>
		/// <returns> true - ��������� ������������ ����� ��������� ����� ������� � ������� </returns>
		public AccessResult IsAccessible(object target, AccessRole accessRole, IPrincipal principal, IRoleResolver resolver) {
			var role = ((IWithRole) target).Role.Trim();
			string customcontext = null;
			var withRoleContext = target as IWithRoleContext;
			if (withRoleContext != null) {
				customcontext = withRoleContext.RoleContext;
			}
			if (string.IsNullOrEmpty(role)) {
				return "no role defined";
			}
			if (role == "DEFAULT") {
				return "Allow DEFAULT role is always true";
			}
			var adapter = new RoleResolverTermAdapter(resolver, principal, customcontext);
			if ((role.Contains("&") || role.Contains("!") || role.Contains("|")) && !(role.Contains(","))) {
				if (null == FormulaEvaluator) {
					throw new Exception(
						"��� ����������� ������ � ����������� ������ � ����� � ����������� ������������������ ���������� �������� ������");
				}
				return FormulaEvaluator.Eval(role, adapter);
			}
			var roles = role.Split(',', ';', '/');
			foreach (var roleInList in roles) {
				if (roleInList == "DEFAULT") {
					return "Allow ��������� ��� ���� DEFAULT";
				}
				var correctedRole = roleInList;
				if (correctedRole.StartsWith("!")) {
					//special deny role, must be first
					correctedRole = "exact___" + correctedRole.Substring(1);
					if (adapter.Get(correctedRole)) {
						return "Deny ���� �������� ������ �� " + correctedRole;
					}
				}
				else if (adapter.Get(correctedRole)) {
					return "Allow ������������ ���� " + correctedRole;
				}
			}
			return "Deny ��� ������������";
		}

		/// <summary>
		/// 	True - ���� ������ ������������ <see cref="IWithRole" /> � ���� ��������
		/// </summary>
		/// <param name="obj"> </param>
		/// <returns> </returns>
		public bool IsSupported(object obj) {
			return obj is IWithRole && !string.IsNullOrEmpty(((IWithRole) obj).Role);
		}
	}
}