using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Общий интерфейс правила
	/// </summary>
	public interface IRule {
		/// <summary>
		/// Тип правила
		/// </summary>
		RuleType Type { get; set; }
		/// <summary>
		/// Имя цели
		/// </summary>
		string Code { get; set; }
		/// <summary>
		/// Действие
		/// </summary>
		RuleActionType Action { get; set; }
		/// <summary>
		/// Дополнительное значение
		/// </summary>
		string Value { get; set; }
		/// <summary>
		/// Применить правило к элементу
		/// </summary>
		/// <param name="e"></param>
		IEnumerable<SchemaNote> Apply(XElement e);
	}
}