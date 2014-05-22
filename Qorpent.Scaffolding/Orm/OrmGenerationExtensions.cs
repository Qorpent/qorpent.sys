using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Orm{
	/// <summary>
	/// 
	/// </summary>
	public static class OrmGenerationExtensions{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<IBSharpClass, XElement, string>> GetOrmFields(this IBSharpClass cls){
			var x = cls.Compiled;
			var datatypes = x.Elements("datatype")
			                 .ToDictionary(_ => CoreExtensions.Attr(_, "code"), _ => CoreExtensions.Attr(_, "type"));
			foreach (var e in x.Elements().OrderBy(_ => _.Attr("idx")).ThenBy(_ => _.Attr("code")))
			{
				if (string.IsNullOrWhiteSpace(e.Value))
				{
					//marks that it's funtion, not a field
					if (e.Name.LocalName == "ref" || datatypes.ContainsKey(e.Name.LocalName))
					{
						if (e.Name.LocalName == "ref"){
							yield return new Tuple<IBSharpClass, XElement, string>(cls, e, "");
						}
						else{
							yield return new Tuple<IBSharpClass, XElement, string>(cls, e, datatypes[e.Name.LocalName]);
						}
					}
				}
			}
		}
	}
}