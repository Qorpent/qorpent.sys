using System;

namespace Qorpent {
	/// <summary>
	/// Тип вердикта
	/// </summary>
	[Flags]
	public enum VerdictType {
		/// <summary>
		/// Решение на разрешение
		/// </summary>
		Allow = 1,
		/// <summary>
		/// Предупреждение
		/// </summary>
		Warning =2,
		/// <summary>
		/// Запрет
		/// </summary>
		Deny = 4,
	}
}