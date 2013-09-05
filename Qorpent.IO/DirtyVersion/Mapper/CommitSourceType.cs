namespace Qorpent.IO.DirtyVersion.Mapper {
	/// <summary>
	/// Тип источников коммита
	/// </summary>
	public enum CommitSourceType {
		/// <summary>
		/// Начальные коммиты
		/// </summary>
		Initial,
		/// <summary>
		/// От одного предка
		/// </summary>
		Single,
		/// <summary>
		/// Слияние
		/// </summary>
		Merged,
	}
}