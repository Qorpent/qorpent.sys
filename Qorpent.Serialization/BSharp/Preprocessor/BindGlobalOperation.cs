#pragma warning disable 649
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	internal class BindGlobalOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			var code = el.Attr(Name);
			if (!string.IsNullOrWhiteSpace(code) && _project.Global.ContainsKey(code)){
				el.SetAttributeValue(Name,_project.Global[code]);
			}

		}

		public string Name;
	}
}