using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	internal class PushToGlobalOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			string[] codes = Split
				                 ? el.Attr("code").SmartSplit(false, true, ',', '/', ';', ' ').ToArray()
				                 : new[]{el.Attr("code")};
			var value = el.Value;
			foreach (var code in codes){
				_project.Global.Set(code, value);
			}
			
		}

		public bool Split;
	}
}