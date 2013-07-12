using System;
using System.Linq;
using Qorpent.Mvc.Binding;
using Qorpent.Wiki;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// �������� ��������� �������� Wiki
	/// </summary>
	[Action("wiki.save", Help = "��������� ������ � ��������", Role = "DOCWRITER")]
	public class WikiSaveAction : WikiActionBase {
		/// <summary>
		/// ��� ��� ���� �������, ������� ��������� ���������
		/// </summary>
		[Bind(Required = true)]
		public string Code;

		/// <summary>
		/// ��������� ��������, ���
		/// </summary>
		[Bind]
		public string Title;
		/// <summary>
		/// ����� ����� ��������
		/// </summary>
		[Bind] public string Text;

		/// <summary>
		/// ���������� �������� Wiki �� �������
		/// </summary>
		/// <returns></returns>
		protected override object MainProcess() {
			var page = new WikiPage {Code = Code, Title = Title ?? "",Text = Text ?? ""};
			foreach (var parameter in Context.Parameters) {
				if (
					parameter.Key.ToUpper() != "CODE" 
					&& parameter.Key.ToUpper() != "TEXT"
					&& parameter.Key.ToUpper() != "TITLE"
					) {
					page.Propeties[parameter.Key] = parameter.Value;
				}
			}
			var success = WikiSource.Save(page);

            if (!success) {
                throw new Exception("�� �� ����� ���� �� �������������� ��������. �������� �������������.");
            }

			return WikiSource.Get(null,Code).First();
		}
	}
}