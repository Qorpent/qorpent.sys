using System;
using Qorpent.IoC;
using Qorpent.Selector.Implementations;
using Qorpent.Serialization;

namespace Qorpent.Selector {
	/// <summary>
	///     Фабрика по умолчанию  для стандартных реализаций языков
	///     используется как DEFAULT для IOC
	/// </summary>
	public class SelectorFactory : IFactory {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public object Get(Type serviceType, string name = "") {
			if (serviceType == typeof (ISelector)) {
				return new DefaultSelector();
			}

			if (serviceType == typeof (ISelectorImpl)) {
				if (name.EndsWith(".xpath")) {
					return new XPathSelectorImpl();
				}
				if (name.EndsWith(".regex")) {
					return new RegexSelectorImpl();
				}
				if (name.EndsWith(".css")) {
					return new CssSelectorImpl();
				}

				throw new ContainerException("нет настроенного ISelectorImpl для " + name);
			}

			throw new ContainerException(serviceType.FullName + " не поддерживается");
		}
	}
}