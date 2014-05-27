using System.Collections.Generic;

namespace Qorpent.Data{
	/// <summary>
	/// 
	/// </summary>
	public interface IObjectDataCache<T> where T:class,new(){
		/// <summary>
		/// Возвращает следующий ID
		/// </summary>
		/// <returns></returns>
		int GetNextId();
		
		/// <summary>
		/// Возвращает сущность по коду или ID
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get(object key);

		/// <summary>
		/// Проверить наличие
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool Exists(object key);

		/// <summary>
		/// Установить значение напрямую
		/// </summary>
		/// <param name="value"></param>
		void Set(T value);

		/// <summary>
		/// Очищает кэш
		/// </summary>
		void Clear();

		/// <summary>
		/// Возвращает все по запросу
		/// </summary>
		/// <param name="query"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		T[] GetAll(object query, object options);
	}
}