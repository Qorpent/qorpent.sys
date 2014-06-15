namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	///     Обертка над SQL видом
	/// </summary>
	public class SqlView : SqlObject{
		/// <summary>
		/// </summary>
		public SqlView(){
			UseTablePrefixedName = true;
		}
	}
}