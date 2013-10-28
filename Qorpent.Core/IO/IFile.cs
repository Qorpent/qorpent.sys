using System.IO;

namespace Qorpent.IO {
    /// <summary>
    ///     ���������� �����
    /// </summary>
    public interface IFile {
        /// <summary>
        ///     ������������� �����
        /// </summary>
        IFileDescriptor Descriptor { get; }
        /// <summary>
        ///     �������� ����� �� �����
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        Stream GetStream(FileAccess access);
    }
}