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
// PROJECT ORIGIN: Qorpent.Core/AccessProviderService.cs
#endregion
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	Реализаия <see cref="IAccessProviderService" /> по умолчанию
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof (IAccessProviderService))]
	public class AccessProviderService : ServiceBase, IAccessProviderService {
		/// <summary>
		/// 	ИНстанция провайдера по умолчанию
		/// </summary>
		[Inject] public IAccessProvider Default { get; set; }


		/// <summary>
		/// 	Возвращает наличие прав на определенное использование объекта
		/// </summary>
		/// <param name="target"> целевой объект </param>
		/// <param name="accessRole"> требуемые права </param>
		/// <param name="principal"> пользователь (если не указан - использовать системного) </param>
		/// <param name="resolver"> подсистема разрешения ролей (системная, если не указано) </param>
		/// <returns> true - указанный пользователь имеет указанные права доступа к объекту </returns>
		public AccessResult IsAccessible(object target, AccessRole accessRole = AccessRole.Access, IPrincipal principal = null,
		                                 IRoleResolver resolver = null) {
			lock (Sync) {
				return Default.IsAccessible(target, accessRole, principal, resolver);
			}
		}

		/// <summary>
		/// 	Позволяет получить собственную, не связанную с блокировками копию провайдера
		/// </summary>
		/// <returns> </returns>
		public IAccessProvider GetProvider() {
			return ResolveService<IAccessProvider>();
		}
	}
}