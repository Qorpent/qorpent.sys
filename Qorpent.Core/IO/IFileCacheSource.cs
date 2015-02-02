using System;
using System.IO;

namespace Qorpent.IO {
    /// <summary>
    /// ��������� ��������� ������ ��� �������� ���������� ������
    /// </summary>
    public interface IFileCacheSource {
        /// <summary>
        /// �������� ���������� ��� URL
        /// </summary>
        string Root { get; set; }
        /// <summary>
        /// ������� "��������" ���������
        /// </summary>
        bool IsMaster { get; set; }
        /// <summary>
        /// ���������� NULL ��� �������� ������ ��� ������� ������������ �����
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Func<Stream> GetStreamer(string name);
    }
}