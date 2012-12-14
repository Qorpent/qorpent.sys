using System;
using System.Collections.Generic;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Abstract meta storage interface
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaStorage<T> where T : class, IWithId, IWithCode, new() {
		/// <summary>
		/// Кэш ID - сущность
		/// </summary>
		IDictionary<int, T> IdCache { get; }

		/// <summary>
		/// Кэш CODE - сущность
		/// </summary>
		IDictionary<string, T> CodeCache { get; }

		/// <summary>
		/// Кэш запрос - набор сущностей
		/// </summary>
		IDictionary<string, T[]> QueryCache { get; }

		/// <summary>
		/// Время поcледней синхронизации
		/// </summary>
		DateTime LastSyncTime { get; set; }

		/// <summary>
		/// True - если объект заблокирован на запись
		/// </summary>
		bool IsInWriteState { get; }

		/// <summary>
		/// True - если по объекту сейчас выполняются операции чтения
		/// </summary>
		bool IsInReadState { get; }

		/// <summary>
		/// Словарь пользовательских опций
		/// </summary>
		IDictionary<string, string> Options { get; }

		/// <summary>
		/// Получить объект синхронизации на запись
		/// </summary>
		/// <returns></returns>
		IDisposable EnterWrite();

		/// <summary>
		/// Отпустить объект синхронизации на запись (должен быть в try/finally) - монопольный
		/// </summary>
		/// <summary>
		/// Получить объект синхронизации на чтение (должен быть в try/finally) - конкурентный
		/// </summary>
		/// <returns></returns>
		IDisposable EnterRead();

		/// <summary>
		/// Вызывается после первичной загрузки кэша из БД
		/// </summary>
		void AfterInitialLoad();

		/// <summary>
		/// Вызывается после частичной загрузки кэша из БД
		/// </summary>
		/// <param name="updatedItems"></param>
		void AfterUpdate(T[] updatedItems);

		/// <summary>
		/// Очистка хранилища
		/// </summary>
		void Clear();

		/// <summary>
		/// Со
		/// </summary>
		/// <returns></returns>
		IMetaSynchronizer<T> GetSynchronizer();

		/// <summary>
		/// Создает новые экземпляр стандартного фасада
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		IMetaFacade<T> CreateFacade();
	}
}