using System.Collections.Generic;

namespace Qorpent.IO {
    /// <summary>
    /// ��������� ��������� ����, ����������� �� ������� ���������
    /// </summary>
    public interface IFileCacheResolver {
        /// <summary>
        /// ���������� ���������� ����
        /// </summary>
        string Root { get; set; }

        /// <summary>
        /// ��������� ������
        /// </summary>
        IList<IFileCacheSource> Sources { get; }

        /// <summary>
        /// ��� �����, ������������� ��� ������������� ��������������� ���������� (������ null)
        /// </summary>
        string Fallback { get; set; }

        /// <summary>
        /// �������
        /// </summary>
        IList<IFileFilter> Filters { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Resolve(string name, bool forceUpdate = false);
    }
}