using System.Linq;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class ElementToAttributeOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			var elements = el.Elements(Name).ToArray();
			foreach (var element in elements){
				var code = element.Attr("code").Escape(EscapingType.XmlName);
				var val = element.Describe().Value;
				el.SetAttributeValue(code,val);
			}
			elements.Remove();
		}

		public string Name;
		
	}
}