namespace Qorpent.Utils.Git{
	/// <summary>
	/// ������������ ���������
	/// </summary>
	public enum MergeStrategyOption{
		/// <summary>
		/// ��� ��������
		/// </summary>
		None,
		/// <summary>
		/// ��������� ����
		/// </summary>
		Ours,
		/// <summary>
		/// �������� ��������
		/// </summary>
		Theirs,
		/// <summary>
		/// ����������� ����
		/// </summary>
		Patience
	}
}