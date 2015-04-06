namespace Qorpent.Utils.Git{
	/// <summary>
	/// Перечисление стратегий
	/// </summary>
	public enum MergeStrategyOption{
		/// <summary>
		/// Нет статегии
		/// </summary>
		None,
		/// <summary>
		/// Побеждает цель
		/// </summary>
		Ours,
		/// <summary>
		/// Пбеждает источник
		/// </summary>
		Theirs,
		/// <summary>
		/// Углубленный мерж
		/// </summary>
		Patience
	}
}