using System;
using System.Collections.Generic;
using System.IO;

namespace Qorpent.Mvc.Actions.Helpers {
	/// <summary>
	/// ���������� ������
	/// </summary>
	public class FileIndexer {
		/// <summary>
		/// ������� �������� ������� � ����� ���������� �� �����
		/// </summary>
		/// <param name="fileMask"></param>
		/// <param name="showDirs"></param>
		/// <param name="showFiles"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public IEnumerable<FileListEntry> Collect(string fileMask=null, bool showDirs=true, bool showDirs =true) 
        {
            var listFilesCollection = new List<FileListEntry>();
            if (showDirs)
		    {
		    }
            if (showDirs)
            {
            }
        }
        /// <summary>
        /// ������ �����
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo[] ListDir()
        {
            var di = new DirectoryInfo(EnvironmentInfo.RootDirectory);
            return di.GetDirectories("*.*", SearchOption.AllDirectories);
        }
        /// <summary>
        /// ������ ������
        /// </summary>
        /// <returns></returns>
        public string[] ListFile()
        {
            return Directory.GetFiles(EnvironmentInfo.RootDirectory, "*.*", SearchOption.AllDirectories);
        }

	}
}