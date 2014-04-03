using Qorpent.Serialization;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// ���������� ����� ����� ���������
	/// </summary>
	[Serialize]
	public class  RevisionDistance{
		/// <summary>
		/// ������
		/// </summary>
		public int Forward { get; set; }
		/// <summary>
		/// �����
		/// </summary>
		public int Behind { get; set; }
		/// <summary>
		/// ������� ���������� ���������
		/// </summary>
		public bool IsZero{
			get { return Forward == 0 && Behind == 0; }
		}
		/// <summary>
		/// ������� ������, ������� ����� ���� �������� �� ����
		/// </summary>
		public bool IsForwardable{
			get { return Behind == 0 && Forward!=0; }
		}
		/// <summary>
		/// ������� ������, ������� ����� �������������� ��� ������ �������
		/// </summary>
		public bool IsUpdateable{
			get { return Forward == 0 && Behind != 0; }
		}
	}
}