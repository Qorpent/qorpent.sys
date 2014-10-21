using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO{
	/// <summary>
	/// Провайдер файлов на основе ресурсов сборки
	/// </summary>
	public class ResourceWebFileProvider : WebFileProvider
	{
		private Assembly _assembly;
		/// <summary>
		/// Целевая сборка
		/// </summary>
		public Assembly Assembly{
			get { return _assembly; }
			set{
				_assembly = value;
				_index = null;
			}
		}
		/// <summary>
		/// Регеск подготовки локального пути ресурса
		/// </summary>
		public string StripRegex { get; set; }

		private IDictionary<string, string> _index;
		private void IndexiseResources(){
			if (null == _index){
				_index = new Dictionary<string, string>();
				var names = Assembly.GetManifestResourceNames();
				foreach (var name in names){
					var rn = name;
					if (!string.IsNullOrWhiteSpace(StripRegex)){
						rn = Regex.Replace(rn,StripRegex, "");
					}
					rn = ("/" + rn.Replace(".", "/")).NormalizePath();
					var li = rn.LastIndexOf("/");
					rn = rn.Substring(0, li) + "." + rn.Substring(li + 1, rn.Length - li - 1);
					_index[rn] = name;
				}

			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="rawName"></param>
		/// <returns></returns>
		protected override WebFileRecord GetRecord(string rawName){
			var localname = rawName;
			if (Prefix != "/") localname = Prefix + localname;
			var result = new ResourceWebFileRecord() { Name = localname, Assembly = Assembly, ResourceName = _index[rawName] };
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fn"></param>
		/// <returns></returns>
		protected override WebFileRecord FindFileNameOnly(string fn)
		{
			IndexiseResources();
			var filename = Path.GetFileName(fn);
			var first = _index.Keys.OrderBy(Path.GetDirectoryName).FirstOrDefault(_ => _.EndsWith("/" + filename));
			if (null != first) return GetRecord(first);
			return null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		protected override WebFileRecord FindExact(string file)
		{
			IndexiseResources();
			if (_index.ContainsKey(file)) return GetRecord(file);
			return null;
		}
	}
}