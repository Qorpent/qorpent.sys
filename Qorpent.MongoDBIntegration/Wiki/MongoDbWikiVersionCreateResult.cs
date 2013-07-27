using System;

namespace Qorpent.MongoDBIntegration.Wiki {
    /// <summary>
    ///     Класс, описывающий результат операции создания версии страницы Wiki
    /// </summary>
    public class WikiVersionCreateResult {
        /// <summary>
        ///     Успешность операции?
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        ///     Версия?
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        ///     Дата публикации
        /// </summary>
        public DateTime Published { get; private set; }

        /// <summary>
        ///     Комментарий к версии
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="version"></param>
        /// <param name="published"></param>
        /// <param name="comment"></param>
        public WikiVersionCreateResult(bool isSuccess, string version, DateTime published, string comment) {
            IsSuccess = isSuccess;
            Version = version;
            Published = published;
            Comment = comment;
        }
    }
}