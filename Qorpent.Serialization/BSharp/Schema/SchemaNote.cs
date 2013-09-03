using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Замечание по схеме
	/// </summary>
	public class SchemaNote {
		/// <summary>
		/// Уровень ошибки
		/// </summary>
		public ErrorLevel Level { get; set; }
		/// <summary>
		/// Целевой элемент для замечания
		/// </summary>
		public XElement ErrorElement { get; set; }
		/// <summary>
		/// Правило, породившее данное сообщение
		/// </summary>
		public IRule Rule { get; set; }
	}
}