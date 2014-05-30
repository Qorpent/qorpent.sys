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
		/// <param name="context"></param>
		/// <param name="t"></param>
		/// <param name="withParents"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<IBSharpClass, XElement>> FindIncomeRefes(IBSharpContext context,IBSharpClass t, bool withParents = false)
		{
			var tables = context.ResolveAll("dbtable");
			foreach (var s in tables)
			{
				var refs = s.Compiled.Elements("ref");
				foreach (var r in refs)
				{
					if (!withParents && r.Attr("code") == "Parent") continue;
					if (!r.Attr("reverse").ToBool()) continue;
					var to = r.Attr("to");
					if (string.IsNullOrWhiteSpace(to))
					{
						to = r.Attr("code") + ".Id";
					}
					var fld = to.Split('.').Last();
					var cls = to.Substring(0, to.Length - fld.Length - 1);
					if (t.Name == cls || t.Compiled.Attr("fullname") == cls)
					{
						yield return new Tuple<IBSharpClass, XElement>(s, r);
					}
				}
			}
		} 

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<Tuple<IBSharpClass, XElement, string>> GetOrmFields(this IBSharpClass cls){
			var x = cls.Compiled;
			var datatypes = x.Elements("datatype")
			                 .ToDictionary(_ => CoreExtensions.Attr(_, "code"), _ => CoreExtensions.Attr(_, "type"));
			IDictionary<string,Tuple<IBSharpClass, XElement, string>> result = new Dictionary<string, Tuple<IBSharpClass, XElement, string>>();
			foreach (var e in x.Elements().OrderBy(_ => _.Attr("idx")).ThenBy(_ => _.Attr("code")))
			{
				if (string.IsNullOrWhiteSpace(e.Value))
				{
					//marks that it's funtion, not a field
					if (e.Name.LocalName == "ref" || datatypes.ContainsKey(e.Name.LocalName))
					{
						if (e.Name.LocalName == "ref"){
							result[e.Attr("code")] = new Tuple<IBSharpClass, XElement, string>(cls, e, "");
						}
						else{
							if (result.ContainsKey(e.Name.LocalName)){
								var existed = result[e.Name.LocalName];
								var qattrs = existed.Item2.Attributes().Where(_ => _.Name.LocalName.StartsWith("qorpent-"));
								result[e.Attr("code")] = new Tuple<IBSharpClass, XElement, string>(cls, e, datatypes[e.Name.LocalName]);
								foreach (var qattr in qattrs){
									if (null == e.Attribute(qattr.Name)){
										e.SetAttributeValue(qattr.Name,qattr.Value);
									}
								}
							}
							else{
								result[e.Attr("code")] = new Tuple<IBSharpClass, XElement, string>(cls, e, datatypes[e.Name.LocalName]);
							}
						}
					}
				}
			}
			return result.Values;
		}

	}
}