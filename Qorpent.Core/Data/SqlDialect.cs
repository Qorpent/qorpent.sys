using System;

namespace Qorpent.Data{
	/// <summary>
	/// </summary>
	[Flags]
	public enum SqlDialect{
		/// <summary>
		///     Условный тип - без генерации
		/// </summary>
		None = 0,

		/// <summary>
		///     Стандарт ANSI
		/// </summary>
		Ansi = 1,

		/// <summary>
		///     SqlServer
		/// </summary>
		SqlServer = 2,

		/// <summary>
		///     PostGresql
		/// </summary>
		PostGres = 4,

		/// <summary>
		///     Oracle
		/// </summary>
		Oracle = 8,
	}
}