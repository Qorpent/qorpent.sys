using System;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum MergeFlags{
		/// <summary>
		/// Копируется вся история
		/// </summary>
		FullHistory = 1,
		/// <summary>
		/// Копируется вся более поздняя история
		/// </summary>
		FullLateHistory = 2,
		/// <summary>
		/// Копируется только последняя версия
		/// </summary>
		Snapshot =4,
		/// <summary>
		/// 
		/// </summary>
		Default = FullLateHistory

	}
}