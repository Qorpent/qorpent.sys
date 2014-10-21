using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Tests{
	/// <summary>
	/// 
	/// </summary>
	public class StubWebStaticProvider:WebFileProvider{
		IList<string> names = new List<string>();

		public void Add(params string[] files){
			foreach (var file in files){
				names.Add(file.ToLowerInvariant().NormalizePath());
			}
		}

		protected override WebFileRecord GetRecord(string rawName){
			if (Prefix == "/") return new StubFileRecord(rawName);
			return new StubFileRecord(Prefix+rawName);
		}

		protected override WebFileRecord FindFileNameOnly(string fn)
		{
			var match = names.FirstOrDefault(_ => _.EndsWith(fn));
			return match==null?null: GetRecord(match);
		}

		protected override WebFileRecord FindExact(string nfile)
		{
			var exact = names.FirstOrDefault(_ => _ == nfile);
			return exact==null?null:GetRecord(exact);
		}
	}
}