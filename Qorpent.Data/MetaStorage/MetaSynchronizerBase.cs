#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Data/MetaSynchronizerBase.cs
#endregion
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