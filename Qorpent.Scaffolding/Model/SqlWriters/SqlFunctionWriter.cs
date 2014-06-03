namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class SqlFunctionWriter : SqlCommandWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="function"></param>
		public SqlFunctionWriter(SqlFunction function)
		{
			this.Function = function;
			this.Parameters = function;
		}
		/// <summary>
		/// 
		/// </summary>
		public SqlFunction Function { get; set; }
	}
}