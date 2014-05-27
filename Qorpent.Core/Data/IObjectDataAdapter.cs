using System;
using System.Collections.Generic;
using System.Data;
using Qorpent.Model;

namespace Qorpent.Data{
	/// <summary>
	/// Интерфейс для пользовательского Orm адаптера
	/// </summary>
	public interface IObjectDataAdapter<T> where T:class,new(){
		/// <summary>
		/// Обработать отдельную запись в наборе данных
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder">признак того, что используется исходный набор полей</param>
		/// <returns></returns>
		T ProcessRecord(IDataReader reader, bool nativeorder = false);
		/// <summary>
		/// Считывает набор данных и возвращает коллекцию типизированных элементов
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder"></param>
		/// <returns></returns>
		IEnumerable<T> ProcessRecordSet(IDataReader reader, bool nativeorder = false);
		/// <summary>
		/// Подготавливет SQL скрипт выборки данных
		/// </summary>
		/// <param name="conditions">условия запроса для where (тип зависит от реализации)</param>
		/// <param name="hints">некии опции генерации (зависят от реализации)</param>
		/// <returns></returns>
		string PrepareSelectQuery(object conditions = null, object hints = null);

		///<summary>Implementation of GetTableName</summary>
		string GetTableName(object options = null);
	}
}