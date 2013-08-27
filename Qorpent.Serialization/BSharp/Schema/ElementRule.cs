using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Правило элемента
	/// </summary>
	public class ElementRule:RuleBase {

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
		public ElementRule Override(ElementRule targetRule) {
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
		public override SchemaNote Apply(XElement e) {
			throw new NotImplementedException();
		}
	}
}