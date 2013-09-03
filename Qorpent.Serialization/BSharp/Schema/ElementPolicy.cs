namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Политика поведения внутренних элементов
	/// </summary>
	public enum ElementPolicy {
		/// <summary>
		/// Все что не запрещено - разрешено
		/// </summary>
		Free,
		/// <summary>
		/// Все что не разрешено - запрещено
		/// </summary>
		Strict,
	}
}