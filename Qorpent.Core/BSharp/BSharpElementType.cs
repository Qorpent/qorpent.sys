namespace Qorpent.BSharp {
	/// <summary>
	///     Тип
	/// </summary>
	public enum BSharpElementType {
		/// <summary>
		///     Перезапись с ноля
		/// </summary>
		Define,

		/// <summary>
		///     Перезапись внутренних элементов
		/// </summary>
		Override,

		/// <summary>
		///     Расширение
		/// </summary>
		Extension,
	}
}