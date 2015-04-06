using System;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum GirFileState{
		/// <summary>
		/// Нет изменений
		/// </summary>
		None =0,
		/// <summary>
		/// Измененный
		/// </summary>
		Modified = 1,
		/// <summary>
		/// Добавленный
		/// </summary>
		Added =2 ,
		/// <summary>
		/// Удаленный
		/// </summary>
		Deleted = 4,
		/// <summary>
		///Переименованный
		/// </summary>
		Renamed =8,
		/// <summary>
		/// Скопированный
		/// </summary>
		Copied =16,
		/// <summary>
		/// Обновленный, но не смерженный
		/// </summary>
		UpdatedButUnmerged =32,
		
	}
}