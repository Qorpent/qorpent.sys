using System;
using System.IO;
using System.Reflection;

namespace Qorpent.IO{
	/// <summary>
	/// Описатель файла из файловой системы
	/// </summary>
	public class ResourceWebFileRecord : WebFileRecord
	{
		/// <summary>
		/// Имя файла в файловой системе
		/// </summary>
		public string ResourceName { get; set; }
		/// <summary>
		/// Целевая сборка, хранящая ресурсы
		/// </summary>
		public Assembly Assembly { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override DateTime GetVersion(){
			return File.GetCreationTime(Assembly.Location);
		}

		/// <summary>
		/// Записывает файл в целевой поток
		/// </summary>
		/// <param name="output"></param>
		public override long Write(Stream output)
		{
			using (var fs = Assembly.GetManifestResourceStream(ResourceName))
			{
				fs.CopyTo(output);
				return fs.Length;
			}

		}
	}
}