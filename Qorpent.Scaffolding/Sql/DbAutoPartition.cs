namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class DbAutoPartition :DbPartitionScheme {
		/// <summary>
		/// 
		/// </summary>
		public DbAutoPartition()
		{
			ObjectType = DbObjectType.AutoPartition;

		}
	}
}