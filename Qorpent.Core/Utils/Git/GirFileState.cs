using System;

namespace Qorpent.Utils.Git{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum GirFileState{
		/// <summary>
		/// ��� ���������
		/// </summary>
		None =0,
		/// <summary>
		/// ����������
		/// </summary>
		Modified = 1,
		/// <summary>
		/// �����������
		/// </summary>
		Added =2 ,
		/// <summary>
		/// ���������
		/// </summary>
		Deleted = 4,
		/// <summary>
		///���������������
		/// </summary>
		Renamed =8,
		/// <summary>
		/// �������������
		/// </summary>
		Copied =16,
		/// <summary>
		/// �����������, �� �� ����������
		/// </summary>
		UpdatedButUnmerged =32,
		
	}
}