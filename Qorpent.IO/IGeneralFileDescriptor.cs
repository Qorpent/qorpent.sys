using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     ���������� �����
    /// </summary>
    public interface IGeneralFileDescriptor {
        /// <summary>
        ///     ������������� �����
        /// </summary>
        IFileEntity FileEntity { get; }
        /// <summary>
        ///     �������� ����� �� �����
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        Stream GetStream(FileAccess access);
    }
}