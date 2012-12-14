using System;
using System.Threading;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MetaSynchronizerBase<T> :ServiceBase, IMetaSynchronizer<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// Целевое хранилище
		/// </summary>
		public IMetaStorage<T> Storage { get; set; }

		/// <summary>
		/// Первичная загрузка
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// Проверяет наличие обновлений
		/// </summary>
		/// <returns></returns>
		public abstract DateTime GetLastVersion();

		/// <summary>
		/// Обновляет изменения из БД в кэш
		/// </summary>
		public abstract void Update();
		/// <summary>
		/// Вспомогательный метод обновления по расписанию
		/// </summary>
		/// <param name="seconds"></param>
		protected void DoAutoUpdate(int seconds) {
			Thread.Sleep(TimeSpan.FromSeconds(seconds));
			if (autoupdate)
			{
				if(Storage.LastSyncTime< GetLastVersion()) {
					Update();
				}
				StartAutoUpdate(seconds);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		protected bool autoupdate = false;

		/// <summary>
		/// Запускает процесс автоматического мониторинга с указанной периодичностью
		/// </summary>
		/// <param name="seconds"></param>
		public void StartAutoUpdate(int seconds = 60) {
			lock (this) {
				autoupdate = true;
				ThreadPool.QueueUserWorkItem(t => DoAutoUpdate((int) t), seconds);
			}
			
		}

		/// <summary>
		/// Прерывает процесс автоматического мониторинга
		/// </summary>
		public void StopAutoUpdate() {
			lock(this) {
				autoupdate = false;
			}
		}
	}
}