using System;

namespace Qorpent.IntermediateFormat {
	/// <summary>
	///		������ ������ ZIF
	/// </summary>
	[Flags]
	public enum IntermediateFormatOutputType {
		/// <summary>
		///		������������� ���������
		/// </summary>
		Undefined = 0,
		/// <summary>
		///		XML
		/// </summary>
		Xml = 1,
		/// <summary>
		///		BXL
		/// </summary>
		Bxl = 2,
		/// <summary>
		///		JSON
		/// </summary>
		Json = 4,
		/// <summary>
		///		�������� �� ��������� <see cref="Xml"/>
		/// </summary>
		Default = Xml
	}
}