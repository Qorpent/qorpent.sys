using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Serialization;

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
		public static void Apply(this IEnumerable<XDiffItem> diffs,XElement target){
			foreach (var diff in diffs.ToArray()){
				diff.Apply(target);
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
	}
}