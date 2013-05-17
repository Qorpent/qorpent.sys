using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// ������� ����� �������� ��� ������ � Wiki
	/// </summary>
	public abstract class WikiActionBase : ActionBase {
		/// <summary>
		/// ������ ��������� Wiki
		/// </summary>
		[Inject]public IWikiSource WikiSource { get; set; }
	}
}