using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Вспомогательные методы файлового хранилища
    /// </summary>
    static class VcsStorageUtils {
        /// <summary>
        ///     Проверяет код коммита на валидность
        /// </summary>
        /// <param name="commit"></param>
        /// <returns>True, если код верен</returns>
        public static bool CorrectCommitCode(VcsCommit commit) {
            return !string.IsNullOrWhiteSpace(commit.Code);
        }
        /// <summary>
        ///     
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Stream StringToStream(string src) {
            return new MemoryStream(Encoding.UTF8.GetBytes(src));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string StreamToString(Stream stream) {
            stream.Position = 0;
            using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                return reader.ReadToEnd();
            }
        }
        /// <summary>
        ///     Пытается открыть поток до файла, если таковой существует. Иначе — null
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Поток или null</returns>
        public static Stream TryOpenStreamToFile(string path) {
            return File.Exists(path) ? File.OpenRead(path) : null;
        }
        /// <summary>
        ///     обёртывает File.WriteAllText для работы через поток
        /// </summary>
        /// <param name="path">Путь для записи</param>
        /// <param name="stream">поток-источник</param>
        public static void WriteAllTextFromStream(string path, Stream stream) {
            File.WriteAllText(path, StreamToString(stream));
        }
        /// <summary>
        ///     Создаёт директорию, если таковая ещё не существует
        /// </summary>
        /// <param name="path">Путь до директории</param>
        public static void CreateDirectoryIfNotExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        ///     Вычисляет SHA 256 от строки
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ComputeShaFromString(string source) {
            using (var stream = StringToStream(source)) {
                var sha = new SHA1Managed();
                var hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
        /// <summary>
        ///     Проверяет, не является ли элемент уже удалённым
        /// </summary>
        /// <param name="element">XML представление элемента</param>
        public static bool IsRemovedElement(this XElement element) {
            var removed = element.Attribute("Removed");

            if (removed != null) {
                if (removed.Value == "true") {
                    return true;
                }
            }

            return false;
        }
    }
}
