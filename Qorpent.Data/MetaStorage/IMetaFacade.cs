using System;
using System.Collections.Generic;
using Qorpent.Model;

namespace Qorpent.Data.MetaStorage {
	/// <summary>
	/// Общий фасад мета-репозитория
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IMetaFacade<T> where T:class,IWithId,IWithCode,new() {
		/// <summary>
		/// Возвращает строку по указанному критерию
		/// </summary>
		/// <param name="criteria">условие поиска - ID,Code,Query,Func</param>
		/// <returns></returns>
		T Get(object criteria);


		/// <summary>
		/// Возвращает строки по указанному критерию
		/// </summary>
		/// <param name="criteria">условие поиска - ID,Code,Query,Func</param>
		/// <param name="persistentCode">код для сохранения запроса в специальном кэше</param>
		/// <returns> </returns>
		IEnumerable<T> Select(Func<T,bool> criteria, string persistentCode = null);

		/// <summary>
		/// Возвращает строки по указанному критерию
		/// </summary>
		/// <param name="criteria">условие поиска - ID,Code,Query,Func</param>
		/// <param name="persistentCode">код для сохранения запроса в специальном кэше</param>
		/// <returns> </returns>
		IEnumerable<T> Select(object criteria, string persistentCode = null);

		/// <summary>
		/// Ссылка на локальное хранилище
		/// </summary>
		IMetaStorage<T> Storage { get; set; }
	}
}