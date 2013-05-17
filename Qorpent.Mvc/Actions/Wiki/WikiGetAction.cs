using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// �������� ��������� �������� Wiki
	/// </summary>
	[Action("wiki.get",Help = "�������� Wiki �� �������� �����")]
	public class WikiGetAction:WikiActionBase {
		/// <summary>
		/// ��� ��� ���� �������, ������� ��������� ��������
		/// </summary>
		[Bind(Required = true)] public string Code;
		/// <summary>
		/// ���������� �������� Wiki �� �������
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			return WikiSource.Get(Code.SmartSplit().ToArray()).ToArray();
		}
	}
}