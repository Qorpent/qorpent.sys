using System;

namespace Qorpent.Config {
	/// <summary>
	/// ��� ��������� ������� � ������
	/// </summary>
	[Flags]
	public enum ConfigRenderType {
		/// <summary>
		/// ������� BXL-����������� ������� � ����������
		/// </summary>
		SimpleBxl = 1,
		/// <summary>
		/// �� ���������- ������� BXL
		/// </summary>
		Default =SimpleBxl
	}
}