namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// Типы движков БД
	/// </summary>
	public enum DatabaseEngineType {
		/// <summary>
		/// Microsoft SQL Server
		/// </summary>
		SqlServer = 0,
		/// <summary>
		/// PostgreSQL
		/// </summary>
		Postgres =1 ,
		/// <summary>
		/// Oracle DBE
		/// </summary>
		Oracle =2 ,
		/// <summary>
		/// MySQL 
		/// </summary>
		MySql =3 ,
		/// <summary>
		/// UnknownEngine
		/// </summary>
		Undefined = 4
	}
}