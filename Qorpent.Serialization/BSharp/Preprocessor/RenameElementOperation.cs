#pragma warning disable 649
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	internal class RenameElementOperation : PreprocessOperation
	{


		public override void Execute(XElement el)
		{
			if (el.Name.LocalName == From){
				el.Name = To.Escape(EscapingType.XmlName);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string To;
		/// <summary>
		/// 
		/// </summary>
		public string From;
	}
}