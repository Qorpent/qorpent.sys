using System;
using System.Data.SqlClient;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Data {
	/// <summary>
	/// �������� ����������
	/// </summary>
	[Serialize]
	public class ConnectionDescriptor {
		/// <summary>
		/// 
		/// </summary>
		public ConnectionDescriptor() {
			ConnectionType = typeof (SqlConnection);
		}
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
		/// �������� ��� ������������
		/// </summary>
		public string ConnectionTypeName {get { return ConnectionType.AssemblyQualifiedName; }}

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

		/// <summary>
		/// ������������� ������ � ����������
		/// </summary>
		public string Evidence { get; set; }
	}
}