using System.Collections.Generic;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Интерфейс локатора ресурсов BSharp
	/// </summary>
	public interface IBSharpRuntimeProvider {
		/// <summary>
		///     Разрешает имена классов с использованием корневого неймспейса
		///     используется при поздних референсах
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rootnamespace"></param>
		/// <returns></returns>
		string Resolve(string name, string rootnamespace);

		/// <summary>
		///     Возвращает исходное определение класса BSharp
		/// </summary>
		/// <param name="fullname"></param>
		/// <returns></returns>
		IBSharpRuntimeClass GetRuntimeClass(string fullname);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetClassNames(string mask = null);

		/// <summary>
		///     Метод обновления кэшей при их наличии
		/// </summary>
		void Refresh();
	}
}