using System;
using System.Collections.Generic;

namespace Qorpent.Utils.Collections.BTreeIndex{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TKeyData"></typeparam>
	/// <typeparam name="TData"></typeparam>
	public class StubBTreeStorage<TKey, TKeyData, TData> : IBTreeStorage<TKey, TKeyData, TData> where TKey : IComparable{
		private int _id=2;
		private int _root = 1;

		/// <summary>
		/// Получить идентификтор корневого узла
		/// </summary>
		/// <returns></returns>
		public int GetRoot(){
			return _root;
		}

		/// <summary>
		/// Установить корневой узел
		/// </summary>
		/// <param name="uid"></param>
		public void SetRoot(int uid){
			_root = uid;
		}

		/// <summary>
		/// Создает букет и возвращает его идентификатор
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public int CreateBucket(int size){
			return _id++;
		}

		/// <summary>
		/// Загрузить букет
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="targetBucket"></param>
		public void ReadBucket(int uid, BTreeBucket<TKey, TKeyData, TData> targetBucket){
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="size"></param>
		/// <param name="targetBucket"></param>
		public void WriteBucket(int uid, int size, BTreeBucket<TKey, TKeyData, TData> targetBucket){
			
		}

		/// <summary>
		/// Возвращает новую страницу данных ключа c указанным размером - либо как корневую, либо как дополнительную
		/// </summary>
		/// <returns></returns>
		public TKeyData GetNewDataPage(int size){
			if (typeof (TKeyData) == typeof (int)){
				return (TKeyData)(object)_id++;
			}
			return default(TKeyData);
		}

		/// <summary>
		/// Возвращает количество элементов в странице данных (с учетом списка перехода на дочерних)
		/// </summary>
		/// <param name="rootpageuid"></param>
		/// <returns></returns>
		public int GetDataCount(int rootpageuid){
			return 0;
		}

		/// <summary>
		/// Встраивает данные в индекс
		/// </summary>
		/// <param name="rootpageuid"></param>
		/// <param name="size"></param>
		/// <param name="factuids"></param>
		public void StoreData(TKeyData rootpageuid, int size, TData[] factuids){
			
		}

		/// <summary>
		/// 
		/// </summary>
		public void Close(){
			
		}

		/// <summary>
		/// 
		/// </summary>
		public void Flush(){
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public IEnumerable<TData> ReadData(TKeyData page, int size){
			yield break;
		}
	}
}