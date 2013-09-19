using System;

namespace Qorpent.IO {
	/// <summary>
	/// Запись о хэшированном файле
	/// </summary>
	public class HashedDirectoryRecord {
		/// <summary>
		/// /
		/// </summary>
		public HashedDirectoryRecord() {
			LastWriteTime = DateTime.MaxValue;
		}
		/// <summary>
		/// Хэш имени
		/// </summary>
		public string NameHash { get; set; }
		/// <summary>
		/// Хэш содержимого
		/// </summary>
		public string DataHash { get; set; }
		/// <summary>
		/// Время записи файла
		/// </summary>
		public DateTime LastWriteTime { get; set; }
	}
}