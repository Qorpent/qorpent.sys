 namespace Qorpent.Mvc.Actions.Helpers {
	/// <summary>
	/// Ёлемент списка
	/// </summary>
	public class FileListEntry {
		/// <summary>
		/// “ип элемента
		/// </summary>
		public FileListEntryType Type { get; set; }

		/// <summary>
		/// Ћокальный путь
		/// </summary>
		public string LocalPath { get; set; }
	}
}