#pragma warning disable 649
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class SetAttributeOperation : PreprocessOperation{
		/// <summary>
		/// </summary>
		public string Name;

		public string SubstRx;
		public string SubstSrc;

		/// <summary>
		/// </summary>
		public string Value;

		private Regex _rx;

		public override void Execute(XElement el){
			string val = Value;
			if (!string.IsNullOrWhiteSpace(SubstSrc)){
				string subst = (SubstSrc == "__value" || SubstSrc == "VALUE") ? el.Value : el.Attr(SubstSrc);
				if (string.IsNullOrWhiteSpace(SubstRx)){
					if (string.IsNullOrWhiteSpace(val)){
						val = subst;
					}
					else{
						val = string.Format(val, subst);
					}
				}
				else{
					_rx = _rx ?? new Regex(SubstRx);
					val = _rx.Replace(subst, val);
				}
			}
			if (Name == "__value" || Name == "VALUE"){
				el.Value = val;
			}
			else{
				el.SetAttributeValue(Name, val);
			}
		}
	}
}