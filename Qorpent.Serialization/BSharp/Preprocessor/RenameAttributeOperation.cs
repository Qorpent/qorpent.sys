#pragma warning disable 649
using System.Xml.Linq;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class RenameAttributeOperation : PreprocessOperation{
		/// <summary>
		/// </summary>
		public string From;

		/// <summary>
		/// </summary>
		public string To;

		public override void Execute(XElement el){
			XAttribute target = el.Attribute(From);
			if (null != target){
				target.Remove();
				el.SetAttributeValue(To, target.Value);
			}
		}
	}
}