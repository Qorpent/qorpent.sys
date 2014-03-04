namespace Qorpent.Scaffolding.SqlGeneration{
	/// <summary>
	/// Режим создания объекта
	/// </summary>
	public enum DbObjectCreateType{
		/// <summary>
		/// Неопределенный
		/// </summary>
		None = 0,
		/// <summary>
		/// При отсутствии
		/// </summary>
		Ensure = 1,
		/// <summary>
		/// Пересоздание
		/// </summary>
		Create = 2,
	}
}