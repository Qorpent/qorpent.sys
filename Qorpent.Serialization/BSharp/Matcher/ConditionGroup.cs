using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.BSharp.Matcher{
	/// <summary>
	///     ������ �������
	/// </summary>
	public sealed class ConditionGroup{
		/// <summary>
		///     ������ ��������
		/// </summary>
		/// <param name="e"></param>
		public ConditionGroup(XElement e){
			var conds = new List<SingleCondition>();
			foreach (XAttribute a in e.Attributes()){
				if (a.Name.LocalName == "_file" || a.Name.LocalName == "_line") continue;
				conds.Add(new SingleCondition(a));
			}
			Conditions = conds.ToArray();
		}

		/// <summary>
		///     �������
		/// </summary>
		public SingleCondition[] Conditions { get; set; }

		/// <summary>
		///     �������� ������ �������
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public bool IsMatch(XElement target){
			return Conditions.All(c => c.IsMatch(target));
		}
	}
}