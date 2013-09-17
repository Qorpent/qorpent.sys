using System;

namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Поведение удаления при попытке удаления HEAD
	/// </summary>
	[Flags]
	public enum DeleteHeadBehavior {
		/// <summary>
		/// Запрет на удаление
		/// </summary>
		Deny=1<<1,
		/// <summary>
		/// Хид переводится в состояние DETACH
		/// </summary>
		Detach=1<<2,
		/// <summary>
		/// Запрет на удаление если речь идет о мерже и возврат к предыдущему если SINGLE или очистка при INIT
		/// </summary>
		AllowSingleDenyMerge =1<<3,
		/// <summary>
		/// Детач при INIT и Merge, предыдущий при INIT
		/// </summary>
		AllowSingleDetachMerge =1<<4,

	}
}