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
		/// <summary>
		/// Обработка инклудов
		/// </summary>
		IncludeProcessing = 1<<2,
		/// <summary>
		/// Компиляция в целом
		/// </summary>
		Common = 1<<3,
	}
}