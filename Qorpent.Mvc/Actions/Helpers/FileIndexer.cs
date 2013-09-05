using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Utils.Extensions;

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
	        return from filename in GetAllFileNames()
                where IsMatchTypeFilter(showDirs, showFiles, filename)
                let entry = GetFileEntry(filename)
                where IsMatchSearchMask(entry,fileMask)
                select entry;
	    }

	    private bool IsMatchSearchMask(FileListEntry entry, string fileMask)
	    {
	        return fileMask.SmartSplit(false, true, ' ')
                .All(mask => IsMatchMask(entry.LocalPath, mask));
	    }

	    private bool IsMatchMask(string localName, string mask)
	    {
	        if (mask.StartsWith("!"))
	        {
	            var realmask = mask.Substring(1);
	            return !localName.Contains(realmask);
	        }
            return localName.Contains(mask);
	    }

	    private FileListEntry GetFileEntry(string name)
	    {
	        var localname = name.Replace("\\", "/");
	        var basedir = EnvironmentInfo.RootDirectory.Replace("\\", "/");
            var basedirStr=basedir.Split('/');
            localname = localname.Replace(basedir, basedirStr[basedirStr.Length-2]+"/");
           
	        return new FileListEntry
	            {
                    LocalPath = localname,
                    Type = IsDirectory(name) ? FileListEntryType.Directory : FileListEntryType.File
	            };
	    }

	    private static bool IsMatchTypeFilter(bool showDirs, bool showFiles, string name)
	    {
	        return (showDirs && IsDirectory(name)) ||
	               (showFiles && !IsDirectory(name));
	    }

	    private static bool IsDirectory(string name)
	    {
	        return File.GetAttributes(name).HasFlag(FileAttributes.Directory);
	    }

	    private static IEnumerable<string> GetAllFileNames()
	    {
	        return Directory.GetFileSystemEntries(EnvironmentInfo.RootDirectory,"*",SearchOption.AllDirectories);
	    }
	}
}