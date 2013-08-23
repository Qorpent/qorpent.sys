using System;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	///     Типы активации объектов в BSharpRuntime
	/// </summary>
	[Flags]
	public enum BSharpActivationType {
		/// <summary>
		///     Тип сервиса диктуется клиентом
		/// </summary>
		Client = 1,

		/// <summary>
		///     Тип сервиса диктуется исходным классом
		/// </summary>
		Configured = 1 << 1,

		/// <summary>
		///     Автоматическое определение
		/// </summary>
		Auto = 1 << 2,

		/// <summary>
		///     По умолчанию -автоматически
		/// </summary>
		Default = Auto
	}
}