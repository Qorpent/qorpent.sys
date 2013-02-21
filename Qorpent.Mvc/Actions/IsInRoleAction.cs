using Qorpent.Mvc.Binding;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// ��������� ������� ���� � ���������� ������������
	/// </summary>
	[Action("_sys.isinrole",Role="SECURITYMANAGER",Help = "�������� ���� � ������������")]
	public class IsInRoleAction: ActionBase {
		[Bind(Required = true)] private string usr= "";
		[Bind(Required = true)]private string role= "";
		[Bind] private bool exact = false;
		/// <summary>
		/// ���������� ����� IsInRole Application.Roles
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			return Application.Roles.IsInRole(usr, role, exact);
		}
	}
}