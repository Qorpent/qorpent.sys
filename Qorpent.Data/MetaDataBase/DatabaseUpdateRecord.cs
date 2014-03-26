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
		/// <summary>
		/// Целевая таблица
		/// </summary>
		public string TargetTable { get; set; }
		/// <summary>
		/// Целевой ID
		/// </summary>
		public int TargetId { get; set; }
		/// <summary>
		/// Целевой код
		/// </summary>
		public string TargetCode { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string FullTableName{
			get { return (string.IsNullOrWhiteSpace(Schema) ? "dbo" : Schema) + "." + TargetTable; }
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
	}
}