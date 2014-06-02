namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// Схема
	/// </summary>
	public class Schema : SqlObject{
		/// <summary>
		/// 
		/// </summary>
		public Schema()  {
			ObjectType = SqlObjectType.Schema;
			PreTable = true;
			LowerCase = true;
		
		}
	}
}