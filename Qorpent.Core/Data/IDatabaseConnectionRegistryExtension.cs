using System;

namespace Qorpent.Data {
	/// <summary>
	/// ���������� ���������� ����������
	/// </summary>
	public interface IDatabaseConnectionRegistryExtension {
		
		/// <summary>
		/// ������� ������ ����������� � �����������
		/// </summary>
		/// <param name="name"></param>
		void Unregister(string name );
		/// <summary>
		/// ���������������� ���������� � ��
		/// </summary>
		
		void Register(ConnectionDescriptor connectionDescriptor);
	}
}