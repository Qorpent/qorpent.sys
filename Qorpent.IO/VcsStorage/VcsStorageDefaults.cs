
namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Некоторые настройки по-умолчанию для хранилища
    /// </summary>
    static class VcsStorageDefaults {
        /// <summary>
        ///     Имя директории для журналовв транзакций
        /// </summary>
        public const string BinLogDirectory = ".logs";
        /// <summary>
        ///     Имя директории для хранения файлов маппинга
        /// </summary>
        public const string MapFilesDirectory = ".maps";
        /// <summary>
        ///     Директория с объектами
        /// </summary>
        public const string ObjFilesDirectory = ".objs";
        /// <summary>
        ///     Расширение файла с маппингом по умолчанию
        /// </summary>
        public const string MapFileExtension = "Map.xml";
        /// <summary>
        ///     Расширение файла с журналом по умолчанию
        /// </summary>
        public const string BinLogExtension = "BinLog.xml";
        /// <summary>
        ///     Имя бранча по умолчанию
        /// </summary>
        public const string DefaultBranch = "Master";
    }
}
