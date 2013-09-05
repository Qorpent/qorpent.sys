using System.IO;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Движок на файловой системе
    /// </summary>
    public class FileStorageFs : IFileStorage {
        /// <summary>
        ///     Текущая рабочая директория
        /// </summary>
        public DirectoryInfo WorkingDirectory { get; private set; }
        /// <summary>
        ///     Поддерживаемый функционал
        /// </summary>
        public FileStorageAbilities Abilities { get; private set; }
        /// <summary>
        ///     Движок на файловой системе
        /// </summary>
        /// <param name="workingDirectory">рабочая директория</param>
        public FileStorageFs(DirectoryInfo workingDirectory) {
            WorkingDirectory = workingDirectory;
            Abilities = FileStorageAbilities.Persist;
        }

        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        public IGeneralFileDescriptor Set(IFileEntity file, Stream stream) {
            return RollRealWriting(file, stream);
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public IGeneralFileDescriptor Get(IFileEntity file) {
            return RollRealReading(file);
        }
        /// <summary>
        ///     Возвращает клас текущего хранилища текущее хранилища
        /// </summary>
        /// <returns></returns>
        public object GetStorage() {
            return this;
        }
        /// <summary>
        ///     Прокатака цикла реальной записи на диск
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        private IGeneralFileDescriptor RollRealWriting(IFileEntity file, Stream stream) {
            VcsStorageUtils.CreateDirectoryIfNotExists(
                GenerateElementDirectory(file)
            );

            VcsStorageUtils.WriteAllTextFromStream(
                GeneratePath(file),
                stream
            );

            return new FsBasedFileDescriptor(FileAccess.Read, new FileEntity { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Реальное чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Дескриптор элемента</param>
        /// <returns></returns>
        private IGeneralFileDescriptor RollRealReading(IFileEntity file) {
            return new FsBasedFileDescriptor(FileAccess.Read, new FileEntity { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Генерирует полный путь к директории, в которой располагается
        ///     описываемый дескриптором элемент
        /// </summary>
        /// <param name="file">Дескриптор</param>
        /// <returns></returns>
        private string GenerateElementDirectory(IFileEntity file) {
            return Path.Combine(WorkingDirectory.FullName, Path.GetDirectoryName(file.Path));
        }
        /// <summary>
        ///     Генерирует путь до файла
        /// </summary>
        /// <param name="file">Представление элемента</param>
        /// <returns>Полный путь</returns>
        private string GeneratePath(IFileEntity file) {
            return Path.Combine(GenerateElementDirectory(file), Path.GetFileName(file.Path));
        }
    }
}
