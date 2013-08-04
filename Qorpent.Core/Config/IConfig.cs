using System.Collections.Generic;

namespace Qorpent.Config {
	/// <summary>
	/// Описатель абстрактного конфига
	/// </summary>
	public interface IConfig {
		/// <summary>
		/// Устанавливает родительский конфиг
		/// </summary>
		/// <param name="config"></param>
		void SetParent(IConfig config);

		/// <summary>
		/// Установить значение параметра
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void Set(string name, object value);

		/// <summary>
		/// Получить строковую опцию
		/// </summary>
		/// <param name="name">имя опции</param>
		/// <param name="def">значение по умолчанию</param>
		/// <returns></returns>
		string Get(string name, string def = "");

		/// <summary>
		/// Получить приведенную типизированную опцию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		T Get<T>(string name, T def = default(T));

		/// <summary>
		/// Получение родителя
		/// </summary>
		/// <returns></returns>
		IConfig GetParent();
		/// <summary>
		/// Возвращает имена всех опций
		/// </summary>
		/// <param name="withParent"></param>
		/// <returns></returns>
		IEnumerable<string> GetNames(bool withParent = false);
	}
}