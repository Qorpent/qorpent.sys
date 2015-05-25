using System;
using System.IO;

namespace Qorpent.IO{
	/// <summary>
	/// Описатель файла из файловой системы
	/// </summary>
	public class FileSystemWebFileRecord:WebFileRecord{
		/// <summary>
		/// Имя файла в файловой системе
		/// </summary>
		public string FileSystemName { get; set; }

	    public override long Length {
	        get { return new FileInfo(FileSystemName).Length; }
	        set { throw new NotImplementedException(); }
	    }

	    /// <summary>
		/// Получение версии из файла
		/// </summary>
		/// <returns></returns>
		protected override DateTime GetVersion(){
			return File.GetLastWriteTime(FileSystemName);
		}
		
		/// <summary>
		/// Записывает файл в целевой поток
		/// </summary>
		/// <param name="output"></param>
		public override long Write(Stream output){
			using (var fs = File.OpenRead(FileSystemName)){
				fs.CopyTo(output);
				return  fs.Length;
			}
			
		}

		/// <summary>
		/// Открытие потока на чтение
		/// </summary>
		/// <returns></returns>
		public override Stream Open(){
			return File.Open(FileSystemName,FileMode.Open,FileAccess.Read,FileShare.ReadWrite);
		}
	}
}