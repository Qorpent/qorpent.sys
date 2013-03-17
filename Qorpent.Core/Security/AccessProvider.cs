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
// PROJECT ORIGIN: Qorpent.Core/AccessProvider.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	Провайдер проверки доступа по умолчанию, использует в своей работе расширения
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class AccessProvider : ServiceBase, IAccessProvider {
		/// <summary>
		/// 	Дочерние проводники проверок доступа, используемые провайдером
		/// </summary>
		[Inject] public IList<IAccessProviderExtension> SubProviders { get; set; }

		/// <summary>
		/// 	Ссылка на держатель текущего пользователя
		/// </summary>
		[Inject] public IPrincipalSource PrincipalSource { get; set; }

		/// <summary>
		/// 	Ссылка на систему разрешения ролей
		/// </summary>
		[Inject] public IRoleResolver RoleResolver { get; set; }

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
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			lock (Sync) {
				Log.Debug("start acl");
				try {
					if (null == SubProviders) {
						return true;
					}
					var activeProviders = SubProviders.Where(x => x.IsSupported(target)).ToArray();
					if (0 == activeProviders.Length) {
						return true;
					}
					if (null == principal) {
						principal = PrincipalSource.CurrentUser;
					}
					if (null == resolver) {
						resolver = RoleResolver;
					}
					var result = new AccessResult {Provider = this, ResultType = AccessResultType.NotDefined, Description = ""};
					foreach (var extension in activeProviders) {
						var subresult = extension.IsAccessible(target, accessRole, principal, resolver);
						if (null == subresult.Provider) {
							subresult.Provider = extension;
						}
						result.SubResults.Add(subresult);

						if (!string.IsNullOrEmpty(subresult.Description)) {
							result.Description += subresult.Description + Environment.NewLine;
						}


						if (subresult.ResultType == AccessResultType.Deny) {
							//если расширение вернуло запрет, то на этом все проверки завершаются
							result.ResultType = AccessResultType.Deny;
							break;
						}

						if (subresult.ResultType == AccessResultType.Allow) {
							// если расширение дало отклик разрешения, то мы выставляем и обшее разрешение
							result.ResultType = AccessResultType.Allow;
						}
					}
					Log.Debug("end acl");
					return result;
				}
				catch (Exception ex) {
					var exc = new QorpentSecurityException("Ошибка определения доступа к объекту " + target, ex);
					Log.Error(exc.Message, exc);
					throw exc;
				}
			}
		}
	}
}