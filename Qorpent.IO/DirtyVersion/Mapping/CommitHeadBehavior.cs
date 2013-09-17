namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Поведение хида при коммите
	/// </summary>
	public enum CommitHeadBehavior {
		/// <summary>
		/// Автоматическое определение поведения
		/// </summary>
		Auto,
		/// <summary>
		/// Коммит не становится хидом, даже если он унаследован от хида
		/// </summary>
		Deny,
		/// <summary>
		/// Коммит в любом случае становится новым хидом независимо от наследования
		/// </summary>
		Direct,
		/// <summary>
		/// Текущий хид дописывается коммиту как источник и после этого он становится сам хидом
		/// </summary>
		Override,
	}
}