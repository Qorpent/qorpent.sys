using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Qorpent.Model;

namespace Qorpent.Data{
	/// <summary>
	/// Базовый интерфейс адаптера
	/// </summary>
	/// <remarks>Нетипизированный, теоретически может возвращать значения разного типа</remarks>
	public interface IObjectDataAdapter{
		/// <summary>
		/// Обработать отдельную запись в наборе данных
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder">признак того, что используется исходный набор полей</param>
		/// <returns></returns>
		object ProcessRecordNative(IDataReader reader, bool nativeorder = false);

		/// <summary>
		/// Считывает набор данных и возвращает коллекцию типизированных элементов
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder"></param>
		/// <returns></returns>
		IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false);

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


	/// <summary>
	/// Интерфейс для пользовательского Orm адаптера
	/// </summary>
	public interface IObjectDataAdapter<T> : IObjectDataAdapter where T:class,new(){
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
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class ObjectDataAdapterBase<T>:IObjectDataAdapter<T> where T:class,new(){
		/// <summary>
		/// Обработать отдельную запись в наборе данных
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder">признак того, что используется исходный набор полей</param>
		/// <returns></returns>
		public abstract object ProcessRecordNative(IDataReader reader, bool nativeorder = false);

		/// <summary>
		/// Считывает набор данных и возвращает коллекцию типизированных элементов
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder"></param>
		/// <returns></returns>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false){
			while (reader.Read()){
				yield return ProcessRecordNative(reader, nativeorder);
			}
		}

		/// <summary>
		/// Подготавливет SQL скрипт выборки данных
		/// </summary>
		/// <param name="conditions">условия запроса для where (тип зависит от реализации)</param>
		/// <param name="hints">некии опции генерации (зависят от реализации)</param>
		/// <returns></returns>
		public abstract string PrepareSelectQuery(object conditions = null, object hints = null);

		///<summary>Implementation of GetTableName</summary>
		public abstract string GetTableName(object options = null);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder"></param>
		/// <returns></returns>
		public T ProcessRecord(IDataReader reader, bool nativeorder = false){
			return ProcessRecordNative(reader, nativeorder) as T;
		}

		/// <summary>
		/// Считывает набор данных и возвращает коллекцию типизированных элементов
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="nativeorder"></param>
		/// <returns></returns>
		public IEnumerable<T> ProcessRecordSet(IDataReader reader, bool nativeorder = false){
			return ProcessRecordSetNative(reader, nativeorder).OfType<T>().ToArray();
		}
	}
}