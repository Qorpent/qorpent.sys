using System.Security.Principal;
using Qorpent.IoC;

namespace Qorpent.Security {
	/// <summary>
	/// 	Реализаия <see cref="IAccessProviderService" /> по умолчанию
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof(IAccessProviderService))]
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