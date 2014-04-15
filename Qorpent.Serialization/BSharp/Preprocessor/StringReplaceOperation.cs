using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	public class StringReplaceOperation : PreprocessOperation
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="el"></param>
		public override void Execute(XElement el)
		{
			foreach (var attribute in el.Attributes()){
				Execute(attribute);
			}
			foreach (var text in el.Nodes().OfType<XText>()){
				Execute(text);
			}
			if (Level == "all"){
				foreach (var descendant in el.Descendants()){
					Execute(descendant);
				}
			}
		}

		private void Execute(XText text){
			if (null!=_regex){
				text.Value = _regex.Replace(text.Value, To);
			}
			else{
				text.Value = text.Value.Replace(_from, To);
			}
		}

		private void Execute(XAttribute attribute){
			if (null != _regex)
			{
				attribute.Value = _regex.Replace(attribute.Value, To);
			}
			else
			{
				attribute.Value = attribute.Value.Replace(_from, To);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string To;
		/// <summary>
		/// 
		/// </summary>
		public string Level;
		/// <summary>
		/// 
		/// </summary>
		private string _from;

		private Regex _regex;

		/// <summary>
		/// 
		/// </summary>
		public string From{
			get { return _from; }
			set {
				_from = value; 
				if (_from[0]=='/' && _from[_from.Length-1]=='/'){
					_regex = new Regex(_from.Substring(1,_from.Length-2));
				}
			}
		}
	}
}