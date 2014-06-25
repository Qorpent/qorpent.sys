#pragma warning disable 649
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class CleanupElementOperation : PreprocessOperation{
		public string Level;
		public string Name;

		public override void Execute(XElement el){
			if (string.IsNullOrWhiteSpace(Name)){
				el.Remove();
				return;
			}
			XElement[] toremove = (Level == "all" ? el.Descendants(Name) : el.Elements(Name)).ToArray();
			toremove.Remove();
		}
	}
}