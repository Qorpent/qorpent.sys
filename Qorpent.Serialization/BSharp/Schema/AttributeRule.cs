using System;
using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Правило для атрибута
	/// </summary>
	public class AttributeRule:RuleBase {
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
			throw new NotImplementedException();
		}

		public void Override(AttributeRule target) {
			if (RuleType.None != this.Type) {
				target.Type = this.Type;
			}
			if (RuleActionType.None != this.Action) {
				target.Action = this.Action;
			}
			if (!string.IsNullOrWhiteSpace(Value)) {
				target.Value = this.Value;
			}
		}
	}
}