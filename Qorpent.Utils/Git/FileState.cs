using System;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// Статусы файлов в репозитории
	/// </summary>
	[Flags]
	public enum FileState:int{
		/*
		X          Y     Meaning
-------------------------------------------------
          [MD]   not updated
M        [ MD]   updated in index
A        [ MD]   added to index
D         [ M]   deleted from index
R        [ MD]   renamed in index
C        [ MD]   copied in index
[MARC]           index and work tree matches
[ MARC]     M    work tree changed since index
[ MARC]     D    deleted in work tree
-------------------------------------------------
D           D    unmerged, both deleted
A           U    unmerged, added by us
U           D    unmerged, deleted by them
U           A    unmerged, added by them
D           U    unmerged, deleted by us
A           A    unmerged, both added
U           U    unmerged, both modified
-------------------------------------------------
?           ?    untracked
!           !    ignored
-------------------------------------------------*?
		 */
		/// <summary>
		/// Неопределенный
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Не изменен
		/// </summary>
		NotModified = 1,
		/// <summary>
		/// Изменен
		/// </summary>
		Modified =2 ,
		/// <summary>
		/// Удален
		/// </summary>
		Deleted = 4,
		/// <summary>
		/// Добавлен
		/// </summary>
		Added =8 ,
		/// <summary>
		/// Переименован
		/// </summary>
		Renamed =16,
		/// <summary>
		/// Скопирован
		/// </summary>
		Copied =32,
		/// <summary>
		/// Обновлен
		/// </summary>
		Updated =64,
		/// <summary>
		/// Не на треке
		/// </summary>
		Untracked =128,
		/// <summary>
		/// Игнор
		/// </summary>
		Ignored = 256

	}
}