using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// �������� ��������� �������� Wiki
	/// </summary>
	[Action("wiki.exists", Help = "�������� ������� Wiki �� �������� �����")]
	public class WikiExistsAction : WikiActionBase
	{
		/// <summary>
		/// ��� ��� ���� �������, ������� ��������� ���������
		/// </summary>
		[Bind(Required = true)]
		public string Code;
		/// <summary>
		/// ���������� �������� Wiki �� �������
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess()
		{
			return WikiSource.Exists(Code.SmartSplit(false, true, ',').ToArray()).ToArray();
		}
	}
}