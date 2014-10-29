using System;
using System.IO;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO{
	/// <summary>
	/// Провайдер статических файлов для Web-приложений на основе файловой системы
	/// </summary>
	public class FileSystemWebFileProvider :WebFileProvider{
		private  string _root;
		/// <summary>
		/// Корневая директория для поиска файлов
		/// </summary>
		public  string Root{
			get { return _root; }
			set { _root = value.NormalizePath(); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawName"></param>
		/// <returns></returns>
		protected override WebFileRecord GetRecord(string rawName){
			var localname = rawName.NormalizePath().Replace(Root, "");
			if (!localname.StartsWith("/")) localname = "/" + localname;
			if (Prefix != "/") localname = Prefix + localname;
			var result = new FileSystemWebFileRecord{Name = localname, FileSystemName = rawName,FullName = rawName};
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fn"></param>
		/// <returns></returns>
		protected override WebFileRecord FindFileNameOnly(string fn){
		    try
		    {
		        var filename = Path.GetFileName(fn);
		        var first = Directory.EnumerateFiles(Root, filename, SearchOption.AllDirectories).FirstOrDefault();
		        if (null != first) return GetRecord(first);
		        return null;
		    }
		    catch (Exception e)
		    {
		        throw new IOException("Error with path '"+Root+"' on '"+fn+"'",e);
		    }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		protected override WebFileRecord FindExact(string file){
			if (file.StartsWith("/")) file = file.Substring(1);
			var fullPath = Path.Combine(Root, file);
			if (File.Exists(fullPath)){
				return GetRecord(fullPath);
			}
			return null;
		}
	}
}