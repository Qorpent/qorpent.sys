using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	internal class SetAttributeOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			var val = Value;
			if (!string.IsNullOrWhiteSpace(SubstSrc)){
				
				var subst = (SubstSrc=="__value"||SubstSrc =="VALUE")? el.Value: el.Attr(SubstSrc);
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
			if (Name == "__value" ||Name=="VALUE"){
				el.Value = val;
			}
			else{
				el.SetAttributeValue(Name, val);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Name;
		/// <summary>
		/// 
		/// </summary>
		public string Value;

	


		public string SubstSrc;

		public string SubstRx;
		private Regex _rx;
	}
}