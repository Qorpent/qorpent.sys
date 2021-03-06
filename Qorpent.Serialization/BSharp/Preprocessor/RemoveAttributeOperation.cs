﻿#pragma warning disable 649
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// </summary>
	internal class RemoveAttributeOperation : PreprocessOperation{
		private string _name;


		private Regex _rx;

		/// <summary>
		/// </summary>
		public string Name{
			get { return _name; }
			set{
				_name = value;
				if (_name.StartsWith("/")){
					_rx = new Regex(_name.Substring(1, _name.Length - 2));
				}
			}
		}

		public override void Execute(XElement el){
			foreach (XAttribute a in el.Attributes().ToArray()){
				if (null != _rx){
					if (_rx.IsMatch(a.Name.LocalName)){
						a.Remove();
					}
				}
				else{
					if (a.Name.LocalName == Name) a.Remove();
				}
			}
		}
	}
}