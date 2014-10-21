using System.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO{
	/// <summary>
	/// Базовый класс для провайдеров файлов для веб-приложений
	/// </summary>
	public abstract class WebFileProvider : IWebFileProvider
	{
		private string _prefix;

		/// <summary>
		/// Префикс имен файлов для определения локальных путей
		/// </summary>
		public string Prefix{
			get{
				if (string.IsNullOrWhiteSpace(_prefix)) _prefix = "/";
				return _prefix;
			}
			set{
				_prefix =("/"+ value).NormalizePath();
			}
		}
		/// <summary>
		/// Осуществляет поиск записи о файле по прямому или косвенному поиску
		/// </summary>
		/// <param name="file"></param>
		/// <param name="searchMode"></param>
		/// <returns></returns>
		public IWebFileRecord Find(string file, WebFileSerachMode searchMode = WebFileSerachMode.Exact){
			
			var nfile = ("/"+file).NormalizePath();
			if (nfile.StartsWith(Prefix + "/")){
				nfile = nfile.Substring(Prefix.Length, nfile.Length - Prefix.Length);
			}
			if (searchMode != WebFileSerachMode.IgnorePath){
				var exact = FindExact(nfile);
				if (null != exact) return exact;
				if (searchMode == WebFileSerachMode.Exact) return null;
			}
			if (ExactOnly) return null;
			var fn = "/" + Path.GetFileName(nfile);
			return FindFileNameOnly(fn);
		}
		/// <summary>
		/// Проверяет соответствие запроса поиска данному провайдеру по префиксу
		/// </summary>
		/// <param name="nsearch"></param>
		/// <returns></returns>
		public bool IsMatch(string nsearch){
			if (Prefix=="/") return true;
			return nsearch.StartsWith(Prefix + "/");
		}
		/// <summary>
		/// Признак того, что данный провайдер может использоваться только в директивном режиме
		/// </summary>
		public bool ExactOnly { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawName"></param>
		/// <returns></returns>
		protected abstract WebFileRecord GetRecord(string rawName);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fn"></param>
		/// <returns></returns>
		protected abstract WebFileRecord FindFileNameOnly(string fn);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="nfile"></param>
		/// <returns></returns>
		protected abstract WebFileRecord FindExact(string nfile);
	}
}