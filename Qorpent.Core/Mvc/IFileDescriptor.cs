using System;

namespace Qorpent.Mvc {
	/// <summary>
	/// ��������� ����������� ������������ �����
	/// </summary>
	public interface IFileDescriptor : IWithRole {
		/// <summary>
		/// ��� ������� �������� � �����
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// ���������� �����
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// Mime-Type �����
		/// </summary>
		string MimeType { get; set; }

		/// <summary>
		/// ����� �����
		/// </summary>
		int Length { get; set; }

		/// <summary>
		/// ����� ���������� ���������
		/// </summary>
		DateTime LastWriteTime { get; set; }
	}
}