using System.Linq;
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
				var subst = el.Attr(SubstSrc);
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
			if (Name == "__value"){
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

	/// <summary>
	/// 
	/// </summary>
	internal class RemoveAttributeOperation : PreprocessOperation
	{
		private string _name;


		public override void Execute(XElement el)
		{
			foreach (var a in el.Attributes().ToArray()){
				if (null != _rx){
					if (_rx.IsMatch(a.Name.LocalName)){
						a.Remove();
					}
					else{
						if(a.Name.LocalName==Name)a.Remove();
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return _name; }
			set{
				_name = value;
				if (_name.StartsWith("/")){
					_rx = new Regex(_name.Substring(1,_name.Length-2));
				}
			}
		}

		private Regex _rx;
	}
}