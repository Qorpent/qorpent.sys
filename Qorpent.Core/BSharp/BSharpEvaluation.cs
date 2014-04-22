using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	/// 
	/// </summary>
	public class BSharpEvaluation{
		/// <summary>
		/// Исходный элемент
		/// </summary>
		public XElement Source { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public BSharpEvaluation Bind(XElement target){
			return new BSharpEvaluation{Source = Source, BindedTarget = target};

		}
		/// <summary>
		/// 
		/// </summary>
		public XElement BindedTarget { get; set; }
		/// <summary>
		/// 
		/// 
		/// </summary>
		public string Code{
			get { return Source.Attr("code"); }
		}
		/// <summary>
		/// 
		/// </summary>
		public string ResultType{
			get { return Source.Attr("result", "attribute"); }
			
		}
		/// <summary>
		/// 
		/// </summary>
		public string Value{
			get { return Source.Value; }
		}
		/// <summary>
		/// Вычисляет и применяет себя к указанному классу
		/// </summary>
		/// <param name="cls"></param>
		public void Evaluate(IBSharpClass cls){
			foreach (var t in GetTargets(BindedTarget??cls.Compiled)){
				EvalSimpleXPathAttributeSubs(t);		
			}
		
		}

		private XElement[] GetTargets(XElement c){
			if (Target == "class"){
				return new[]{c};
			}
			if (Target == "elements"){
				return c.Elements().ToArray();
			}
			if (Target.StartsWith("./")){
				return c.XPathSelectElements(Target).ToArray();
			}
			return c.Elements(Target).ToArray();
		}
		/// <summary>
		/// 
		/// </summary>
		public string Target{
			get { return Source.Attr("for", "class"); }
		}

		private void EvalSimpleXPathAttributeSubs(XElement e){
			var curval = e.Attr(Code);
			var expr = curval;
			if (!string.IsNullOrWhiteSpace(Value) && Value.Contains("${this}")){
				expr = Value.Replace("${this}", curval);
			}
			if (string.IsNullOrWhiteSpace(expr)){
				return;
			}
			try{
				var realvalue = e.XPathEvaluate(expr);
				e.SetAttributeValue(Code, realvalue);
			}
			catch (Exception ex){
				e.SetAttributeValue(Code, "error: " + ex.Message);
			}
		}
	}
}