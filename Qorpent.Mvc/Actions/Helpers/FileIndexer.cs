using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
	    public IEnumerable<FileListEntry> Collect(string fileMask = null, bool showDirs = true, bool showFiles = true) 
        {
            var listFilesCollection = new List<FileListEntry>();
            var filesNameMass = ListFile();
            var dirNameMass = ListDir();
	       if (showDirs)
		    {
                for (int index = 0; index < filesNameMass.Count(); index++)
                {
                    listFilesCollection.Add(new FileListEntry() { LocalPath = filesNameMass[index], Type = FileListEntryType.Directory });
                }
		    }
            
          // var dirsNameMass = ListDir();
            if (showFiles)
            {
                for (int index = 0; index < dirNameMass.Count(); index++)
                {
                    listFilesCollection.Add(new FileListEntry() { LocalPath = dirNameMass[index].ToString(), Type = FileListEntryType.File });
                }
            }
	        return listFilesCollection;
        }
        /// <summary>
        /// Список папок
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo[] ListDir()
        {
            var di = new DirectoryInfo(EnvironmentInfo.RootDirectory);
            return di.GetDirectories("*.*", SearchOption.AllDirectories);
        }
        /// <summary>
        /// Список файлов
        /// </summary>
        /// <returns></returns>
        public string[] ListFile()
        {
            return Directory.GetFiles(EnvironmentInfo.RootDirectory, "*.*", SearchOption.AllDirectories);
        }

	}
}