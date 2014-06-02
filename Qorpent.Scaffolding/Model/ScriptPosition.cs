namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// Позиция скрипта, относительно базового
	/// </summary>
	public enum ScriptPosition{
		/// <summary>
		/// До типового скрипта
		/// </summary>
		Before,
		/// <summary>
		/// Перед таблицами, но после подготовительных объектов
		/// </summary>
		BeforeTables,
		/// <summary>
		/// После  таблиц но до остальных объектов
		/// </summary>
		AfterTables,
		/// <summary>
		/// После типового скрипта
		/// </summary>
		After,
	}
}