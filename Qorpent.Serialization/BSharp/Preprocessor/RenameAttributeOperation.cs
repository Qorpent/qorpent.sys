using System.Xml.Linq;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class RenameAttributeOperation : PreprocessOperation{
	

		public override void Execute(XElement el){
			XAttribute target = el.Attribute(From);
			if (null !=target){
				target.Remove();
				el.SetAttributeValue(To,target.Value);
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