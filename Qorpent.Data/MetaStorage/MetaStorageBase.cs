using System;
using System.Collections.Generic;
using System.Threading;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Базовое мета-хранилище
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MetaStorageBase<T> : ServiceBase,IMetaStorage<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// Счетчик текущих read- активностей
		/// </summary>
		protected int ReadCount;

		/// <summary>
		/// Блокировка на чтение
		/// </summary>
		protected object ReadLock = new object();

		/// <summary>
		/// Блокировка на запись
		/// </summary>
		protected object WriteLock = new object();

		/// <summary>
		/// 
		/// </summary>
		public MetaStorageBase() {
			IdCache = new Dictionary<int, T>();
			CodeCache = new Dictionary<string, T>();
			QueryCache = new Dictionary<string, T[]>();
			Options = new Dictionary<string, string>();
		}

		

		/// <summary>
		/// Вспомогательный класс для синхронизации на чтение
		/// </summary>
		/// <typeparam name="TIt"></typeparam>
		protected class ReadHandler<TIt> :IDisposable where TIt : class, IWithId, IWithCode, new() {
			private MetaStorageBase<TIt> _parent;
			/// <summary>
			/// </summary>
			/// <param name="parent"></param>
			public ReadHandler(MetaStorageBase<TIt> parent ) {
				_parent = parent;
				
				lock (parent.WriteLock) {
					
					if(!Monitor.IsEntered(_parent.ReadLock)) {
						Monitor.Enter(_parent.ReadLock);
					}
					_parent.ReadCount++;
				}
			}

			/// <summary>
			/// Выполняет определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose() {
				_parent.ReadCount--;
				if(0==_parent.ReadCount) {
					Monitor.Exit(_parent.ReadLock);
				}
			}
		}

		/// <summary>
		/// Вспомогательный класс для синхронизации на чтение
		/// </summary>
		/// <typeparam name="TIt"></typeparam>
		protected class WriteHandler<TIt> : IDisposable where TIt : class, IWithId, IWithCode, new()
		{
			private MetaStorageBase<TIt> _parent;
			/// <summary>
			/// /
			/// </summary>
			/// <param name="parent"></param>
			public WriteHandler(MetaStorageBase<TIt> parent)
			{
				_parent = parent;
				Monitor.Enter(_parent.WriteLock);
				Monitor.Enter(_parent.ReadLock);
				_parent.IsInWriteState = true;
			}

			/// <summary>
			/// Выполняет определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов.
			/// </summary>
			/// <filterpriority>2</filterpriority>
			public void Dispose() {
				_parent.IsInWriteState = false;
				Monitor.Exit(_parent.ReadLock);
				Monitor.Exit(_parent.WriteLock);
			}
		}

		/// <summary>
		/// Кэш ID - сущность
		/// </summary>
		public IDictionary<int, T> IdCache { get; private set; }

		/// <summary>
		/// Кэш CODE - сущность
		/// </summary>
		public IDictionary<string, T> CodeCache { get; private set; }

		/// <summary>
		/// Кэш запрос - набор сущностей
		/// </summary>
		public IDictionary<string, T[]> QueryCache { get; private set; }

		/// <summary>
		/// Время поcледней синхронизации
		/// </summary>
		public DateTime LastSyncTime { get; set; }

		/// <summary>
		/// True - если объект заблокирован на запись
		/// </summary>
		public bool IsInWriteState { get; protected set; }

		/// <summary>
		/// True - если по объекту сейчас выполняются операции чтения
		/// </summary>
		public bool IsInReadState { get { return ReadCount != 0; } }

		/// <summary>
		/// Словарь пользовательских опций
		/// </summary>
		public IDictionary<string, string> Options { get; private set; }

		/// <summary>
		/// Получить объект синхронизации на запись
		/// </summary>
		/// <returns></returns>
		public IDisposable EnterWrite() {
			return new WriteHandler<T>(this);
		}

		/// <summary>
		/// Отпустить объект синхронизации на запись (должен быть в try/finally) - монопольный
		/// </summary>
		/// <summary>
		/// Получить объект синхронизации на чтение (должен быть в try/finally) - конкурентный
		/// </summary>
		/// <returns></returns>
		public IDisposable EnterRead() {
			return new ReadHandler<T>(this);
		}

		/// <summary>
		/// Вызывается после первичной загрузки кэша из БД
		/// </summary>
		public void AfterInitialLoad() {
			
		}

		/// <summary>
		/// Вызывается после частичной загрузки кэша из БД
		/// </summary>
		/// <param name="updatedItems"></param>
		public void AfterUpdate(T[] updatedItems) {
			
		}

		/// <summary>
		/// Очистка хранилища
		/// </summary>
		public void Clear() {
			using(EnterWrite()) {
				IdCache.Clear();
				CodeCache.Clear();
				QueryCache.Clear();
			}
		}

		/// <summary>
		/// Создает новые экземпляр стандартного фасада
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public virtual IMetaFacade<T> CreateFacade() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Со
		/// </summary>
		/// <returns></returns>
		public virtual IMetaSynchronizer<T> GetSynchronizer() {
			throw new NotImplementedException();
		}
	}
}