using System;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum SqlMethodOptions{
		/// <summary>
		/// Не является Sql-методом
		/// </summary>
		None =0,
		/// <summary>
		/// Признак того, что сущность вообще является SQL-методом
		/// </summary>
		IsMethod = 1,
		/// <summary>
		/// Признак того, что результат запроса является кэшируемым
		/// </summary>
		Cacheable =2 ,
	}
}