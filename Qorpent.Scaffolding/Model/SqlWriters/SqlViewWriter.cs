using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class SqlViewWriter : SqlCommandWriter
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="view"></param>
		public SqlViewWriter(SqlView view)
		{
			this.View = view;
			this.Parameters = view;
		}
		/// <summary>
		/// 
		/// </summary>
		public SqlView View { get; set; }
	}
}