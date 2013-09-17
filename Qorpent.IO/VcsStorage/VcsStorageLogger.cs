using System;
using System.IO;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Внутренний журнал транзакций
    /// </summary>
    class VcsStorageLogger : IDisposable {
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
        /// 
        /// </summary>
        private readonly Object _lock = new Object();
        /// <summary>
        ///     Низкойровневый файловый движок
        /// </summary>
        public IFileStorage Engine { get; private set; }
        /// <summary>
        ///     Внутренний журнал транзакций
        /// </summary>
        public VcsStorageLogger(IFileStorage engine) {
            SetEngine(engine);
            InitializeNewBinLog();
        }
        /// <summary>
        ///     Деструктор
        /// </summary>
        ~VcsStorageLogger() {
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
        public void SetEngine(IFileStorage engine) {
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

            BinLog.SetAttributeValue("StartWriting", DateTime.Now);
            BinLog.SetAttributeValue("BinLogGuid", BinLogGuid);
            BinLog.SetAttributeValue("LogElements", 0);
        }
        /// <summary>
        ///     Производит прокатку записи журнала на диск
        /// </summary>
        private void Dump() {
            Engine.Set(
                new FileDescriptor {
                    Path = Path.Combine(VcsStorageDefaults.BinLogDirectory, BinLogGuid + "." + VcsStorageDefaults.BinLogExtension)
                },
                VcsStorageUtils.StringToStream(BinLog.ToString())
            );
        }
        /// <summary>
        ///     Генерирует запись для бинарного журнала о транзакции
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private XElement GenerateNode(VcsStorageTransaction transaction) {
            var node = new XElement("transaction");

            node.SetAttributeValue("Filename", transaction.Filename);
            node.SetAttributeValue("Commit", transaction.Commit.Code);
            node.SetAttributeValue("DateTime", transaction.DateTime);
            node.SetAttributeValue("Type", transaction.Type);
            node.SetAttributeValue("Branch", transaction.Commit.Branch);
            node.SetAttributeValue("Commiter", transaction.Commit.Commiter);

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
