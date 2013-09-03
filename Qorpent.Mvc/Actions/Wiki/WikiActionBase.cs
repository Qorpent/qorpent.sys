using System.Linq;
using Qorpent.IoC;
using Qorpent.Mvc.Binding;
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

	/// <summary>
	/// ������� ����� �������� ��� ������ � Wiki
	/// </summary>
	[Action("wiki.find", Help = "������������ ����� ��������")]
	public class WikiFind : WikiActionBase {
		/// <summary>
		/// ������ ������
		/// </summary>
		[Bind] public string Search { get; set; }
		/// <summary>
		/// ��������� ������ ��������� ��������
		/// </summary>
		[Bind(Default = -1)]public int Start { get; set; }
		/// <summary>
		/// ����� ����������� ������
		/// </summary>	
		[Bind(Default = -1)]public int Count { get; set; }

		/// <summary>
		/// ����� ����������� ������
		/// </summary>	
		[Bind(Default = true)]public bool Files { get; set; }
		/// <summary>
		/// ����� ����������� ������
		/// </summary>	
		[Bind(Default = true)]public bool Pages { get; set; }

		/// <summary>
		/// ���������� ��������� �����������
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			var type = WikiObjectType.None;
			if (Files) type = type | WikiObjectType.File;
			if (Pages) type = type | WikiObjectType.Page;
            if (string.IsNullOrEmpty(Search)) Search = "*";
			return WikiSource.Find(Search, Start, Count, type).ToArray()
			;
		}
		
	}
}