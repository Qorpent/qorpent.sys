using System.IO;

namespace Qorpent.IO {
    /// <summary>
    /// Расширения для удобной работы с <see cref="IFileStorage"/>
    /// </summary>
    public static class FileStorageExtensions {
        /// <summary>
        /// Читает и возвращает текстовой файл из хранилища
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Read(this IFileStorage storage, string name) {
            return storage.Get(new FileDescriptor {Path = name}).Read();
        }

        /// <summary>
        /// Читает и возвращает текстовой файл из хранилища
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Read(this IFile file) {
            var stream = file.GetStream(FileAccess.Read);
            
            if (stream == null) {
                return null;
            }

            using (var sr = new StreamReader(stream)) {
                return sr.ReadToEnd();
            }
        }
    }
}