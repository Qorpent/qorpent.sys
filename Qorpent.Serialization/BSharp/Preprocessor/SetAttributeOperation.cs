using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	internal class SetAttributeOperation : PreprocessOperation
	{


		public override void Execute(XElement el){
			var val = Value;
			if (!string.IsNullOrWhiteSpace(SubstSrc)){
				var subst = el.Attr(SubstSrc);
				if (string.IsNullOrWhiteSpace(SubstRx)){
					val = string.Format(val, subst);
				}
				else{
					_rx = _rx ?? new Regex(SubstRx);
					val = _rx.Replace(subst, val);
				}
			}
			el.SetAttributeValue(Name,val);
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