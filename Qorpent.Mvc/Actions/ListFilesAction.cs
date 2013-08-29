
﻿using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.IoC;
using Qorpent.Mvc;
using Qorpent.Mvc.Actions;
using Qorpent.Mvc.Actions.Helpers;
using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {


	/// <summary>
	/// 	Возвращает текущий откомпилированный манифест (полный XML)
	/// </summary>
	[Action("_sys.listfiles", Role = "DEVELOPER", Help = "Возвращает список всех файлов и папок, где находися приложение",
		Arm = "admin")]
	public class ListFilesAction : ActionBase {
		/// <summary>
		/// Имя скрипта
		/// </summary>
		[Bind] protected string FileMask;

		/// <summary>
		/// Показывать папки
		/// </summary>
		[Bind] protected bool ShowDirs;

		/// <summary>
		/// Показывать файлы
		/// </summary>
		[Bind] protected bool ShowFiles;

		/// <summary>
		/// 	fh
		/// </summary>
		protected override object MainProcess() {
			return new FileIndexer().Collect(FileMask, ShowDirs, ShowFiles).ToArray();
		}



	}
}
