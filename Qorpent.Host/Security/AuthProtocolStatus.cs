using System;

namespace Qorpent.Host.Exe.Security{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum AuthProtocolStatus:byte
	{
		/// <summary>
		/// Стандарт
		/// </summary>
		None = 0,
		/// <summary>
		/// Ошибочное состояние
		/// </summary>
		Error = 1,
		/// <summary>
		/// Требует защищенного соединения (на манер SSL)
		/// </summary>
		Secure = 2,
	}
}