<<<<<<< HEAD
﻿using System.IO;
using Qorpent.IO.FileDescriptors;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Движок на файловой системе
    /// </summary>
    public class FileStorageFs : IFileStorageExtended {
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
        /// <param name="workingDirectory">Рабочая директория</param>
        public FileStorageFs(DirectoryInfo workingDirectory) {
            WorkingDirectory = workingDirectory;
            Abilities = FileStorageAbilities.Persist;
        }

        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток до файла</param>
        public IGeneralFileDescriptor Set(IFileEntity file, Stream stream) {
            return RollRealWriting(file, stream);
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns>Дескриптор файла</returns>
        public IGeneralFileDescriptor Get(IFileEntity file) {
            return RollRealReading(file);
        }
        /// <summary>
        ///     Производит удаление файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        public void Del(IFileEntity file) {
            RollRealDeleting(file);
        }
        /// <summary>
        ///     Возвращает клас текущего хранилища текущее хранилища
        /// </summary>
        /// <returns>Класс-хранилище</returns>
        public object GetStorage() {
            return this;
        }
        /// <summary>
        ///     Прокатака цикла реальной записи на диск
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток до файла</param>
        private IGeneralFileDescriptor RollRealWriting(IFileEntity file, Stream stream) {
            VcsStorageUtils.CreateDirectoryIfNotExists(
                GenerateElementDirectory(file)
            );

            VcsStorageUtils.WriteAllTextFromStream(
                GeneratePath(file),
                stream
            );

            return new FileDescriptorFsBased(FileAccess.Read, new FileEntity { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Реальное чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Дескриптор элемента</param>
        /// <returns>Дескриптор файла</returns>
        private IGeneralFileDescriptor RollRealReading(IFileEntity file) {
            return new FileDescriptorFsBased(FileAccess.Read, new FileEntity { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Производит прокат реального удаления файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        private void RollRealDeleting(IFileEntity file) {
            File.Delete(GeneratePath(file));
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
=======
﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.IO.FileDescriptors;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Storages {
    /// <summary>
    ///     Движок на файловой системе
    /// </summary>
    public class FileStorageFs : IFileStorageExtended {
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
        /// <param name="workingDirectory">Рабочая директория</param>
        public FileStorageFs(DirectoryInfo workingDirectory) {
            WorkingDirectory = workingDirectory;
            Abilities = FileStorageAbilities.Persist;
        }

        /// <summary>
        ///     Запись элемента в низкоуровневое хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток до файла</param>
        public IFile Set(IFileDescriptor file, Stream stream) {
            return RollRealWriting(file, stream);
        }
        /// <summary>
        ///     Чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns>Дескриптор файла</returns>
        public IFile Get(IFileDescriptor file) {
            return RollRealReading(file);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IEnumerable<IFile> EnumerateFiles(FileSearchOptions options = null) {
            var files = WorkingDirectory.Exists ? WorkingDirectory.EnumerateFiles("*", SearchOption.AllDirectories) : new List<FileInfo>();

            foreach (var enumerateFile in files) {
                var name = enumerateFile.FullName.Replace(WorkingDirectory.FullName+"\\", "");
                var f = new FileFsBased(FileAccess.ReadWrite,new FileDescriptor{DateTime = enumerateFile.LastWriteTime,Filename = name,Path = enumerateFile.FullName});
                yield return f;
            }
        }

        /// <summary>
        ///     Производит удаление файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        public void Del(IFileDescriptor file) {
            RollRealDeleting(file);
        }
        /// <summary>
        ///     Возвращает клас текущего хранилища текущее хранилища
        /// </summary>
        /// <returns>Класс-хранилище</returns>
        public object GetUnderlinedStorage() {
            return this;
        }
        /// <summary>
        ///     Вывод файлов в директории
        /// </summary>
        /// <param name="pattern">Шаблон для поиска</param>
        /// <returns>Массив имён</returns>
        public string[] List(string pattern) {
            var di = new DirectoryInfo(WorkingDirectory.FullName);
            if (!di.Exists) {
                return new string[] {};
            }

            return di.GetFiles(pattern).Select(el => el.Name).ToArray();
        }
        /// <summary>
        ///     Прокатака цикла реальной записи на диск
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <param name="stream">Поток до файла</param>
        private IFile RollRealWriting(IFileDescriptor file, Stream stream) {
            VcsStorageUtils.CreateDirectoryIfNotExists(
                GenerateElementDirectory(file)
            );

            VcsStorageUtils.WriteAllTextFromStream(
                GeneratePath(file),
                stream
            );

            return new FileFsBased(FileAccess.Read, new FileDescriptor { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Реальное чтение элемента из низкоуровневого хранилища
        /// </summary>
        /// <param name="file">Дескриптор элемента</param>
        /// <returns>Дескриптор файла</returns>
        private IFile RollRealReading(IFileDescriptor file) {
            return new FileFsBased(FileAccess.Read, new FileDescriptor { Path = GeneratePath(file) });
        }
        /// <summary>
        ///     Производит прокат реального удаления файла из хранилища
        /// </summary>
        /// <param name="file">Представление файла</param>
        private void RollRealDeleting(IFileDescriptor file) {
            File.Delete(GeneratePath(file));
        }
        /// <summary>
        ///     Генерирует полный путь к директории, в которой располагается
        ///     описываемый дескриптором элемент
        /// </summary>
        /// <param name="file">Дескриптор</param>
        /// <returns></returns>
        private string GenerateElementDirectory(IFileDescriptor file) {
            return Path.Combine(WorkingDirectory.FullName, Path.GetDirectoryName(file.Path));
        }
        /// <summary>
        ///     Генерирует путь до файла
        /// </summary>
        /// <param name="file">Представление элемента</param>
        /// <returns>Полный путь</returns>
        private string GeneratePath(IFileDescriptor file) {
            return Path.Combine(GenerateElementDirectory(file), Path.GetFileName(file.Path));
        }
    }
}
>>>>>>> origin/master
