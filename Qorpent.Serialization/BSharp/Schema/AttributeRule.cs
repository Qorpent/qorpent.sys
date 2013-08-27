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
		/// <param name="attr"></param>
		public AttributeRule(XAttribute attr) {
			
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
			throw new NotImplementedException();
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