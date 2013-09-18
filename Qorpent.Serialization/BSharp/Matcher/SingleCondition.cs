using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Utils.Extensions;
using Qorpent.Serialization;

namespace Qorpent.BSharp.Matcher {
	/// <summary>
	/// Единичное условие
	/// </summary>
	public sealed class SingleCondition {
		private XElement _e;

		/// <summary>
		/// Создает условие из атрибута
		/// </summary>
		/// <param name="a"></param>
		public SingleCondition(XAttribute a) {
			Value = a.Value;
			ConditionType = ConditionType.Eq;
			if ("NULL" == Value) {
				ConditionType = ConditionType.IsNull;
			}
			else if("TRUE" == Value) {
				ConditionType = ConditionType.IsTrue;
			}
			AttributeName = a.Name.LocalName;
			if (a.Name.LocalName.Contains("__")) {
				SetupFromSpecialName(a);
			}
			if (AttributeName == "xpath") {
				ConditionType = ConditionType.XPath;
			}
		}

		private void SetupFromSpecialName(XAttribute a) {
			var n = a.Name.LocalName;
            if (n.Contains(XmlNameEscaper.EscapeXmlName("!")))
            {
				Negate = true;
                n = n.Replace(XmlNameEscaper.EscapeXmlName("!"), "");
			}
			if (Value == "NULL") {
				ConditionType= ConditionType.IsNull;
			}else if (Value == "TRUE") {
				ConditionType =ConditionType.IsTrue;
			}
            if (n.EndsWith(XmlNameEscaper.EscapeXmlName(">>")))
            {
				ConditionType = ConditionType.Gr;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName(">")))
            {
				ConditionType = ConditionType.GrE;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName("<<")))
            {
				ConditionType= ConditionType.Le;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName("<")))
            {
				ConditionType = ConditionType.LeE;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName("~")))
            {
				ConditionType = ConditionType.Match;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName("&")))
            {
				ConditionType = ConditionType.InList;
            }
            else if (n.EndsWith(XmlNameEscaper.EscapeXmlName("%")))
            {
				ConditionType = ConditionType.Contains;
			}
			var idx = n.IndexOf("__");
			if (-1 != idx) {
				n = n.Substring(0, idx);
			}
			AttributeName = n;
			
		}

		/// <summary>
		/// Имя атрибута
		/// </summary>
		public string AttributeName { get; set; }
		/// <summary>
		/// Тип условия
		/// </summary>
		public ConditionType ConditionType { get; set; }
		/// <summary>
		/// Отрицание
		/// </summary>
		public bool Negate { get; set; }
		/// <summary>
		/// Значение
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Проверка условия на целевом элементе
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool IsMatch(XElement e) {
			_e = e;
			var a = e.Attribute(AttributeName);
            if (null == a && AttributeName == "localname") {
                a= new XAttribute("localname",e.Name.LocalName);
            }
			var mainismatch = InternalIsMatch(a);
			return Negate? !mainismatch : mainismatch;
		}

		private bool InternalIsMatch(XAttribute a) {
			
			switch (ConditionType) {
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