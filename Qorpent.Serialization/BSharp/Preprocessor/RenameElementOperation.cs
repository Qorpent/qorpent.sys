#pragma warning disable 649
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class RenameElementOperation : PreprocessOperation{
		/// <summary>
		/// </summary>
		public string From;

		/// <summary>
		/// </summary>
		public string To;

		public override void Execute(XElement el){
			if (el.Name.LocalName == From){
				el.Name = To.Escape(EscapingType.XmlName);
			}
		}
	}
}