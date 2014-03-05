using System;
using System.Linq;

namespace Qorpent.Scaffolding{
	/// <summary>
	/// 
	/// </summary>
	public class BSharpClassRef{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="srcname"></param>
		public BSharpClassRef(string srcname){
			var parts = srcname.Split('.');
			if (parts.Length == 1){
				Name = parts[0];
			}
			else{
				Name = parts.Last();
				Namespace = String.Join(".", parts.Take(parts.Length - 1));
			}
			if (Name.EndsWith("*")){
				Name =  Name.Substring(0, Name.Length - 1);
				IsArray = true;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Namespace="";
		/// <summary>
		/// 
		/// </summary>
		public string Name="";
		/// <summary>
		/// 
		/// </summary>
		public bool IsArray = false;
	}
}