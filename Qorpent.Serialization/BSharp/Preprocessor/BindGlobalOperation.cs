#pragma warning disable 649
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class BindGlobalOperation : PreprocessOperation{
		public string Name;

		public override void Execute(XElement el){
			string code = el.Attr(Name);
			if (!string.IsNullOrWhiteSpace(code) && _project.Global.ContainsKey(code)){
				el.SetAttributeValue(Name, _project.Global[code]);
			}
		}
	}
}