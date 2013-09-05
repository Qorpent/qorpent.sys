using System.Collections.Generic;
using System.IO;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    /// 
    /// </summary>
    public interface IVcsStoragePersister {
        /// <summary>
        ///     Коммит файла в хранилище
        /// </summary>
        /// <param name="element">Путь файла от корня хранилища</param>
        /// <param name="stream">Данные для записи</param>
        /// <returns>Внутренний идентификатор коммита</returns>
        void Commit(IVcsStorageElement element, Stream stream);
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        Stream Pick(IVcsStorageElement element);
        /// <summary>
        ///     Удалить элемент из хранилища
        /// </summary>
        /// <param name="element">Целевой элемент</param>
        void Remove(IVcsStorageElement element);
        /// <summary>
        ///     Удалить определённые версии файла из хранилища
        /// </summary>
        /// <param name="element">Целевой элемент</param>
        /// <param name="commits">Перечисление коммитов</param>
        void Remove(IVcsStorageElement element, IEnumerable<string> commits);
        /// <summary>
        ///     Подсчитывает количество версий элемента в хранилище
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns></returns>
        int Count(IVcsStorageElement element);
        /// <summary>
        ///     проверяет существование файла в хранилище
        /// </summary>
        /// <returns></returns>
        bool Exists(IVcsStorageElement element);
    }
}
