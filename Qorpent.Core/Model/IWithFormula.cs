namespace Qorpent.Model {
	/// <summary>
	/// 	Модельный интерфейс - что-то с формулой
	/// </summary>
	public interface IWithFormula
	{
		/// <summary>
		/// Строка формулы
		/// </summary>
		string Formula { get; set; }
		/// <summary>
		/// Тип формулы
		/// </summary>
		string FormulaType { get; set; }
		/// <summary>
		/// Признак активности формулы
		/// </summary>
		string IsFormula { get; set; }
	}
}