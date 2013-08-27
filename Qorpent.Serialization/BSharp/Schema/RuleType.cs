namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Типы правил схемы
	/// </summary>
	public enum RuleType {
		/// <summary>
		/// Разрешен
		/// </summary>
		Allow,
		/// <summary>
		/// Требуется
		/// </summary>
		Require,
		/// <summary>
		/// Устаревший
		/// </summary>
		Obsolete,
		/// <summary>
		/// Запрещенный
		/// </summary>
		Deny,
	}
}