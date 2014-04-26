using System;
using System.IO;
using System.Reflection;
using Qorpent.IO;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
	/// <summary>
	/// Статический источник для ресурсов
	/// </summary>
	public class ResourceContentDescriptor : StaticContentDescriptor{
		private string _resname;
		private string _assemblyFile;
		private Assembly _assembly;
		private DateTime _version;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="resourceName"></param>
		/// <param name="assembly"></param>
		/// <param name="version"></param>
		public ResourceContentDescriptor(string name, string resourceName, Assembly assembly, DateTime version = new DateTime()){
			Name = name;
			_resname = resourceName;
			_assembly = assembly;

			if (version.Year <= 1900){
				_assemblyFile = Path.GetFullPath(_assembly.CodeBase.Replace("file:///", ""));
			}
			else{
				_version = version;
			}


			FullName = "res://" + (assembly.GetName().Name + "/" + name).NormalizePath();
			MimeType = MimeHelper.GetMimeByExtension(Path.GetExtension(name));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override DateTime GetLastVersion(){
			if (_version.Year > 1900){
				return _version;
			}
			return File.GetLastWriteTime(_assemblyFile);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override Stream Open(){
			return _assembly.GetManifestResourceStream(_resname);
		}
	}
}