using System.Collections.Generic;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// ��������� �����-�� ������ ��� �����
	/// </summary>
	public interface ICustomMerger{
		/// <summary>
		/// ���������� ������ � ������� ��, ������� ������������� ��������� � ��������
		/// </summary>
		/// <param name="basedelta"></param>
		/// <returns></returns>
		IEnumerable<MetaFileRegistryDelta> Merge(IEnumerable<MetaFileRegistryDelta> basedelta);
	}
}