using System;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Синхронизатор локального кэша с БД
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaSynchronizer<T> where T:class ,new() {

		/// <summary>
		/// Целевое хранилище
		/// </summary>
		IMetaStorage<T> Storage { get; set; } 
		/// <summary>
		/// Первичная загрузка
		/// </summary>
		void Load();

		/// <summary>
		/// Проверяет наличие обновлений
		/// </summary>
		/// <returns></returns>
		DateTime GetLastVersion();

		/// <summary>
		/// Обновляет изменения из БД в кэш
		/// </summary>
		void Update();

		/// <summary>
		/// Запускает процесс автоматического мониторинга с указанной периодичностью
		/// </summary>
		/// <param name="seconds"></param>
		void StartAutoUpdate(int seconds);

		/// <summary>
		/// Прерывает процесс автоматического мониторинга
		/// </summary>
		void StopAutoUpdate();
	}
}