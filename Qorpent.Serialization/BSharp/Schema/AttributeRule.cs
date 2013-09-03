using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {



	/// <summary>
	/// Правило для атрибута
	/// </summary>
	public class AttributeRule:RuleBase {
		/// <summary>
		/// Правило атрибута
		/// </summary>
		public  AttributeRule(){}
		/// <summary>
		/// Создать из атрибута
		/// </summary>
		public AttributeRule(string name, string val) {
			string code = name;
			var type = RuleType.Allow;
			var action = RuleActionType.None;
			var value = val;
			if (code.StartsWith("+")) {
				code = code.Replace("+", "");
				if (!string.IsNullOrWhiteSpace(value) && value != "1") {
					action = RuleActionType.Rename;
				}
			}else if (code.StartsWith("-")) {
				code = code.Replace("-", "");
				action =RuleActionType.Remove;
			}else if (code.StartsWith("!")) {
				code = code.Replace("!", "");
				type = RuleType.Deny;
			}
			else if (code.StartsWith("@")) {
				code = code.Replace("@", "");
				type = RuleType.Require;
			}
			else if (code.StartsWith("~"))
			{
				code = code.Replace("~", "");
				type = RuleType.Obsolete;
			}
			Code = code;
			Type = type;
			Action = action;
			Value = value;
		}

		/// <summary>
		/// Получить клон правила для атрибута
		/// </summary>
		/// <returns></returns>
		public AttributeRule Clone() {
			return this.MemberwiseClone() as AttributeRule;
		}

		/// <summary>
		/// Применить правило к элементу
		/// </summary>
		/// <param name="e"></param>
		public override IEnumerable<SchemaNote> Apply(XElement e) {
			var r = InternalApply(e);
			if (null == r) return new SchemaNote[] {};
			return r.ToArray();
		}

		private IEnumerable<SchemaNote> InternalApply(XElement e) {
			if (Type == RuleType.Allow) {
				return ProcessAllow(e);
			}
			if (Type == RuleType.Deny) {
				return ProcessDeny(e);
			}
			if (Type == RuleType.Require) {
				return ProcessRequire(e);
			}
			if (Type == RuleType.Obsolete) {
				return ProcessObsolete(e);
			}
			return null;
		}

		private IEnumerable<SchemaNote> ProcessObsolete(XElement e) {
			if (null != e.Attribute(Code)) {
				return new[]{new SchemaNote {ErrorElement = e, Level = ErrorLevel.Warning, Rule = this}};
			}
			return null;
		}

		private IEnumerable<SchemaNote> ProcessRequire(XElement e) {
			ProcessAllow(e);
			if (null == e.Attribute(Code)) {
				e.SetAttributeValue(Code, Value);
				return new[]{new SchemaNote {ErrorElement = e, Level = ErrorLevel.Hint, Rule = this}};
			}
			return null;
		}

		private IEnumerable<SchemaNote> ProcessDeny(XElement e) {
			var a = e.Attribute(Code);
			if (null != a) {
				a.Remove();
				return new[]{new SchemaNote {ErrorElement = e, Level = ErrorLevel.Error, Rule = this}};
			}
			return null;
		}

		private IEnumerable<SchemaNote> ProcessAllow(XElement e) {
			if (Action == RuleActionType.Remove) {
				var a = e.Attribute(Code);
				if (null != a) {
					a.Remove();
				}
			}
			else if (Action == RuleActionType.Rename) {
				var a = e.Attribute(Code);
				if (null != a) {
					e.SetAttributeValue(Value, a.Value);
					a.Remove();
				}
			}
			return null;
		}

		/// <summary>
		/// Перекрывает настройки целевого правила для атрибута
		/// </summary>
		/// <param name="target"></param>
		public void Override(AttributeRule target) {
			if (RuleType.None != Type) {
				target.Type = Type;
			}
			if (RuleActionType.None != Action) {
				target.Action = Action;
			}
			if (!string.IsNullOrWhiteSpace(Value)) {
				target.Value = Value;
			}
		}
	}
}