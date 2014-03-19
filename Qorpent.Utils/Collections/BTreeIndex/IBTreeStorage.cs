using System;
using System.Collections.Generic;

namespace Qorpent.Utils.Collections.BTreeIndex
{
	/// <summary>
	/// 
	/// </summary>
	public interface IBTreeStorage<TKey,TKeyData,TData> 
		where TKey: IComparable{
		/// <summary>
		/// Получить идентификтор корневого узла
		/// </summary>
		/// <returns></returns>
		int GetRoot();
		/// <summary>
		/// Установить корневой узел
		/// </summary>
		/// <param name="uid"></param>
		void SetRoot(int uid);
		/// <summary>
		/// Создает букет и возвращает его идентификатор
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		int CreateBucket(int size);
		/// <summary>
		/// Загрузить букет
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="targetBucket"></param>
		void ReadBucket(int uid, BTreeBucket<TKey, TKeyData, TData> targetBucket);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="size"></param>
		/// <param name="targetBucket"></param>
		void WriteBucket(int uid, int size, BTreeBucket<TKey, TKeyData, TData> targetBucket);
		/// <summary>
		/// Возвращает новую страницу данных ключа c указанным размером - либо как корневую, либо как дополнительную
		/// </summary>
		/// <returns></returns>
		TKeyData GetNewDataPage(int size);
		/// <summary>
		/// Возвращает количество элементов в странице данных (с учетом списка перехода на дочерних)
		/// </summary>
		/// <param name="rootpageuid"></param>
		/// <returns></returns>
		int GetDataCount(int rootpageuid);

		/// <summary>
		/// Встраивает данные в индекс
		/// </summary>
		/// <param name="rootpageuid"></param>
		/// <param name="size"></param>
		/// <param name="factuids"></param>
		void StoreData(TKeyData rootpageuid, int size, TData[] factuids);
		/// <summary>
		/// 
		/// </summary>
		void Close();
		/// <summary>
		/// 
		/// </summary>
		void Flush();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="page"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		IEnumerable<TData> ReadData(TKeyData page,int size);
	}
}
