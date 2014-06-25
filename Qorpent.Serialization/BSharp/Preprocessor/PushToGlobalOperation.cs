#pragma warning disable 649
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class PushToGlobalOperation : PreprocessOperation{
		public bool Split;

		public override void Execute(XElement el){
			string[] codes = Split
				                 ? el.Attr("code").SmartSplit(false, true, ',', '/', ';', ' ').ToArray()
				                 : new[]{el.Attr("code")};
			string value = el.Value;
			foreach (string code in codes){
				lock (_project.Global){
					_project.Global.Set(code, value);
				}
			}
		}
	}
}