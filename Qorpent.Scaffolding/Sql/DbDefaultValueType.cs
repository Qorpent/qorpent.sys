namespace Qorpent.Scaffolding.SqlGeneration{
	/// <summary>
	/// Тип значения по умолчанию
	/// </summary>
	public enum DbDefaultValueType{
		/// <summary>
		/// Неопределенный
		/// </summary>
		None,
		/// <summary>
		/// Прямой
		/// </summary>
		Native,
		/// <summary>
		/// Заключать  в строку
		/// </summary>
		String,
		/// <summary>
		/// Выражение
		/// </summary>
		Expression
	}
}