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
           if (showFiles)
		    {
                var fileName = Directory.GetFiles(EnvironmentInfo.RootDirectory, "", SearchOption.AllDirectories);
                for (int index = 0; index < fileName.Count(); index++)
                {
                    listFilesCollection.Add(new FileListEntry() { LocalPath = fileName[index], Type = FileListEntryType.File });
                }
		    }
           if (showDirs)
           {
	        string[] dirNames = Directory.GetDirectories(EnvironmentInfo.RootDirectory, "", SearchOption.AllDirectories);
	        
                for (int index = 0; index < dirNames.Count(); index++)
                {
                    listFilesCollection.Add(new FileListEntry() { LocalPath = dirNames[index], Type = FileListEntryType.Directory });
                }
            }
	        return listFilesCollection;
        }
      }
}