using System;
using System.IO;
using System.Text;

namespace Qorpent.IO{
	/// <summary>
	/// ��������� ������ � ����� ��� ��� - ����������
	/// </summary>
	public interface IWebFileRecord{
		/// <summary>
		/// ��������� ��� ������
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// ������ ������
		/// </summary>
		DateTime Version { get; set; }

		/// <summary>
		/// ���-���� ������
		/// </summary>
		string ETag { get; set; }

		/// <summary>
		/// ���������
		/// </summary>
		Encoding Encoding { get; set; }

		/// <summary>
		/// ���������� ���� � ������� �����
		/// </summary>
		/// <param name="output"></param>
		long Write(Stream output);

		/// <summary>
		/// ������� �������� � ���� ������� ����
		/// </summary>
		/// <returns></returns>
		byte[] GetData();

		/// <summary>
		/// ������� �������� � ���� ������
		/// </summary>
		/// <returns></returns>
		string Read();
		/// <summary>
		/// ������ ���
		/// </summary>
		string FullName { get; set; }
		/// <summary>
		/// ������� �������������� ��������
		/// </summary>
		bool IsFixedContent { get; set; }
		/// <summary>
		/// ������������� �������
		/// </summary>
		string FixedContent { get; set; }

		/// <summary>
		/// ������������� �������� ������
		/// </summary>
		byte[] FixedData { get; set; }

		/// <summary>
		/// ��� MIME ��� �����
		/// </summary>
		string MimeType { get; set; }

	    string Role { get; set; }
	    long Length { get; set; }

	    /// <summary>
		/// �������� ������ �� ������
		/// </summary>
		/// <returns></returns>
		Stream Open();
	}
}