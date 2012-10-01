using System;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Data {
	/// <summary>
	/// �������� ����������
	/// </summary>
	[Serialize]
	public class ConnectionDescriptor {
		/// <summary>
		/// ��� ����������
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		///  ������ �����������
		/// </summary>
		[SerializeNotNullOnly]
		public string ConnectionString { get; set; }
		/// <summary>
		/// ��� �����������
		/// </summary>
		public Type ConnectionType { get; set; }
		/// <summary>
		/// ��� ����������� � ����������
		/// </summary>
		[SerializeNotNullOnly]
		public string ContainerName { get; set; }
		/// <summary>
		/// True - ������������ ��������� ��� ��������
		/// </summary>
		public bool InstantiateWithContainer { get; set; }

		/// <summary>
		/// ������ �� ���������
		/// </summary>
		[SerializeNotNullOnly]
		public IContainer Container { get; set; }
	}
}