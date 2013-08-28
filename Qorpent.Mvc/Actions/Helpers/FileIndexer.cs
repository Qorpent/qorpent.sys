using System;
using System.Collections.Generic;

namespace Qorpent.Mvc.Actions.Helpers {
	/// <summary>
	/// Индексатор файлов
	/// </summary>
	public class FileIndexer {
		/// <summary>
		/// Выводит перечень файолов и папок приложения по маске
		/// </summary>
		/// <param name="fileMask"></param>
		/// <param name="showDirs"></param>
		/// <param name="showFiles"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public IEnumerable<FileListEntry> Collect(string fileMask=null, bool showDirs=true, bool showFiles =true) {
			throw new NotImplementedException();
		}
	}
}