using System;

namespace Qorpent.Host.Security{
	/// <summary>
	///     Тип токена пользователя
	/// </summary>
	[Flags]
	public enum TokenType : byte{
		/// <summary>
		/// </summary>
		None = 0,

		/// <summary>
		/// </summary>
		Guest = 1,

		/// <summary>
		/// </summary>
		Remote = 2,

		/// <summary>
		/// </summary>
		Local = 4,

		/// <summary>
		/// </summary>
		Admin = 8,

		/// <summary>
		/// </summary>
		Error = 16,

		/// <summary>
		/// </summary>
		Authenticated = Remote | Local | Admin
	}
}