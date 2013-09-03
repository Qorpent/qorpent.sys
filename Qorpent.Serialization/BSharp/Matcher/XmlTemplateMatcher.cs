using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp.Matcher
{
	/// <summary>
	/// Хелпер класс сопоставления XElement с образцом и целевым с 
	/// выдачей ответа соответствия или не соответствия
	/// </summary>
	public sealed class XmlTemplateMatcher
	{
		private ConditionGroup[] _groups;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="condition"></param>
		public XmlTemplateMatcher(XElement condition) {
			_groups =new[]{ new ConditionGroup(condition)};
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="conditions"></param>
		public XmlTemplateMatcher(IEnumerable<XElement> conditions) {
			_groups = conditions.Select(_ => new ConditionGroup(_)).ToArray();
		}
		/// <summary>
		/// Проверяет соответствие элемента заданному шаблону
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool IsMatch(XElement e) {
			return _groups.Any(_ => _.IsMatch(e));
		}

	}
}
