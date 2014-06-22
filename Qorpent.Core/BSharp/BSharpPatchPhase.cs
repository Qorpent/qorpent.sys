using System;

namespace Qorpent.BSharp{
	/// <summary>
	/// Фаза применения патча
	/// </summary>
	[Flags]
	public enum BSharpPatchPhase{
		/// <summary>
		/// На самом начальном этапе после прочтения исходников
		/// </summary>
		Before,
		/// <summary>
		/// После построения, но перед линковкой
		/// </summary>
		AfterBuild,
		/// <summary>
		/// После линковки
		/// </summary>
		After
	}
}