using System;
using System.Linq;
using System.Xml.Linq;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// Базовый сервис провайдера BSharpRuntime
	/// </summary>
	public class BSharpRuntimeProviderService : ServiceBase, IBSharpRuntimeProviderService {

		/// <summary>
		/// Массив локаторов ресурсов BSharp
		/// </summary>
		[Inject]
		public IBSharpRuntimeProvider[] Providers { get; set; }

		/// <summary>
		/// Массив сериализаторов объектов
		/// </summary>
		[Inject]
		public IBSharpRuntimeBuilder[] Builders { get; set; }

		/// <summary>
		/// Разрешает имена классов с использованием корневого неймспейса
		/// используется при поздних референсах
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootnamespace"></param>
		/// <returns></returns>
		public string Resolve(string name, string rootnamespace) {
			string result = null;
			foreach (var p in Providers) {
				result = p.Resolve(name, rootnamespace);
				if (null != result) {
					break;
				}
			}
			return result;
		}

		/// <summary>
		/// Возвращает исходное определение класса BSharp
		/// </summary>
		/// <param name="fullname"></param>
		/// <returns></returns>
		public XElement GetRaw(string fullname) {
			XElement result = null;
			foreach (var p in Providers)
			{
				result = p.GetRaw(fullname);
				if (null != result)
				{
					break;
				}
			}
			return result;
		}

	

		/// <summary>
		/// Возвращает типизированный сериализованный объект IBSharp
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public T Get<T>(string name) where T : class {
			var raw = GetRaw(name);
			var builder = Builders.FirstOrDefault(_ => _.IsSupported<T>());
			if (null != builder) {
				return builder.Create<T>(raw);
			}
			T result = null;
			if (typeof (T).IsInterface) {
				result = ResolveService<T>();
			}
			else {
				result = Activator.CreateInstance<T>();
			}
			if (null == result) {
				throw new Exception("no implementation class was found");
			}
			if (result is IXmlReadable) {
				((IXmlReadable)result).ReadFromXml(raw);
			}
			else {
				throw new Exception("result object does not support initialization from XML");
			}
			return result;
		}

	}
}