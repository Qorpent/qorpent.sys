using System;

namespace Qorpent.IO.DirtyVersion.Mapper {
	/// <summary>
	/// Запись в журнал
	/// </summary>
	public class MapRecord {
		/// <summary>
		/// Имя исходного объекта
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Рассчитанный хэш имени
		/// </summary>
		public string NameHash { get; set; }
		/// <summary>
		/// Время создания нового объекта
		/// </summary>
		public DateTime VersionTime { get; set; }
		/// <summary>
		/// Новый хэш
		/// </summary>
		public string NewDataHash { get; set; }
		/// <summary>
		/// Коммитер
		/// </summary>
		public string Commiter { get; set; }
		/// <summary>
		/// Исходный хэш
		/// </summary>
		public string[] SourceDataHashes { get; set; }
		/// <summary>
		/// Признак, что обновленная запись - является действительно текущей для репозитория
		/// </summary>
		public bool IsHead { get; set; }
	}
}