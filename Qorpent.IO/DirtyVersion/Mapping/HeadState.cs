using System;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Состояние слияние с HEAD
	/// </summary>
	[Flags]
	public enum HeadState {
		/// <summary>
		/// Не установлен
		/// </summary>
		None = 0,
		/// <summary>
		/// Сам является хидом
		/// </summary>
		IsHead = 1,
		/// <summary>
		/// Сведен в хид
		/// </summary>
		Merged = 1<<1,
		/// <summary>
		/// Не сведенный, но не голова бранча
		/// </summary>
		NonMerged = 1<<2,
		/// <summary>
		/// Не сведенный бранч
		/// </summary>
		NonMergedHead =1<<3,
	}
}