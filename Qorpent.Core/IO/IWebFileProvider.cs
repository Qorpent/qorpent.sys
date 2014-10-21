namespace Qorpent.IO{
	/// <summary>
	/// ��������� ���������� ������ ��� ���-����������
	/// </summary>
	public interface IWebFileProvider{
		/// <summary>
		/// ����� ���� � ��������� ������
		/// </summary>
		/// <param name="file"></param>
		/// <param name="searchMode"></param>
		/// <returns></returns>
		IWebFileRecord Find(string file, WebFileSerachMode searchMode = WebFileSerachMode.Exact);
		/// <summary>
		/// ��������� ������������ ������ ��� ������� ����������
		/// </summary>
		/// <param name="nsearch"></param>
		/// <returns></returns>
		bool IsMatch(string nsearch);
		/// <summary>
		/// ������� ���� ������ ��� ����������� ��������� �����
		/// </summary>
		string Prefix { get; set; }
	}
}