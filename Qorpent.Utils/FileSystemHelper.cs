using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public static class FileSystemHelper
	{
	    /// <summary>
	    /// Создает и очищает временную папку
	    /// </summary>
	    /// <param name="name">Указать имя папки, по умолчанию - заточка для тестов КЛАСС_МЕТОД</param>
	    /// <param name="tmproot">Указать, если нужно использовать особенную корневую временную папку</param>
	    /// <returns></returns>
	    public static string ResetTemporaryDirectory(string name = null, string tmproot = null) {
            var root = Path.GetTempPath();
            if (!string.IsNullOrWhiteSpace(tmproot)) {
                Directory.CreateDirectory(tmproot);
                root = tmproot;
            }
	        if (string.IsNullOrWhiteSpace(name)) {
	            var method = new StackFrame(1, true).GetMethod();
	            name = method.DeclaringType.Name + "_" + method.Name;
	        }
            var tmpdir = Path.Combine(root,name);
            KillDirectory(tmpdir);
            Directory.CreateDirectory(tmpdir);
	        return tmpdir;
	    }

		/// <summary>
		/// Avoids Directory.Delete problem with ReadOnly files and checks directory existence
		/// </summary>
		/// <param name="dirname"></param>
		public static void KillDirectory(string dirname)
		{
			if (Directory.Exists(dirname))
			{
				try
				{
					Directory.Delete(dirname, true);
				}
				catch
				{
                    Thread.Sleep(100);
					foreach (var file in Directory.GetFiles(dirname, "*.*", SearchOption.AllDirectories))
					{
						File.SetAttributes(file, FileAttributes.Normal);
					}
					Directory.Delete(dirname, true);
				}
			}


		}
	}
}
