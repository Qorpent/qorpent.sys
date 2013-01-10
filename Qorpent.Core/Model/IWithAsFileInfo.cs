using System;

namespace Qorpent.Model {
	/// <summary>
	/// ��������� �-�� �������� ���������� ��� ��������� - ���������, ��������� ��������, ����� �������� � ������
	/// </summary>
	public interface IWithAsFileInfo {
		/// <summary>
		/// ���������, �������� ��������
		/// </summary>
		string Owner { get; set; }

		/// <summary>
		/// ��������� �������, �������� ��������
		/// </summary>
		string Updater { get; set; }

		/// <summary>
		/// ����� �������� ��������
		/// </summary>
		DateTime Created { get; set; }

		/// <summary>
		/// ����� ������ ��������
		/// </summary>
		DateTime Updated { get; set; }
	}
}