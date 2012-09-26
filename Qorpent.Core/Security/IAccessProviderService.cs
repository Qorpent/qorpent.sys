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
// Original file : IAccessProviderService.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	������ ������ ���������� �� ����������� �������
	/// </summary>
	public interface IAccessProviderService : IAccessProvider {
		/// <summary>
		/// 	��������� �������� �����������, �� ��������� � ������������ ����� ����������
		/// </summary>
		/// <returns> </returns>
		IAccessProvider GetProvider();
	}

	/// <summary>
	/// 	��������� <see cref="IAccessProviderService" /> �� ���������
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class AccessProviderService : ServiceBase, IAccessProviderService {
		/// <summary>
		/// 	��������� ���������� �� ���������
		/// </summary>
		[Inject] public IAccessProvider Default { get; set; }


		/// <summary>
		/// 	���������� ������� ���� �� ������������ ������������� �������
		/// </summary>
		/// <param name="target"> ������� ������ </param>
		/// <param name="accessRole"> ��������� ����� </param>
		/// <param name="principal"> ������������ (���� �� ������ - ������������ ����������) </param>
		/// <param name="resolver"> ���������� ���������� ����� (���������, ���� �� �������) </param>
		/// <returns> true - ��������� ������������ ����� ��������� ����� ������� � ������� </returns>
		public AccessResult IsAccessible(object target, AccessRole accessRole = AccessRole.Access, IPrincipal principal = null,
		                                 IRoleResolver resolver = null) {
			lock (this) {
				return Default.IsAccessible(target, accessRole, principal, resolver);
			}
		}

		/// <summary>
		/// 	��������� �������� �����������, �� ��������� � ������������ ����� ����������
		/// </summary>
		/// <returns> </returns>
		public IAccessProvider GetProvider() {
			return ResolveService<IAccessProvider>();
		}
	}
}