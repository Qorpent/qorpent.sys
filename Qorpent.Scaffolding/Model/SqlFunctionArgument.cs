namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public class SqlFunctionArgument{
		/// <summary>
		/// Порядок в вызове
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Название аргумента
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Тип данных
		/// </summary>
		public DataType DataType { get; set; }

		/// <summary>
		/// Значение по умолчанию
		/// </summary>
		public DefaultValue DefaultValue { get; set; }

		/// <summary>
		/// Возвращаемый аргумент
		/// </summary>
		public bool IsOutput { get; set; }
	}
}