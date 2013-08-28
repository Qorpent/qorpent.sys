using System;
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
			if (code.StartsWith("__PLUS__")) {
				code = code.Replace("__PLUS__", "");
				if (!string.IsNullOrWhiteSpace(value) && value != "1") {
					action = RuleActionType.Rename;
				}
			}else if (code.StartsWith("__MINUS__")) {
				code = code.Replace("__PLUS__", "");
				type = RuleType.Deny;
			}else if (code.StartsWith("__EXC__")) {
				code = code.Replace("__EXC__", "");
				type = RuleType.Require;
			}
			else if (code.StartsWith("__TILDA__"))
			{
				code = code.Replace("__TILDA__", "");
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
		public override SchemaNote Apply(XElement e) {
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

		private SchemaNote ProcessObsolete(XElement e) {
			if (null != e.Attribute(Code)) {
				return new SchemaNote {ErrorElement = e, Level = ErrorLevel.Warning, Rule = this};
			}
			return null;
		}

		private SchemaNote ProcessRequire(XElement e) {
			ProcessAllow(e);
			if (null == e.Attribute(Code)) {
				e.SetAttributeValue(Code, Value);
				return new SchemaNote {ErrorElement = e, Level = ErrorLevel.Hint, Rule = this};
			}
			return null;
		}

		private SchemaNote ProcessDeny(XElement e) {
			var a = e.Attribute(Code);
			if (null != a) {
				a.Remove();
				return new SchemaNote {ErrorElement = e, Level = ErrorLevel.Error, Rule = this};
			}
			return null;
		}

		private SchemaNote ProcessAllow(XElement e) {
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