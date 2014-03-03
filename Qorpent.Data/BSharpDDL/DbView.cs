namespace Qorpent.Data.BSharpDDL{
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