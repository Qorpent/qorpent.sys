using System.Collections.Generic;

namespace Qorpent.Data {
	/// <summary>
	/// ���������� ���������� ����������
	/// </summary>
	public interface IDatabaseConnectionProviderExtension {
		/// <summary>
		/// ���������� ��������� ������������������ ������ ����������� ����������
		/// </summary>
		/// <returns></returns>
		IEnumerable<ConnectionDescriptor> GetConnections();
		
	}
}