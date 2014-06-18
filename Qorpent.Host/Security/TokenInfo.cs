using System;

namespace Qorpent.Host.Exe.Security{
	/// <summary>
	/// 
	/// </summary>
	public class TokenInfo{
		/// <summary>
		/// Статус
		/// </summary>
		public bool Ok;
		/// <summary>
		/// Результирующий логин
		/// </summary>
		public string Login;
		/// <summary>
		/// Токен
		/// </summary>
		public string Token;
		/// <summary>
		/// Срок действия
		/// </summary>
		public DateTime Expire;
	}
}