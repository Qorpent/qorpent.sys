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
	/// 	—ервис уровн€ приложени€ по определению доступа
	/// </summary>
	public interface IAccessProviderService : IAccessProvider {
		/// <summary>
		/// 	ѕозвол€ет получить собственную, не св€занную с блокировками копию провайдера
		/// </summary>
		/// <returns> </returns>
		IAccessProvider GetProvider();
	}

	/// <summary>
	/// 	–еализаи€ <see cref="IAccessProviderService" /> по умолчанию
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class AccessProviderService : ServiceBase, IAccessProviderService {
		/// <summary>
		/// 	»Ќстанци€ провайдера по умолчанию
		/// </summary>
		[Inject] public IAccessProvider Default { get; set; }


		/// <summary>
		/// 	¬озвращает наличие прав на определенное использование объекта
		/// </summary>
		/// <param name="target"> целевой объект </param>
		/// <param name="accessRole"> требуемые права </param>
		/// <param name="principal"> пользователь (если не указан - использовать системного) </param>
		/// <param name="resolver"> подсистема разрешени€ ролей (системна€, если не указано) </param>
		/// <returns> true - указанный пользователь имеет указанные права доступа к объекту </returns>
		public AccessResult IsAccessible(object target, AccessRole accessRole = AccessRole.Access, IPrincipal principal = null,
		                                 IRoleResolver resolver = null) {
			lock (this) {
				return Default.IsAccessible(target, accessRole, principal, resolver);
			}
		}

		/// <summary>
		/// 	ѕозвол€ет получить собственную, не св€занную с блокировками копию провайдера
		/// </summary>
		/// <returns> </returns>
		public IAccessProvider GetProvider() {
			return ResolveService<IAccessProvider>();
		}
	}
}