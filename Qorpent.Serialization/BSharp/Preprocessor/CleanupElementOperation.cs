using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class CleanupElementOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			if (string.IsNullOrWhiteSpace(Name)){
				el.Remove();
				return;
			}
			var toremove = (Level == "all" ? el.Descendants(Name) : el.Elements(Name)).ToArray();
			toremove.Remove();
		}

		public string Name;
		public string Level;
	}
}