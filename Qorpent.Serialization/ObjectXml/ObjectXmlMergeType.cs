namespace Qorpent.ObjectXml {
	/// <summary>
	///     Тип
	/// </summary>
	public enum ObjectXmlMergeType {
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