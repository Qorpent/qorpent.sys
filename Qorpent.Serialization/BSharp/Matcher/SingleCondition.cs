using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Matcher{
	/// <summary>
	///     ��������� �������
	/// </summary>
	public sealed class SingleCondition{
		private XElement _e;

		/// <summary>
		///     ������� ������� �� ��������
		/// </summary>
		/// <param name="a"></param>
		public SingleCondition(XAttribute a){
			Value = a.Value;
			ConditionType = ConditionType.Eq;
			if ("NULL" == Value){
				ConditionType = ConditionType.IsNull;
			}
			else if ("TRUE" == Value){
				ConditionType = ConditionType.IsTrue;
			}
			AttributeName = a.Name.LocalName;
			if (a.Name.LocalName.Contains("__")){
				SetupFromSpecialName(a);
			}
			if (AttributeName == "xpath"){
				ConditionType = ConditionType.XPath;
			}
		}

		/// <summary>
		///     ��� ��������
		/// </summary>
		public string AttributeName { get; set; }

		/// <summary>
		///     ��� �������
		/// </summary>
		public ConditionType ConditionType { get; set; }

		/// <summary>
		///     ���������
		/// </summary>
		public bool Negate { get; set; }

		/// <summary>
		///     ��������
		/// </summary>
		public string Value { get; set; }

		private void SetupFromSpecialName(XAttribute a){
			string n = a.Name.LocalName;
			if (n.Contains("!".Escape(EscapingType.XmlName))){
				Negate = true;
				n = n.Replace("!".Escape(EscapingType.XmlName), "");
			}
			if (Value.ToUpper() == "NULL"){
				ConditionType = ConditionType.IsNull;
			}
			else if (Value.ToUpper() == "TRUE"){
				ConditionType = ConditionType.IsTrue;
			}
			if (n.EndsWith(">>".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.Gr;
			}
			else if (n.EndsWith(">".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.GrE;
			}
			else if (n.EndsWith("<<".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.Le;
			}
			else if (n.EndsWith("<".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.LeE;
			}
			else if (n.EndsWith("~".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.Match;
			}
			else if (n.EndsWith("&+".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.CrossList;
			}
			else if (n.EndsWith("&&".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.IverseInList;
			}
			else if (n.EndsWith("&".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.InList;
			}


			else if (n.EndsWith("%".Escape(EscapingType.XmlName))){
				ConditionType = ConditionType.Contains;
			}
			int idx = n.IndexOf("__");
			if (-1 != idx){
				n = n.Substring(0, idx);
			}
			AttributeName = n;
		}

		/// <summary>
		///     �������� ������� �� ������� ��������
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool IsMatch(XElement e){
			_e = e;
			XAttribute a = e.Attribute(AttributeName);
			if (null == a && AttributeName == "localname"){
				a = new XAttribute("localname", e.Name.LocalName);
			}
			bool mainismatch = InternalIsMatch(a);
			return Negate ? !mainismatch : mainismatch;
		}

		private bool InternalIsMatch(XAttribute a){
			switch (ConditionType){
				case ConditionType.Contains:
					if (null == a) return false;
					return a.Value.Contains(Value);
				case ConditionType.Eq:
					if (null == a) return string.IsNullOrWhiteSpace(Value);
					return Value == a.Value;
				case ConditionType.Gr:
					if (null == a) return 0 > Value.ToDecimal();
					return a.Value.ToDecimal() > Value.ToDecimal();
				case ConditionType.GrE:
					if (null == a) return 0 >= Value.ToDecimal();
					return a.Value.ToDecimal() >= Value.ToDecimal();
				case ConditionType.Le:
					if (null == a) return 0 < Value.ToDecimal();
					return a.Value.ToDecimal() < Value.ToDecimal();
				case ConditionType.LeE:
					if (null == a) return 0 <= Value.ToDecimal();
					return a.Value.ToDecimal() <= Value.ToDecimal();
				case ConditionType.InList:
					if (null == a) return false;
					return a.Value.SmartSplit(false, true, ' ', ',', ';').Contains(Value);
				case ConditionType.IverseInList:
					if (null == a) return false;
					return Value.SmartSplit(false, true, ' ', ',', ';').Contains(a.Value);
				case ConditionType.CrossList:
					if (null == a) return false;
					IList<string> l1 = a.Value.SmartSplit(false, true, ' ', ',', ';');
					IList<string> l2 = Value.SmartSplit(false, true, ' ', ',', ';');
					return l1.Intersect(l2).Any();
				case ConditionType.IsTrue:
					if (null == a) return false;
					return a.Value.ToBool();
				case ConditionType.IsNull:
					return a == null;
				case ConditionType.Match:
					if (null == a) return false;
					string a1 = a.Value;
					if (string.IsNullOrWhiteSpace(a1)) return false;
					return Regex.IsMatch(a1, Value);
				case ConditionType.XPath:
					return _e.XPathSelectElements(Value).Any();
				default:
					throw new Exception("unknown condition " + ConditionType);
			}
		}
	}
}