using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Запись о плановом обновлении
	/// </summary>
	public class DatabaseUpdateRecord{
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

	}
}