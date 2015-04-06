using System;
using System.IO;
using Microsoft.SqlServer.Server;

namespace Qorpent.Utils.Collections.BTreeIndex{
	/// <summary>
	/// Единичное значение в кортеже
	/// </summary>
	public class BTreeValue<TKey, TKeyData, TData> : IBinarySerialize
		where TKey : IComparable{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj){
			if (null == obj) return false;
			if (!(obj is BTreeValue<TKey, TKeyData, TData>)) return false;
			var v = obj as BTreeValue<TKey, TKeyData, TData>;

			return v.BucketBeforeId == this.BucketBeforeId
			       &&
			       v.BucketAfterId == this.BucketAfterId
			       &&
			       v.Key.Equals( this.Key)
			       &&
			       v.Value.Equals(this.Value);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			if (ContainingBucket.Values.IndexOf(this)==ContainingBucket.Values.Count-1){
				return string.Format("{0}<-'{1}'->{2}", BucketBeforeId, Key, BucketAfterId);	
			}
			return string.Format("{0}<-'{1}'->", BucketBeforeId, Key);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			return (BucketAfterId + BucketBeforeId + Key.GetHashCode() + Value.GetHashCode()).GetHashCode();
		}
		/// <summary>
		/// Ссылка на контейнерный букет
		/// </summary>
		public BTreeBucket<TKey, TKeyData, TData> ContainingBucket;

		/// <summary>
		/// Ссылка на предыдущий букет
		/// </summary>
		public int BucketBeforeId;

		/// <summary>
		/// Ссылка на следующий букет
		/// </summary>
		public int BucketAfterId;

		/// <summary>
		/// Ключ
		/// </summary>
		public TKey Key;

		/// <summary>
		/// Ссылка на область данных ключа
		/// </summary>
		public TKeyData Value;

		/// <summary>
		/// Предыдущий букет
		/// </summary>
		public BTreeBucket<TKey, TKeyData, TData> BucketBefore;
		/// <summary>
		/// Следующий букет
		/// </summary>
		public BTreeBucket<TKey, TKeyData, TData> BucketAfter;

		/// <summary>
		/// Generates a user-defined type (UDT) or user-defined aggregate from its binary form.
		/// </summary>
		/// <param name="r">The <see cref="T:System.IO.BinaryReader"/> stream from which the object is deserialized.</param>
		public void Read(BinaryReader r){
			Key = BTreeUtils.Read<TKey>(r);
			Value = BTreeUtils.Read<TKeyData>(r);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="w"></param>
		public void Write(BinaryWriter w){
			BTreeUtils.Write(w,Key);
			BTreeUtils.Write(w,Value);
		}
	}
}