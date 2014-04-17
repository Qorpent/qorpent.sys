using System.Collections.Generic;

namespace Qorpent.Config {
	/// <summary>
	///     Описатель абстрактного конфига
	/// </summary>
	public interface IConfig : IDictionary<string, object> {
		/// <summary>
		///     Устанавливает родительский конфиг
		/// </summary>
		/// <param name="config"></param>
		void SetParent(IConfig config);
		/// <summary>
		///     Установить значение параметра
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void Set(string name, object value);
		/// <summary>
		///     Получить приведенную типизированную опцию
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		T Get<T>(string name, T def = default(T));
		/// <summary>
		///     Получение родителя
		/// </summary>
		/// <returns></returns>
		IConfig GetParent();
		/// <summary>
		///     Возвращает имена всех опций
		/// </summary>
		/// <param name="withParent"></param>
		/// <returns></returns>
		IEnumerable<string> GetNames(bool withParent = false);
		/// <summary>
		///     Сериализация конфига в заданном формате
		/// </summary>
		/// <param name="rendertype"></param>
		/// <returns></returns>
		string ToString(ConfigRenderType rendertype);

		/// <summary>
		/// 
		/// </summary>
		void Stornate();
	}
}