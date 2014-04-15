using System.Xml.Linq;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class SetAttributeOperation : PreprocessOperation
	{


		public override void Execute(XElement el)
		{
			el.SetAttributeValue(Name,Value);
		}
		/// <summary>
		/// 
		/// </summary>
		public string Name;
		/// <summary>
		/// 
		/// </summary>
		public string Value;
	}
}