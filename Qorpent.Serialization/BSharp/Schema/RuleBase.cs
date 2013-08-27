using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Базис правила
	/// </summary>
	public abstract class RuleBase:IRule {
		/// <summary>
		/// Тип правила
		/// </summary>
		public RuleType Type { get; set; }

		/// <summary>
		/// Имя цели
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// Действие
		/// </summary>
		public RuleActionType Action { get; set; }

		/// <summary>
		/// Дополнительное значение
		/// </summary>
		public string Value { get; set; }

		/// <summary>
		/// Применить правило к элементу
		/// </summary>
		/// <param name="e"></param>
		public abstract SchemaNote Apply(XElement e);
	}
}