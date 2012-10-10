namespace Qorpent.Model {
	/// <summary>
	/// 	��������� ��������� - ���-�� � ��������
	/// </summary>
	public interface IWithFormula
	{
		/// <summary>
		/// ������ �������
		/// </summary>
		string Formula { get; set; }
		/// <summary>
		/// ��� �������
		/// </summary>
		string FormulaType { get; set; }
		/// <summary>
		/// ������� ���������� �������
		/// </summary>
		string IsFormula { get; set; }
	}
}