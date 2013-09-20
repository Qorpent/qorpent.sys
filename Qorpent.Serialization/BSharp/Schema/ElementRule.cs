using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Serialization;
using Qorpent.Serialization.Escaping;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Правило элемента
	/// </summary>
	public class ElementRule:RuleBase {
		/// <summary>
		/// 
		/// </summary>
		public ElementRule() {
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		public ElementRule(XElement s) {
			var code = s.GetCode();
			var type = RuleType.Allow;
			if (code.StartsWith("+")) {
				this.ForseStrict = true;
				code = code.Replace("+", "");
			}else if (code.StartsWith("-")) {
				type = RuleType.Deny;
				code = code.Replace("-", "");
			}
			foreach (var a in s.Attributes()) {
				if (a.Name.LocalName == "code") continue;
				if (a.Name.LocalName == "name") {
					AttributeRules.Add(new AttributeRule(a.Value, "1"));
				}
				else {
					var acode = a.Name.LocalName;
					acode =
                        acode.Replace("+".Escape(EscapingType.XmlName), "+")
                             .Replace("-".Escape(EscapingType.XmlName), "-")
                             .Replace("@".Escape(EscapingType.XmlName), "@")
                             .Replace("!".Escape(EscapingType.XmlName), "!")
                             .Replace("~".Escape(EscapingType.XmlName), "~");
					AttributeRules.Add(new AttributeRule(acode, a.Value));
				}
			}

			Code = code;
			Type = type;

		}
		/// <summary>
		/// Признак того, что данное правило форсирует ограниченный режим
		/// </summary>
		public bool ForseStrict { get; set; }

		private List<AttributeRule> _attributeRules = new List<AttributeRule>();
		/// <summary>
		/// Дочерние правила для атрибутов
		/// </summary>
		public List<AttributeRule> AttributeRules {
			get { return _attributeRules; }
		}

		private List<ElementRule> _elementRules = new List<ElementRule>();
		/// <summary>
		/// Дочерние правила для атрибутов
		/// </summary>
		public List<ElementRule> ElementRules
		{
			get { return _elementRules; }
		}
		



		/// <summary>
		/// Клонирует элемент
		/// </summary>
		/// <returns></returns>
		public ElementRule Clone() {
			var result = MemberwiseClone() as ElementRule;
			result._elementRules = new List<ElementRule>();
			result._attributeRules = new List<AttributeRule>();
			foreach (var _er in this.ElementRules) {
				result._elementRules.Add(_er.Clone());
			}
			foreach (var _ar in this.AttributeRules) {
				result._attributeRules.Add(_ar.Clone());
			}
			return result;
		}
		/// <summary>
		/// Перекрытие целевого правила
		/// </summary>
		/// <param name="targetRule"></param>
		/// <returns></returns>
		public void Override(ElementRule targetRule) {
			if (RuleType.None != Type) {
				targetRule.Type = Type;
			}
			if (!string.IsNullOrWhiteSpace(Code)) {
				targetRule.Code=Code;
			}
			if (RuleActionType.None != this.Action)
			{
				targetRule.Action = this.Action;
			}
			if (!string.IsNullOrWhiteSpace(Value))
			{
				targetRule.Value = this.Value;
			}
			foreach (var e in ElementRules) {
				var target = targetRule.ElementRules.FirstOrDefault(_ => _.Code == e.Code);
				if (null == target) {
					targetRule.ElementRules.Add(e.Clone());
				}
				else {
					e.Override(target);
				}
			}

			foreach (var a in AttributeRules) {
				var target = targetRule.AttributeRules.FirstOrDefault(_ => _.Code == a.Code);
				if (null == target) {
					targetRule.AttributeRules.Add(a.Clone());
				}
				else {
					a.Override(target);
				}
			}


		}


		/// <summary>
		/// Применить правило к элементу
		/// </summary>
		/// <param name="e"></param>
		public override IEnumerable<SchemaNote> Apply(XElement e) {
			return InternalApply(e).ToArray();
		}

		private IEnumerable<SchemaNote> InternalApply(XElement e) {
			if (Type == RuleType.Deny) {
				e.Remove();
				yield break;
			}
			bool strict = AttributeRules.Any(_ => _.Type == RuleType.Allow);
			if (strict) {
				e.Attributes().Where(_ => !AttributeRules.Any(__ => __.Code == _.Name.LocalName)).Remove();
			}
			foreach (var a in AttributeRules) {
				var asn = a.Apply(e);
				if (null != asn) {
					foreach (var asn_ in asn) yield return asn_;
				}
			}
		}
	}
}