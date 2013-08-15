using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Фазы компиляции BSharp
	/// </summary>
	[Flags]
	public enum BSharpCompilePhase {
		/// <summary>
		/// Индексация источников
		/// </summary>
		SourceIndexing  =1,
		/// <summary>
		/// Разрешение импортов
		/// </summary>
		ImportResolution = 1<<1,
	}
}