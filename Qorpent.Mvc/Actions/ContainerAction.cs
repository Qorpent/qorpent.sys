using System.Linq;
using Qorpent.IoC;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 	���������� ������� ����������������� �������� (������ XML)
	/// </summary>
	[Action("_sys.container", Role = "DEVELOPER", Help = "���������� ������ ��������� ����������� � ����������", Arm = "admin")]
	public class ContainerAction : ActionBase
	{
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			return Container.GetComponents().Select(_ => new ComponentDefinitionWrapper(_)).ToArray();
		}
	}
}