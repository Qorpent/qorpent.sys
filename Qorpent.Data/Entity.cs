using System;
using Qorpent.Model;

namespace Zeta.Data.Model {
	/// <summary>
	/// ������� ����� ��������
	/// </summary>
	public class Entity : IEntity {
		/// <summary>
		/// 	������������� ���������� �������������
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	��������� ���������� �������������
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	��������/���
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		/// 	������ �����
		/// </summary>
		public string Tag { get; set; }

		/// <summary>
		/// 	�����������
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 	��������
		/// </summary>
		public DateTime Version { get; set; }
	}
}