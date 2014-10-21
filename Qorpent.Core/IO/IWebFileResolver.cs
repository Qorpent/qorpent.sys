namespace Qorpent.IO{
	/// <summary>
	/// ��������� ���������� ������ ������ ����������
	/// </summary>
	public interface IWebFileResolver{
		/// <summary>
		/// ������������ ����� ��������� ����� �� �������
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IWebFileRecord Find(string search);
		/// <summary>
		/// ������� ��� ����������
		/// </summary>
		void Clear();
		/// <summary>
		/// ������������ ���������� ������
		/// </summary>
		/// <param name="provider"></param>
		void Register(IWebFileProvider provider);
		/// <summary>
		/// ����� ������� ���������� ����������
		/// </summary>
		string Prefix { get; set; }
	}
}