using System;
using System.Xml.Linq;

namespace Qorpent.BSharp.Schema {
	/// <summary>
	/// Правило для атрибута
	/// </summary>
	public class AttributeRule:RuleBase {
		/// <summary>
		/// Применить правило к элементу
		/// </summary>
		/// <param name="e"></param>
		public override SchemaNote Apply(XElement e) {
			throw new NotImplementedException();
		}
	}
}