using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.XDiff{
	/// <summary>
	/// 
	/// </summary>
	public static class XDiffExtensions{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="diffs"></param>
		/// <param name="target"></param>
		/// <param name="options"></param>
		public static XElement Apply(this IEnumerable<XDiffItem> diffs,XElement target, XDiffOptions options = null){
			options = options ?? new XDiffOptions();
			IDictionary<string, string> movereplaces = new Dictionary<string, string>();
			var orderedDiffs = diffs.
			OrderBy(_ => _.Action)
				.ThenByDescending(_ => null == _.NewestElement ? 0 : _.NewestElement.Descendants().Count())
				.ToArray();
			foreach (var diff in orderedDiffs)
			{
				if (diff.CanChangeHierarchyTarget){
					FillDictionary(movereplaces,diff);
				}else if (diff.CanChangeHierarchyTarget){
					while (movereplaces.ContainsKey(diff.NewValue)){
						diff.NewValue = movereplaces[diff.NewValue];
					}	
				}
				diff.Apply(target,true,options);
			}
			return target;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private  static void FillDictionary(IDictionary<string, string> map, XDiffItem diff ){
			var id = diff.BasisElement.Attribute("id");
			var code = diff.BasisElement.Attribute("code");
			var oldname = diff.BasisElement.Name.LocalName;
			if (diff.Action == XDiffAction.RenameElement){
				
				var newname = diff.NewValue;
				if (null != id){
					map[oldname + "-id-" + id.Value] = newname + "-id-" + id.Value;

				}
				if (null != code){
					map[oldname + "-code-" + code.Value] = newname + "-code-" + code.Value;
				}
				
			}else if (diff.Action == XDiffAction.ChangeAttribute){
				if (diff.NewestAttribute.Name.LocalName == "code"){
					map[oldname + "-code-" + code.Value] = oldname + "-code-" + diff.NewestAttribute.Value;
					map["code-" + code.Value] = "code-" + diff.NewestAttribute.Value;
				}
				else if (diff.NewestAttribute.Name.LocalName == "id")
				{
					map[oldname + "-id-" + id.Value] = oldname + "-id-" + diff.NewestAttribute.Value;
					map["id-" + id.Value] = "id-" + diff.NewestAttribute.Value;
				}
			}


		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="diffs"></param>
		/// <param name="fullelements"></param>
		/// <returns></returns>
		public static string LogToString(this IEnumerable<XDiffItem> diffs, bool fullelements = false){
			var sw = new StringWriter();
			diffs.Log(sw,fullelements);
			return sw.ToString();
		}

		/// <summary>
		/// Записывает журнал изменений в текстовой редактор в формате B#
		/// </summary>
		/// <param name="diffs"></param>
		/// <param name="output"></param>
		/// <param name="fullelements"></param>
		public static void Log(this IEnumerable<XDiffItem> diffs, TextWriter output, bool fullelements = false){
			var i = 0;
			foreach (var d in diffs.ToArray().OrderBy(_=>_.Action)){
				output.WriteLine(d.Action+" n"+(i++));
				if (null != d.BasisElement){
					WriteElement("BasisElement", output, d.BasisElement,fullelements,d.Action);
				}
				if (null != d.NewestElement)
				{
					WriteElement("NewestElement", output, d.NewestElement, fullelements,d.Action);
				}
				if (null != d.NewValue){
					output.WriteLine("\tNewValue : "+d.NewValue.Escape(EscapingType.BxlStringOrLiteral));
				}
				if (null != d.BasisAttribute)
				{
					output.WriteLine("\tBasisAttribute "+d.BasisAttribute.Name.LocalName+" :" + d.BasisAttribute.Value.Escape(EscapingType.BxlStringOrLiteral) );
				}
				if (null != d.NewestAttribute)
				{
					output.WriteLine("\tNewestAttribute " + d.NewestAttribute.Name.LocalName + " :" + d.NewestAttribute.Value.Escape(EscapingType.BxlStringOrLiteral));
				}
			}
		}

		private static void WriteElement(string name, TextWriter output, XElement d,bool fullelements,XDiffAction action){
			if (d.Name.LocalName == "update" || d.Name.LocalName == "insert"){
				output.WriteLine("\t" + name + " : (" + d.ToString().Replace("(", "\\(").Replace(")", "\\)") + ")");
				return;
			}
			fullelements = fullelements || (name == "NewestElement" && action == XDiffAction.CreateElement);
			
			if (fullelements){
				output.WriteLine("\t"+name+" : (" + d.ToString().Replace("(", "\\(").Replace(")", "\\)") + ")");
			}
			else{
				var attr = d.Attribute("id");
				if (null == attr){
					attr = d.Attribute("code");
				}
				var p = d.Attribute("__parent");
				output.Write("\t" + name + " name=" + d.Name.LocalName + " " + attr.Name + "=" + attr.Value.Escape(EscapingType.BxlStringOrLiteral));
				if (null != p){
					output.Write(" parent="+p.Value.Escape(EscapingType.BxlStringOrLiteral));
				}
				if (name == "NewestElement" && action == XDiffAction.ChangeElement){
					output.Write(" : ");
					output.Write(d.Value.Escape(EscapingType.BxlStringOrLiteral));
				}
				output.WriteLine();
				
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="__options"></param>
		/// <returns></returns>
		public static bool IsJoined(XElement a, XElement b, XDiffOptions __options){
			//тут не на кодах, id используется как первичный ключ
			var aid = __options.ChangeIds? null : a.Attribute("id");
			var haid = aid != null;
			var bid = __options.ChangeIds ? null : b.Attribute("id");
			var hbid = bid != null;
			var acode = a.Attribute("code");
			var hacode = acode != null;
			var bcode = b.Attribute("code");
			var hbcode = bcode != null;

			if (!haid && !hacode) return false; //no identity on a
			if (!hbid && !hbcode) return false; //no idenityt on a

			var testvala = "";
			var testvalb = "";
			if (haid && hbid){ // both on ids
				testvala = aid.Value;
				testvalb = bid.Value;
			}else if (hacode && hbcode){
				testvala = acode.Value;
				testvalb = bcode.Value;
			}
			else{
				return false; //some incompability checks
			}
			if (testvala != testvalb) return false;
			if (__options.IsNameIndepended){
				return true;
			}
			return a.Name.LocalName == b.Name.LocalName;
		}

		
	}
}