using System;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum MergeFlags{
		/// <summary>
		/// ���������� ��� �������
		/// </summary>
		FullHistory = 1,
		/// <summary>
		/// ���������� ��� ����� ������� �������
		/// </summary>
		FullLateHistory = 2,
		/// <summary>
		/// ���������� ������ ��������� ������
		/// </summary>
		Snapshot =4,
		/// <summary>
		/// 
		/// </summary>
		Default = FullLateHistory

	}
}