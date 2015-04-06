using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Server;

namespace Qorpent.Utils.Collections.BTreeIndex{
	/// <summary>
	/// Описание букета
	/// </summary>
	public class BTreeBucket<TKey, TKeyData, TData> : IBinarySerialize
		where TKey : IComparable{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj){
			if (null == obj) return false;
			if (!(obj is BTreeBucket<TKey, TKeyData, TData>)) return false;
			var b = obj as BTreeBucket<TKey, TKeyData, TData>;
			if (b.Id != Id) return false;
			if (b.Values.Count != Values.Count) return false;
			for (var i = 0; i < b.Values.Count; i++){
				var bv = b.Values[i];
				var tv = Values[i];
				if (!bv.Equals(tv)) return false;
			}
			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode(){
			return Values.Sum(_ => _.Key.GetHashCode()).GetHashCode() + Id.GetHashCode();
		}

		/// <summary>
		/// 
		/// </summary>
		public BTreeBucket(){
			Values = new List<BTreeValue<TKey, TKeyData, TData>>();
		}
		/// <summary>
		/// Признак корневого букета
		/// </summary>
		public bool IsRoot { get; set; }
		/// <summary>
		/// Обратная ссылка на индекс
		/// </summary>
		public BTreeIndex<TKey, TKeyData, TData> Index { get; set; }
		/// <summary>
		/// Идентификатор букета
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Родительский букет
		/// </summary>
		public BTreeBucket<TKey, TKeyData, TData> Parent { get; set; }

		/// <summary>
		/// Значения в составе букета
		/// </summary>
		public IList<BTreeValue<TKey, TKeyData, TData>> Values { get; set; } 
		/// <summary>
		/// Проверка заполненности узла - все элементы должны иметь значение 0
		/// </summary>
		public bool IsFull{
			get{

				return Values.Count>=Index.BucketSize;
			}
		}
		/// <summary>
		/// Проверка на то что букет листовой - нет никаких ссылко на дочерние букеты
		/// </summary>
		public bool IsLeaf{
			get { return Values.All(_ => _.BucketAfterId == 0 && _.BucketBeforeId == 0); }
		}
		/// <summary>
		/// Фиксирует изменения в букете
		/// </summary>
		public void FixChanges(){
			_changed = false;
		}
		/// <summary>
		/// Маркирует букет как измененный
		/// </summary>
		public void MarkChanged(){
			_changed = true;
		}

		private bool _changed = false;

		/// <summary>
		/// Признак изменения с момента последнего коммита или загрузки
		/// </summary>
		public bool Changed{
			get { return _changed; }
		}

		/// <summary>
		/// Универсальный метод привязки ключа к индексу с возвращением его значения
		/// встраивает элемент в индекс и опционально встраивает ссылку на факт в данные индекса
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="keydata"></param>
		/// <param name="data">optional - joined sub value ref to fact</param>
		/// <returns></returns>
		public void EnsureValue(TKey uid, TKeyData keydata = default(TKeyData), TData[] data = null){
			var existed = Values.FirstOrDefault(_ => _.Key.Equals(uid));
			if (null != existed){
				if (Index.SelfDataOnly && !keydata.Equals(default(TKeyData))){
					existed.Value = keydata;
				}
				else{
					if (null != data && 0 != data.Length){
						Index.AppendData(existed.Value, data);
					}
				}
				return;
			}
			if (IsLeaf){
				//если это лист, то в любом случае встаиваем
				bool wasfull = IsFull;
				StoreHere(uid, keydata, data);
				if (wasfull){
// если на момент присоединения лист уже был полон, то сплитим
					DoSplit();
				}
			}
			else{
				DownFallValue(uid,keydata, data);
			}
			
			
		}

		private void DownFallValue(TKey uid, TKeyData keydata, TData[] data){
			var start = 0;
			var end = Values.Count;
			var ix = (Values.Count - start) / 2;
			var current = Values[ix];
			while (true)
			{
				if (current.Key.Equals(uid)) return;
				if (current.Key.CompareTo(uid)>0)
				{
					if (0 == ix || (Values[ix - 1].Key.CompareTo(uid)<0))
					{
						StoreLeft(uid,keydata, data, current, ix);
						break;
					}
					end = ix;
				}
				if (current.Key.CompareTo(uid)<0)
				{
					if (Values.Count - 1 == ix || (Values[ix + 1].Key.CompareTo(uid)>0)){
						StoreRight(uid, keydata,data, current, ix);
						break;
					}
					start = ix;
				}
				ix = ((end - start) / 2) + start;
				current = Values[ix];
			}
		}

		private void StoreRight(TKey uid, TKeyData keydata, TData[] data, BTreeValue<TKey, TKeyData, TData> current, int ix)
		{
			if (0 == current.BucketAfterId){
				var newbucket = Index.CreateBucket();
				current.BucketAfterId = newbucket.Id;
				current.BucketAfter = newbucket;

				MarkChanged();
			}
			if (null == current.BucketAfter){
				current.BucketAfter = Index.LoadBucket(current.BucketAfterId);
			}
			if (Values.Count - 1 != ix){
				Values[ix + 1].BucketBeforeId = current.BucketAfterId;
				Values[ix + 1].BucketBefore = current.BucketAfter;
			}
			current.BucketAfter.EnsureValue(uid, keydata,data);
		}

		private void StoreLeft(TKey uid, TKeyData keydata, TData[] data, BTreeValue<TKey, TKeyData, TData> current, int ix)
		{
			if (0 == current.BucketBeforeId){
				var newbucket = Index.CreateBucket();
				current.BucketBeforeId = newbucket.Id;
				current.BucketBefore = newbucket;

				MarkChanged();
			}
			if (null == current.BucketBefore){
				current.BucketBefore = Index.LoadBucket(current.BucketBeforeId);
			}
			if (0 != ix){
				Values[ix - 1].BucketAfterId = current.BucketBeforeId;
				Values[ix - 1].BucketAfter = current.BucketBefore;
			}
			current.BucketBefore.EnsureValue(uid, keydata, data);
		}

		

		private void DoSplit(){
			if (null == Parent && !IsRoot){
				throw new Exception("illegal node state for splitting");
			}
			if (IsRoot || !Parent.IsFull){
				DoSplitUp();
			}
			else{
				DoSplitDown();
			}

		}

		private void DoSplitDown(){
			var mediana = Values[Index.BucketSize/2];
			var left = Index.CreateBucket();
			var right = Index.CreateBucket();
			for (var i = 0; i < Index.BucketSize/2; i++){
				left.Values.Add(Values[i]);
				Values[i].ContainingBucket = left;
			}
			for (var i = Index.BucketSize/2 + 1; i < Values.Count; i++){
				right.Values.Add(Values[i]);
				Values[i].ContainingBucket = right;
			}
			Values.Clear();
			Values.Add(mediana);
			mediana.ContainingBucket = this;
			mediana.BucketBefore = left;
			mediana.BucketBeforeId = left.Id;
			mediana.BucketAfter = right;
			mediana.BucketAfterId = right.Id;
			left.Parent = this;
			right.Parent = this;
			MarkChanged();
			right.MarkChanged();
			left.MarkChanged();
		}

		private void DoSplitUp(){
			MarkChanged();
			var mediana = Values[Index.BucketSize/2];
			mediana.ContainingBucket = null;
			mediana.BucketBeforeId = Id;
			mediana.BucketBefore = this;
			var rightBucket = Index.CreateBucket();
			rightBucket.MarkChanged();
			mediana.BucketAfterId = rightBucket.Id;
			mediana.BucketAfter = rightBucket;
			var rest = Values.Skip(Index.BucketSize/2 + 1).ToArray();
			if (mediana.Value == null){
				throw new Exception("here!!");
			}
			Values.Remove(mediana);
			foreach (var value in rest){
				rightBucket.Values.Add(value);
				value.ContainingBucket = rightBucket;
				Values.Remove(value);
			}
			BTreeBucket<TKey, TKeyData, TData> target = Parent;
			if (IsRoot){
				target = Index.CreateBucket();
				mediana.ContainingBucket = target;
				target.IsRoot = true;
				Index.RootBucket = target;
				Index.RootBucketId = target.Id;
				this.IsRoot = false;
				target.MarkChanged();
				target.Values.Add(mediana);
				Parent = target;
			}
			else{
				mediana.ContainingBucket = target;
				Parent.StoreValueAfterSplit(mediana);
			}
			rightBucket.Parent = target;
			target.MarkChanged();
		}

		/// <summary>
		/// Встраивает объект в родительский
		/// </summary>
		/// <param name="mediana"></param>
		private void StoreValueAfterSplit(BTreeValue<TKey, TKeyData, TData> mediana)
		{
			bool wasfull = IsFull;
			int insertIndex = -1;
			for (var i = 0; i < Values.Count; i++)
			{
				if ( Values[i].Key.CompareTo(mediana.Key)>0)
				{
					insertIndex = i;
					break;
				}
			}
			if (-1 == insertIndex)
			{
				insertIndex = Values.Count;
				Values.Add(mediana);
				
			}
			else
			{
				Values.Insert(insertIndex, mediana);
			}
			if (0 != insertIndex && Values[insertIndex - 1].BucketAfterId != mediana.BucketBeforeId){
				Values[insertIndex - 1].BucketAfterId = mediana.BucketBeforeId;
				Values[insertIndex - 1].BucketAfter = mediana.BucketBefore;
			}
			if (Values.Count-1 != insertIndex && Values[insertIndex + 1].BucketBeforeId != mediana.BucketAfterId)
			{
				Values[insertIndex + 1].BucketBeforeId = mediana.BucketAfterId;
				Values[insertIndex + 1].BucketBefore = mediana.BucketAfter;
			}

			if(wasfull)DoSplit();
		}
		/// <summary>
		/// 
		/// </summary>
		public int Depth{
			get { 
				if (IsRoot) return 0;
				return 1 + Parent.Depth;
			}
		}

		/// <summary>
		/// Метод встраивания значения в текущий букет - добавляет к набору если отсутствует и переупорядочивает
		/// </summary>
		/// <param name="uid"></param>
		/// <param name="keydata"></param>
		/// <param name="data"></param>
		private void StoreHere(TKey uid, TKeyData keydata , TData[] data ){


			BTreeValue<TKey, TKeyData, TData> value = null;
			if (Index.SelfDataOnly){
				value = new BTreeValue<TKey, TKeyData, TData>{Key = uid, Value = keydata};
			}
			else{
				value = new BTreeValue<TKey, TKeyData, TData>{Key = uid, Value = Index.CreateDataStorage()};
			}
			value.ContainingBucket = this;
			int insertIndex = -1;
			for (var i = 0; i < Values.Count; i++){
				if ( Values[i].Key.CompareTo(uid)>0){
					insertIndex = i;
					break;
				}
			}
			if (-1 == insertIndex){
				Values.Add(value);
			}
			else{
				Values.Insert(insertIndex, value);
			}
			MarkChanged();
			
			if (null!=data && 0 != data.Length){
				
					Index.AppendData(value.Value, data);
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return Id+" : "+string.Join("",Values.Select(_ => _.ToString()));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="uid"></param>
		/// <returns></returns>
		public BTreeValue<TKey, TKeyData, TData> Find(TKey uid)
		{
			var start = 0;
			var end = Values.Count;
			var ix = (Values.Count - start)/2;
			var current = Values[ix];
			while (true){
				if (current.Key.Equals(uid)) return current;
				if (current.Key.CompareTo(uid)>0){
					if (0 == ix || Values[ix - 1].Key.CompareTo(uid)<0){
						return ReturnFromLeft(uid, current);
					}
					end = ix;
				}
				if (current.Key.CompareTo(uid)<0){
					if (Values.Count - 1 == ix || Values[ix + 1].Key.CompareTo(uid)>0){
						return ReturnFromRight(uid, current);
					}
					start = ix;
				}
				ix = ((end - start) / 2) + start;
				current = Values[ix];
			}

		}

		private BTreeValue<TKey, TKeyData, TData> ReturnFromRight(TKey uid, BTreeValue<TKey, TKeyData, TData> current)
		{
			if (0 == current.BucketAfterId) return null;
			if (null == current.BucketAfter){
				current.BucketAfter = Index.LoadBucket(current.BucketAfterId);
			}
			return current.BucketAfter.Find(uid);
		}

		private BTreeValue<TKey, TKeyData, TData> ReturnFromLeft(TKey uid, BTreeValue<TKey, TKeyData, TData> current)
		{
			if (0 == current.BucketBeforeId) return null;
			if (null == current.BucketBefore){
				current.BucketBefore = Index.LoadBucket(current.BucketBeforeId);
			}
			return current.BucketBefore.Find(uid);
		}

		/// <summary>
		/// Generates a user-defined type (UDT) or user-defined aggregate from its binary form.
		/// </summary>
		/// <param name="r">The <see cref="T:System.IO.BinaryReader"/> stream from which the object is deserialized.</param>
		public void Read(BinaryReader r){
			Id = r.ReadInt32();
			var cnt = r.ReadInt32();
			BTreeValue<TKey, TKeyData, TData> last = null;
			BTreeValue<TKey, TKeyData, TData> current = null;
			for (var i = 0; i < cnt; i++){
				current = new BTreeValue<TKey, TKeyData, TData>{ContainingBucket = this};
				current.Read(r);
				current.BucketBeforeId = r.ReadInt32();

				if (0 != current.BucketBeforeId){
					current.BucketBefore = new BTreeBucket<TKey, TKeyData, TData>{Index = Index};
					current.BucketBefore.Read(r);
					current.BucketBefore.Parent = this;
					Index.PlainIndex[current.BucketBefore.Id] = current.BucketBefore;
				}

				if (null != last){
					last.BucketAfterId = current.BucketBeforeId;
					if (0 != current.BucketBeforeId){
						last.BucketAfter = current.BucketBefore;
					}
				}
				last = current;
				if (cnt - 1 == i){
					current.BucketAfterId = r.ReadInt32();
					if (0 != current.BucketAfterId){
						current.BucketAfter = new BTreeBucket<TKey, TKeyData, TData>();
						current.BucketAfter.Index = Index;
						current.BucketAfter.Read(r);
						current.BucketAfter.Parent = this;
						this.Index.PlainIndex[current.BucketAfter.Id] = current.BucketAfter;
					}
				}
				this.Values.Add(current);
			}
		}
		/// <summary>
		/// Записывает бина
		/// </summary>
		/// <param name="w"></param>
		public void Write(BinaryWriter w){
			w.Write(Id);
			w.Write(Values.Count);
			for (var i = 0; i < Values.Count; i++){
				var val = Values[i];
				val.Write(w);
				w.Write(val.BucketBeforeId);
				if (0 != val.BucketBeforeId){
					val.BucketBefore.Write(w);
				}
				if (Values.Count - 1 == i){
					w.Write(val.BucketAfterId);	
					if (0 != val.BucketAfterId){
						val.BucketAfter.Write(w);
					}
				}
			}
		}
	}
}