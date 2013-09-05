using System;

namespace Qorpent.IO.DirtyVersion.Mapper {
	/// <summary>
	/// Информация о коммитере и дате коммита
	/// </summary>
	public class CommitAuthorInfo {
		/// <summary>
		/// Автор
		/// </summary>
		public string Commiter { get; set; }
		/// <summary>
		/// Время записи
		/// </summary>
		public DateTime Time { get; set; }
	}
}