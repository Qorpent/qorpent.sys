using System;
using System.IO;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутренний журнал транзакций
    /// </summary>
    class VcsStorageBinLog : IDisposable {
        /// <summary>
        ///     Указатель на то, что клас уже разрушен
        /// </summary>
        private bool _disposed;
        /// <summary>
        ///     Внутренний журнал транзакций
        /// </summary>
        private XElement BinLog { get; set; }
        /// <summary>
        ///     Текущий уникальный идентификатор журнала
        /// </summary>
        private Guid BinLogGuid { get; set; }
        /// <summary>
        ///     Дата начала записи в лог
        /// </summary>
        private DateTime BinLogStartWriting { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private readonly Object _lock = new Object();
        /// <summary>
        ///     Низкойровневый файловый движок
        /// </summary>
        public IVcsStorageEngine Engine { get; private set; }
        /// <summary>
        ///     Директория для хранения журналов транзакций
        /// </summary>
        public DirectoryInfo BinLogStorage { get; private set; }
        /// <summary>
        ///     Внутренний журнал транзакций
        /// </summary>
        public VcsStorageBinLog(IVcsStorageEngine engine) {
            SetEngine(engine);
            InitializeNewBinLog();
        }
        /// <summary>
        ///     Деструктор
        /// </summary>
        ~VcsStorageBinLog() {
            Dispose(false);
        }
        /// <summary>
        ///     Внешний диспоуз
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }       
        /// <summary>
        ///     Коммит тринзакции в бинарный журнал
        /// </summary>
        /// <param name="transaction"></param>
        public void Transaction(VcsStorageTransaction transaction) {
            lock (_lock) {
                IncrementElements();
                BinLog.Add(GenerateNode(transaction));
            }
        }
        /// <summary>
        ///     Принудительно сбрасывает бинарный журнал на диск и
        ///     пересоздаёт новую инстанцию бинарного журнала в классе
        /// </summary>
        public void Recycle() {
            Dump();
            InitializeNewBinLog();
        }
        /// <summary>
        ///     Установить движок
        /// </summary>
        /// <param name="engine"></param>
        public void SetEngine(IVcsStorageEngine engine) {
            Engine = engine;
        }
        /// <summary>
        ///     Внутренний деструктор
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    Shutdown();
                }

                _disposed = true;
            }
        }
        /// <summary>
        ///     Инициализирует новую инстанцию бинарного журнала
        /// </summary>
        private void InitializeNewBinLog() {
            BinLog = new XElement("BinLog");
            BinLogGuid = Guid.NewGuid();
            BinLogStartWriting = DateTime.Now;

            BinLog.SetAttributeValue("StartWriting", BinLogStartWriting);
            BinLog.SetAttributeValue("BinLogGuid", BinLogGuid);
            BinLog.SetAttributeValue("LogElements", 0);
        }
        /// <summary>
        ///     Производит прокатку записи журнала на диск
        /// </summary>
        private void Dump() {
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = BinLogGuid + "." + VcsStorageDefaults.BinLogExtension,
                    RelativeDirectory = VcsStorageDefaults.BinLogDirectory
                },
                Stream = VcsStorageUtils.StringToStream(BinLog.ToString()),
                StreamAccess = FileAccess.Read
            });
        }
        /// <summary>
        ///     Генерирует запись для бинарного журнала о транзакции
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private XElement GenerateNode(VcsStorageTransaction transaction) {
            var node = new XElement("transaction");

            node.SetAttributeValue("Filename", transaction.Filename);
            node.SetAttributeValue("Guid", transaction.Commit);
            node.SetAttributeValue("DateTime", transaction.DateTime);
            node.SetAttributeValue("Type", transaction.Type);

            return node;
        }
        /// <summary>
        ///     Увеличивает внутренний счётчик количества элементов на единицу
        /// </summary>
        private void IncrementElements() {
            BinLog.SetAttributeValue("LogElements", BinLog.Attribute("LogElements").Value.ToInt() + 1);
        }
        /// <summary>
        ///     Завершение цикла работы
        /// </summary>
        private void Shutdown() {
            Dump();
        }
    }
}
