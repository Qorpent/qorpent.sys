using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Интерфейс плоского файлового хранилища
    /// </summary>
    public interface IPlainFileStorage : IEnumerable<string> {
        /// <summary>
        ///     Директория, на которую нацелено хранилище
        /// </summary>
        string WorkingDirectory { get; set; }
        /// <summary>
        ///     Удаление файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        void Remove(string filename);
        /// <summary>
        ///     Получение строкового содержимого файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Строковое содержимое файла</returns>
        string GetString(string filename);
        /// <summary>
        ///     Получение байтового содержимого файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Байтовое содержимое файла</returns>
        byte[] Get(string filename);
        /// <summary>
        ///     Запись данных в файл в хранилище
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">Строкове денные</param>
        void Write(string filename, string data);
        /// <summary>
        ///     Запись данных в файл в хранилище
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">Байтовые денные</param>
        void Write(string filename, byte[] data);
        /// <summary>
        ///     Получение потока до файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="mode">Режим открытия файла</param>
        /// <param name="access">Режим доступа до файла</param>
        /// <param name="share">Режим совместного использования файла</param>
        /// <returns>Поток до файла</returns>
        FileStream GetStream(string filename, FileMode mode, FileAccess access, FileShare share = FileShare.None);
    }
    /// <summary>
    ///     Обычное плоское файловое хранилище
    /// </summary>
    public class PlainFileStorage : IPlainFileStorage {
        /// <summary>
        ///     Текущая директория
        /// </summary>
        private DirectoryInfo _directory;
        /// <summary>
        ///     Рабочая директория
        /// </summary>
        private string _workingDirectory;
        /// <summary>
        ///     Директория, на которую нацелено хранилище
        /// </summary>
        public string WorkingDirectory {
            get { return _workingDirectory; }
            set {
                value = new FileNameResolver().Resolve(value, false);
                _directory = new DirectoryInfo(value);
                _workingDirectory = value;
            }
        }
        /// <summary>
        ///     Удаление файла
        /// </summary>
        /// <param name="filename">Имя фалйа</param>
        public void Remove(string filename) {
            foreach (var file in _directory.GetFiles(filename)) {
                file.Delete();
            }
        }
        /// <summary>
        ///     Получение строкового содержимого файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Строковое содержимое файла</returns>
        public string GetString(string filename) {
            return Encoding.UTF8.GetString(Get(filename));
        }
        /// <summary>
        ///     Получение байтового содержимого файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <returns>Байтовое содержимое файла</returns>
        public byte[] Get(string filename) {
            var ms = new MemoryStream();
            var stream = GetStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.CopyTo(ms);
            stream.Close();
            return ms.ToArray();
        }
        /// <summary>
        ///     Запись данных в файл в хранилище
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">Строкове денные</param>
        public void Write(string filename, string data) {
            Write(filename, Encoding.UTF8.GetBytes(data));
        }
        /// <summary>
        ///     Запись данных в файл в хранилище
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">Байтовые денные</param>
        public void Write(string filename, byte[] data) {
            using (var stream = GetStream(filename, FileMode.Truncate, FileAccess.Write, FileShare.Read)) {
                stream.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        ///     Получение потока до файла
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="mode">Режим открытия файла</param>
        /// <param name="access">Режим доступа до файла</param>
        /// <param name="share">Режим совместного использования файла</param>
        /// <returns>Поток до файла</returns>
        public FileStream GetStream(string filename, FileMode mode, FileAccess access, FileShare share = FileShare.None) {
            return _directory.GetFiles(filename)[0].Open(mode, access, share);
        }
        /// <summary>
        ///     Получение перечисления имён файлов в хранилище
        /// </summary>
        /// <returns>Перечисление имён файлов</returns>
        public IEnumerable<string> Enumerate() {
            return Enumerate("*");
        }
        /// <summary>
        ///     Получение перечисления имён файлов в хранилище
        /// </summary>
        /// <param name="searchMask">Маска для поиска</param>
        /// <returns>Перечисление имён файлов</returns>
        public IEnumerable<string> Enumerate(string searchMask) {
            return _directory.GetFiles(searchMask).Select(_ => _.Name);
        }
        /// <summary>
        ///     Получение перечисления имён файлов в хранилище
        /// </summary>
        /// <returns>Перечисление имён файлов</returns>
        public IEnumerator<string> GetEnumerator() {
            return Enumerate().GetEnumerator();
        }
        /// <summary>
        ///     Получение перечисления имён файлов в хранилище
        /// </summary>
        /// <returns>Перечисление имён файлов</returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
