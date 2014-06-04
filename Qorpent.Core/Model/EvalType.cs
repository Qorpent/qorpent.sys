namespace Qorpent.Model{
	/// <summary>
	/// Тип расчетов
	/// </summary>
	public enum EvalType{
		/// <summary>
		/// Без расчетов, всегда 0
		/// </summary>
		None = 0,
		/// <summary>
		/// Первичное значение
		/// </summary>
		Primary = 1,
		/// <summary>
		/// Суммовое значение
		/// </summary>
		Sum = 2,
		/// <summary>
		/// Суммы по особым условиям обхода
		/// </summary>
		ReferencedSum = 3,
		/// <summary>
		/// Формула (через компиляцию из стандартной формулы)
		/// </summary>
		Formula = 4,
		/// <summary>
		/// Специальный пользовательский
		/// </summary>
		Custom = 5,
	}
}