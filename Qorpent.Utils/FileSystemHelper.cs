using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public static class FileSystemHelper
	{
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
