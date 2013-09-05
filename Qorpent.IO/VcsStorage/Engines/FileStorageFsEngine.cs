using System;
using System.IO;

namespace Qorpent.IO.VcsStorage.Engines {
    /// <summary>
    ///     Движок на файловой системе
    /// </summary>
    public class VcsStorageFsEngine : IVcsStorageEngine {
        /// <summary>
        ///     Текущая рабочая директория
        /// </summary>
        public DirectoryInfo WorkingDirectory { get; private set; }
        /// <summary>
        ///     Движок на файловой системе
        /// </summary>
        /// <param name="workingDirectory">рабочая директория</param>
        public VcsStorageFsEngine(DirectoryInfo workingDirectory) {
            WorkingDirectory = workingDirectory;
        }
        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="engineElement">Представление элемента</param>
        public void Set(IVcsStorageEngineElement engineElement) {
            if (!engineElement.StreamAccess.HasFlag(FileAccess.Read)) {
                throw new Exception("Can not access to the stream!");
            }

            RollRealWriting(engineElement);
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="descriptor">Дескриптор элемента</param>
        /// <returns></returns>
        public IVcsStorageEngineElement Get(IVcsStorageElementDescriptor descriptor) {
            return RollRealReading(descriptor);
        }
        /// <summary>
        ///     Прокатака цикла реальной записи на диск
        /// </summary>
        /// <param name="engineElement">Представление элемента</param>
        private void RollRealWriting(IVcsStorageEngineElement engineElement) {
            VcsStorageUtils.CreateDirectoryIfNotExists(
                GenerateElementDirectory(engineElement.Descriptor)    
            );

            VcsStorageUtils.WriteAllTextFromStream(
                GeneratePath(engineElement.Descriptor),
                engineElement.Stream
            );
        }
        /// <summary>
        ///     Реальное чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="descriptor">Дескриптор элемента</param>
        /// <returns></returns>
        private IVcsStorageEngineElement RollRealReading(IVcsStorageElementDescriptor descriptor) {
            return new VcsStorageEngineElement {
                Descriptor = descriptor,
                StreamAccess = FileAccess.Read,
                Stream = VcsStorageUtils.TryOpenStreamToFile(GeneratePath(descriptor))
            };
        }
        /// <summary>
        ///     Генерирует полный путь к директории, в которой располагается
        ///     описываемый дескриптором элемент
        /// </summary>
        /// <param name="descriptor">Дескриптор</param>
        /// <returns></returns>
        private string GenerateElementDirectory(IVcsStorageElementDescriptor descriptor) {
            return Path.Combine(
                WorkingDirectory.FullName,
                descriptor.RelativeDirectory
            );
        }
        /// <summary>
        ///     Генерирует путь до файла
        /// </summary>
        /// <param name="descriptor">Представление элемента</param>
        /// <returns>Полный путь</returns>
        private string GeneratePath(IVcsStorageElementDescriptor descriptor) {
            return Path.Combine(
                GenerateElementDirectory(descriptor),
                descriptor.Filename
            );
        }
    }
}
