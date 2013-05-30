namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// ��� ������������� �������� SQL
	/// </summary>
	public enum SqlCommandType {
		/// <summary>
		/// ����� ��������� (SELECT func, EXEC proc)
		/// </summary>
		Call,
		/// <summary>
		/// ��������� ��������� ������� EXEC proc, SELECT * FROM func
		/// </summary>
		Select,
	}
}