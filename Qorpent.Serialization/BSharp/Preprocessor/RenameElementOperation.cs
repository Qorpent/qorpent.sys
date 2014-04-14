using System.Xml.Linq;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class RenameElementOperation : PreprocessOperation
	{


		public override void Execute(XElement el)
		{
			if (el.Name.LocalName == From){
				el.Name = To;
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