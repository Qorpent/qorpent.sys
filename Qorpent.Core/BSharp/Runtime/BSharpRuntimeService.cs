using System.Collections.Generic;
using System.Linq;
using Qorpent.IoC;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Базовый сервис провайдера BSharpRuntime
	/// </summary>
	public class BSharpRuntimeService : ServiceBase, IBSharpRuntimeService {
		/// <summary>
		///     Массив локаторов ресурсов BSharp
		/// </summary>
		[Inject]
		public IBSharpRuntimeProvider[] Providers { get; set; }

		/// <summary>
		///     Массив сериализаторов объектов
		/// </summary>
		[Inject]
		public IBSharpRuntimeActivatorService[] Activators { get; set; }

		/// <summary>
		///     Разрешает имена классов с использованием корневого неймспейса
		///     используется при поздних референсах
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootnamespace"></param>
		/// <returns></returns>
		public string Resolve(string name, string rootnamespace) {
			lock (this) {
				string result = null;
				foreach (IBSharpRuntimeProvider p in Providers) {
					result = p.Resolve(name, rootnamespace);
					if (null != result) {
						break;
					}
				}
				return result;
			}
		}

		/// <summary>
		///     Возвращает исходное определение класса BSharp
		/// </summary>
		/// <param name="fullname"></param>
		/// <returns></returns>
		public IBSharpRuntimeClass GetRuntimeClass(string fullname) {
			lock (this) {
				IBSharpRuntimeClass result = null;
				foreach (IBSharpRuntimeProvider p in Providers) {
					result = p.GetRuntimeClass(fullname);
					if (null != result) {
						break;
					}
				}
				return result;
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetClassNames(string mask) {
			lock (this) {
				return Providers.SelectMany(_ => _.GetClassNames(mask)).Distinct().ToArray();
			}
		}

		/// <summary>
		///     Очищает кэш классов
		/// </summary>
		public void Refresh() {
			lock (this) {
				foreach (IBSharpRuntimeProvider p in Providers) {
					p.Refresh();
				}
			}
		}


		/// <summary>
		///     Активирует сервис по имени класса
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="acivationType"></param>
		/// <returns></returns>
		public T Activate<T>(string name, BSharpActivationType acivationType = BSharpActivationType.Auto) {
			IBSharpRuntimeClass runtimeclass = GetRuntimeClass(name);
			if (null == runtimeclass) {
				throw new BSharpRuntimeException("cannot create runtime class with name " + name);
			}
			return Activate<T>(runtimeclass, acivationType);
		}

		/// <summary>
		///     Создать типизированный объект из динамического объекта BSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Activate<T>(IBSharpRuntimeClass runtimeclass, BSharpActivationType acivationType) {
			IBSharpRuntimeActivatorService activator = Activators
				.OrderBy(_ => _.Index)
				.FirstOrDefault(_ => _.CanActivate<T>(runtimeclass, acivationType));
			if (null == activator) {
				throw new BSharpRuntimeException("cannot find activator for " + runtimeclass.Fullname +
				                                 " to create requested service " + typeof (T).Name);
			}

			return activator.Activate<T>(runtimeclass, acivationType);
		}
	}
}