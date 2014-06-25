#pragma warning disable 649
using System.Linq;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class ElementToAttributeOperation : PreprocessOperation{
		public string Name;

		public override void Execute(XElement el){
			XElement[] elements = el.Elements(Name).ToArray();
			foreach (XElement element in elements){
				string code = element.Attr("code").Escape(EscapingType.XmlName);
				string val = element.Describe().Value;
				el.SetAttributeValue(code, val);
			}
			elements.Remove();
		}
	}
}