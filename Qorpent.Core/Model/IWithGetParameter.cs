namespace Qorpent.Model {
	/// <summary>
	/// ��������� ��������� ��������� �� �����
	/// </summary>
	public interface IWithGetParameter {
		/// <summary>
		/// �������� �������� ������������ ���������
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetParameter(string name);
	}
}