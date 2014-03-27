using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Запись о плановом обновлении
	/// </summary>
	public class DatabaseUpdateRecord{
		private static int __Id = 1;
		/// <summary>
		/// 
		/// </summary>
		public  int Id = __Id++;

		private string _targetCode;
		private int _targetId = Int32.MinValue; 

		/// <summary>
		/// Целевая таблица
		/// </summary>
		public string TargetTable { get; set; }
		
		/// <summary>
		/// 
		/// </summary>
		public string FullTableName{
			get { return (String.IsNullOrWhiteSpace(Schema) ? "dbo" : Schema) + "." + TargetTable; }
		}
		/// <summary>
		/// Ссылка на файл, с зарегистрированной дельтой
		/// </summary>
		public MetaFileRegistryDelta FileDelta { get; set; }
		/// <summary>
		/// Информация о выявленной разнице в файле
		/// </summary>
		public XDiffItem DiffItem { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int TargetId
		{
			get
			{
				if (Int32.MinValue == _targetId)
				{
					if (null != DiffItem){
						var e = DiffItem.BasisElement ?? DiffItem.NewestElement;
						_targetId = e.Attr("id").ToInt();
					}
					else{
						_targetId = 0;
					}
				}
				return _targetId;
			}
			set { _targetId = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		public string TargetCode{
			get{
				if (null == _targetCode){
					if (null != DiffItem){
						var e = DiffItem.BasisElement ?? DiffItem.NewestElement;
						_targetCode = e.Attr("code");
					}
					else{
						_targetCode = "";
					}
				}
				return _targetCode;
			}
			set { _targetCode = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool IsSameObject(DatabaseUpdateRecord record){
			if (record.FullTableName != FullTableName) return false;
			if (record.TargetId != 0 && this.TargetId != 0 && record.TargetId != this.TargetId) return false;
			if (!String.IsNullOrWhiteSpace(record.TargetCode) && !String.IsNullOrWhiteSpace(TargetCode) &&
			    record.TargetCode != this.TargetCode) return false;
			if (!(record.TargetId == this.TargetId || record.TargetCode == this.TargetCode)) return false;
			return true;
		}

		/// <summary>
		/// Sql комманда, запланированная к выполнению
		/// </summary>
		public string SqlCommand { get; set; }
		/// <summary>
		/// Схема в таблице данных
		/// </summary>
		public string Schema { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Error { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string ErrorCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FullCode{
			get { return FullTableName + "." + TargetId + "." + TargetCode; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public DatabaseUpdateRecord Copy(){
			var result = MemberwiseClone() as DatabaseUpdateRecord;
			result.Id = __Id++;
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (!String.IsNullOrWhiteSpace(ErrorCode) || !String.IsNullOrWhiteSpace(Error)){
				return String.Format("error: {0} -- {1}", ErrorCode, Error);
			}
			var code = FileDelta == null ? "NOFILE" : FileDelta.Code;
			var rev = FileDelta == null
				          ? "NOREV"
				          : FileDelta.SourceHistory.OrderByDescending(_ => _.RevisionTime).First().Revision;
			return String.Format("{0}->{1} {2} {3}\r\n{4}", code, rev, DiffItem.Action, FullCode, new[]{DiffItem}.LogToString());

		}

		/// <summary>
		/// 
		/// </summary>
		private class ObjectMatcher:IEqualityComparer<DatabaseUpdateRecord>{
			/// <summary>
			/// /
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(DatabaseUpdateRecord x, DatabaseUpdateRecord y){
				return x.IsSameObject(y);
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(DatabaseUpdateRecord obj){
				return 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static IDictionary<ObjectKey,LinkedList<DatabaseUpdateRecord>> Group(IEnumerable<DatabaseUpdateRecord> source){
			return source.GroupBy(_ => _, new ObjectMatcher())
				.Select(_ => new LinkedList<DatabaseUpdateRecord>(_))
				.ToDictionary(GetObjectKey,_=>_);
		}
		/// <summary>
		/// Возвращает совокупный ключ объекта
		/// </summary>
		/// <param name="databaseUpdateRecords"></param>
		/// <returns></returns>
		public static ObjectKey GetObjectKey(IEnumerable<DatabaseUpdateRecord> databaseUpdateRecords){
			var result = new ObjectKey();
			foreach (var databaseUpdateRecord in databaseUpdateRecords){
				result.Table = databaseUpdateRecord.TargetTable;
				if (0 != databaseUpdateRecord.TargetId){
					result.Id = databaseUpdateRecord.TargetId;
				}
				if (!string.IsNullOrWhiteSpace(databaseUpdateRecord.TargetCode)){
					result.Code = databaseUpdateRecord.TargetCode;
				}
			}
			return result;
		}
	}
	/// <summary>
	/// Структура привязки к объекту
	/// </summary>
	public struct ObjectKey{
		/// <summary>
		/// Таблица
		/// </summary>
		public string Table { get; set; }
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Код
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return string.Format("{0}.{1}.{2}", Table, Id, Code);

		}
	}
}