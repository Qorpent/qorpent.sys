namespace Qorpent.IO{
	/// <summary>
	/// ����� ������ ����� � ����������
	/// </summary>
	public enum WebFileSerachMode{
		/// <summary>
		/// ������
		/// </summary>
		Exact = 1,
		/// <summary>
		/// ������� ������� ������ �� ������� ����, ����� �� �����
		/// </summary>
		ExactThenIgnore = 2,
		/// <summary>
		/// � �������������� ����
		/// </summary>
		IgnorePath = 4
	}
}