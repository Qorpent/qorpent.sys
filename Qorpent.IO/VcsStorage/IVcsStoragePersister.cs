using System.Collections.Generic;
using System.IO;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    /// 
    /// </summary>
    public interface IVcsStoragePersister {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        VcsCommit Commit(VcsCommit commit, Stream stream);
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        Stream Pick(VcsCommit commit);
        /// <summary>
        ///     Удалить элемент из хранилища
        /// </summary>
        /// <param name="file">Целевой элемент</param>
        void Remove(IFileDescriptor file);
        /// <summary>
        ///     Удалить определённые версии файла из хранилища
        /// </summary>
        /// <param name="commit">Целевой элемент</param>
        /// <param name="commits">Перечисление коммитов</param>
        void Remove(VcsCommit commit, IEnumerable<string> commits);
        /// <summary>
        ///     Подсчитывает количество версий элемента в хранилище
        /// </summary>
        /// <param name="file">Представление файла</param>
        /// <returns></returns>
        int Count(IFileDescriptor file);
        /// <summary>
        ///     проверяет существование файла в хранилище
        /// </summary>
        /// <returns></returns>
        bool Exists(IFileDescriptor file);
    }
}
