using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Data.DataCache{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ObjectDataCacheBindLazyList<T> :IList<T>,ILazyCollection<T> where T : class, new(){
		private bool _wasLoaded;
		private List<T> InternalList;
		/// <summary>
		/// Запрос к кэшу
		/// </summary>
		public string Query { get; set; }
		/// <summary>
		/// Связанный кэш
		/// </summary>
		public ObjectDataCache<T> Cache { get; set; }

		/// <summary>
		/// Признак того, что коллекция уже загрузилась
		/// </summary>
		public bool WasLoaded{
			get { return _wasLoaded; }
			private set { _wasLoaded = value; }
		}

		/// <summary>
		/// Выполнение загрузки коллекции
		/// </summary>
		public void Load(){
			if(_wasLoaded)return;
			InternalList = new List<T>();
			lock (this){
				foreach (var item in Cache.GetAll(Query))
				{
					this.InternalList.Add(item);
				}
				_wasLoaded = true;	
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator(){
			Load();
			return InternalList.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator(){
			Load();
			return InternalList.GetEnumerator();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item){
			Load();
			InternalList.Add(item);
		}
		/// <summary>
		/// 
		/// </summary>
		public void Clear(){
			Load();
			InternalList.Clear();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item){
			Load();
			return InternalList.Contains(item);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex){
			Load();
			InternalList.CopyTo(array,arrayIndex);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item){
			Load();
			return InternalList.Remove(item);
		}
		/// <summary>
		/// 
		/// </summary>
		public int Count { get{
			Load();
			return InternalList.Count;
		} }
		/// <summary>
		/// 
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item){
			Load();
			return InternalList.IndexOf(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, T item){
			Load();
			InternalList.Insert(index,item);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index){
			Load();
			InternalList.RemoveAt(index);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]{
			get{
				Load();
				return InternalList[index];
			}
			set{
				Load();
				InternalList[index] = value;
			}
		}
	}
}