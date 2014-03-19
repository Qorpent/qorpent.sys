using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;

namespace Qorpent.Utils.Collections.BTreeIndex{
	/// <summary>
	/// 
	/// </summary>
	public class BTreeIndex<TKey, TKeyData, TData> : IBinarySerialize
		where TKey : IComparable
	
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="bucketSize"></param>
		public BTreeIndex(int bucketSize = 0){
			Storage= new StubBTreeStorage<TKey, TKeyData, TData>();
			BucketSize = DEFAULT_BUCKET_SIZE;
			if (0 < bucketSize){
				BucketSize = bucketSize;
			}
			DataPageSize = DEFAULT_DATA_SIZE;
			PlainIndex = new Dictionary<int, BTreeBucket<TKey, TKeyData, TData>>();
		}
	

		/// <summary>
		/// Размер букета по умолчанию
		/// </summary>
		public const int DEFAULT_BUCKET_SIZE = 39; //value was evaluated with experiment
		/// <summary>
		/// Размер страницы кортежа на ссылки на факты
		/// </summary>
		public const int DEFAULT_DATA_SIZE = 2;
		/// <summary>
		/// 
		/// </summary>
		public int BucketSize { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public int DataPageSize { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public bool SelfDataOnly { get; set; }

		/// <summary>
		/// Хранилище индекса
		/// </summary>
		public IBTreeStorage<TKey, TKeyData, TData> Storage { get; set; }
		/// <summary>
		/// Плоское отображение индекса
		/// </summary>
		public IDictionary<int, BTreeBucket<TKey, TKeyData, TData>> PlainIndex { get; private set; }
		/// <summary>
		/// Идентификатор корневого букета
		/// </summary>
		public int RootBucketId { get; set; }
		/// <summary>
		/// Загруженный объект корневого букета
		/// </summary>
		public BTreeBucket<TKey, TKeyData, TData> RootBucket { get; set; }

		/// <summary>
		/// Выстраивает индекс относительно uid
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="selfdata"></param>
		/// <param name="factuids"></param>
		public void EnsureValue(TKey uid, TKeyData selfdata= default(TKeyData), TData[] factuids = null){
			if(null==Storage)throw new Exception("cannot work without storage");
			CheckRoot();
			RootBucket.EnsureValue(uid,selfdata,factuids);
		}

		private void CheckRoot(){
			if (null == RootBucket){
				if (0 == RootBucketId){
					RootBucketId = Storage.GetRoot();
				}
				if (0 == RootBucketId){
					RootBucketId = Storage.CreateBucket(BucketSize);
				}
				RootBucket = LoadBucket(RootBucketId);
				RootBucket.IsRoot = true;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public TKeyData CreateDataStorage(){
			if(null==Storage)throw new Exception("no storage configured");
			return Storage.GetNewDataPage(DataPageSize);
		}
		/// <summary>
		/// Присоединить данные к узлу
		/// </summary>
		/// <param name="dataRef"></param>
		/// <param name="data"></param>
		public void AppendData(TKeyData dataRef, TData[] data){
			if(null==Storage)throw new Exception("no storage configured");
			Storage.StoreData(dataRef,DataPageSize,data);
		}
		/// <summary>
		/// 
		/// </summary>
		public void Flush(){
			if(null==Storage)throw new Exception("no storage configured");
			var changes = PlainIndex.Values.Where(_ => _.Changed).ToArray();
			if (changes.Length != 0){
				foreach (var bucket in changes){
					if (bucket.IsRoot){
						Storage.SetRoot(bucket.Id);
					}
					Storage.WriteBucket(bucket.Id,BucketSize,bucket);
					bucket.FixChanges();
				}
				Storage.Flush();
			}
		}
		/// <summary>
		/// Создает новый букет
		/// </summary>
		/// <returns></returns>
		public BTreeBucket<TKey, TKeyData, TData> CreateBucket()
		{
			if (null == Storage) throw new Exception("no storage configured");
			var uid = Storage.CreateBucket(BucketSize);
			var bucket = new BTreeBucket<TKey, TKeyData, TData> { Id = uid, Index = this };
			PlainIndex[uid] = bucket;
			return bucket;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public BTreeBucket<TKey, TKeyData, TData> LoadBucket(int id)
		{
			if (null == Storage) throw new Exception("no storage configured");
			var bucket = new BTreeBucket<TKey, TKeyData, TData> { Id = id, Index = this };
			PlainIndex[id] = bucket;
			Storage.ReadBucket(id,bucket);
			return bucket;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		public BTreeValue<TKey, TKeyData, TData> Find(TKey uid)
		{
			CheckRoot();
			return RootBucket.Find(uid);
		}
		/// <summary>
		/// Считывает список ID фактов по ключу
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		public IEnumerable<TData> GetData(TKey uid){
			if (null == Storage) throw new Exception("no storage configured");
			var val = Find(uid);
			if(null==val)yield break;
			foreach (var res in Storage.ReadData(val.Value,DataPageSize)){
				yield return res;
			}
		}

		/// <summary>
		/// Generates a user-defined type (UDT) or user-defined aggregate from its binary form.
		/// </summary>
		/// <param name="r">The <see cref="T:System.IO.BinaryReader"/> stream from which the object is deserialized.</param>
		public void Read(BinaryReader r){
			PlainIndex.Clear();
			RootBucket = new BTreeBucket<TKey, TKeyData, TData>{Index = this};
			RootBucket.Read(r);
			RootBucketId = RootBucket.Id;
			PlainIndex[RootBucketId] = RootBucket;
		}

		/// <summary>
		/// Converts a user-defined type (UDT) or user-defined aggregate into its binary format so that it may be persisted.
		/// </summary>
		/// <param name="w">The <see cref="T:System.IO.BinaryWriter"/> stream to which the UDT or user-defined aggregate is serialized.</param>
		public void Write(BinaryWriter w){
			if(null==RootBucket)return;
			RootBucket.Write(w);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="base64index"></param>
		/// <returns></returns>
		public BTreeIndex<TKey, TKeyData, TData> Load(string base64index){
			using (var s = new MemoryStream(Convert.FromBase64String(base64index))){
				using (var r = new BinaryReader(s, Encoding.UTF8)){
					Read(r);
				}
			}
			return this;
		}
	}
}