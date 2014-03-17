namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Вид, представление
	/// </summary>
	public class DbView:DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbView(){
			this.ObjectType = DbObjectType.View;
		}
	}
}