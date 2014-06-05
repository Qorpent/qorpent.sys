namespace Qorpent.Model{
	/// <summary>
	/// ��� ��������
	/// </summary>
	public enum EvalType{
		/// <summary>
		/// ��� ��������, ������ 0
		/// </summary>
		None = 0,
		/// <summary>
		/// ��������� ��������
		/// </summary>
		Primary = 1,
		/// <summary>
		/// �������� ��������
		/// </summary>
		Sum = 2,
		/// <summary>
		/// ����� �� ������ �������� ������
		/// </summary>
		ReferencedSum = 3,
		/// <summary>
		/// ������� (����� ���������� �� ����������� �������)
		/// </summary>
		Formula = 4,
		/// <summary>
		/// ����������� ����������������
		/// </summary>
		Custom = 5,
	}
}