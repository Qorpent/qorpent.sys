using System.Collections.Generic;

namespace Qorpent.Data.DataCache{
	/// <summary>
	/// 
	/// </summary>
	public interface IExternalDataProvider{
		/// <summary>
		/// Проверяет поддержку загрузки определенного типа объектов
		/// </summary>
		/// <returns></returns>
		bool Supports<T>(object query = null);
		/// <summary>
		/// Осуществляет загрузку единичного объкта
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectDataCache"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		T Get<T>(ObjectDataCache<T> objectDataCache, object key) where T : class, new();

		/// <summary>
		/// Поиск множественных объектов по некоему запросу
		/// </summary>
		/// <param name="objectDataCache"></param>
		/// <param name="query"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IEnumerable<T> Find<T>(ObjectDataCache<T> objectDataCache, object query) where T : class, new();
	}
}