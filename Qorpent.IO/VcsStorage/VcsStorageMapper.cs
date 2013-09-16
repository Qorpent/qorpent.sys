using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    ///     Маппер обеспечивает поддержание хранилище
    ///     в порядке с учётом версионности
    /// </summary>
    class VcsStorageMapper : IDisposable {
        /// <summary>
        ///     
        /// </summary>
        private readonly Object _lock = new Object();
        /// <summary>
        ///     Текущая карта хранилища
        /// </summary>
        private XElement Map { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IFileStorage Engine { get; private set; }
        /// <summary>
        ///     Указатель на то, что класс уже разрушен
        /// </summary>
        private bool _disposed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        public VcsStorageMapper(IFileStorage engine) {
            SetEngine(engine);
            Initialize();
        }
        /// <summary>
        ///     Деструктор
        /// </summary>
        ~VcsStorageMapper() {
            Dispose(false);
        }
        /// <summary>
        ///     Деструктор
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        ///     Транзакция: операции вставки или удаления
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="transactionType"></param>
        public void Transaction(VcsCommit commit, VcsStorageTransactionType transactionType) {
            lock (_lock) {
                switch (transactionType) {
                    case VcsStorageTransactionType.Commit: Insert(commit); break;
                    case VcsStorageTransactionType.Remove: Remove(commit); break;
                }
            }
        }
        /// <summary>
        ///     Поиск представления элемента в хранилище
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VcsCommit> Find(VcsCommit commit) {
            return Find(commit, true);
        }
        /// <summary>
        ///     Установить движок
        /// </summary>
        /// <param name="engine"></param>
        public void SetEngine(IFileStorage engine) {
            Engine = engine;
        }
        /// <summary>
        ///     Поиск представления элемента в хранилище
        /// </summary>
        /// <param name="commit"></param>
        /// <param name="excludeRemoved"></param>
        /// <returns></returns>
        private IEnumerable<VcsCommit> Find(VcsCommit commit, bool excludeRemoved) {
            var container = GetElement(commit);

            if (container == null) {
                return new List<VcsCommit>();
            }

            return new List<VcsCommit>(container.Descendants("Commit").Where(
                el => el.IsRemovedElement() == !excludeRemoved
            ).Select(
                el => new VcsCommit {
                    Code = el.Attribute("Code").Value,
                    File = new FileEntity {
                        Version = el.Attribute("Code").Value,
                        Path = container.Attribute("Filename").Value,
                        Filename = Path.GetFileName(container.Attribute("Filename").Value),
                        DateTime = DateTime.Parse(el.Attribute("DateTime").Value),
                        Owner = el.Attribute("Commiter").Value
                    }
                }
            ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commit"></param>
        public void Insert(VcsCommit commit) {
            var container = GetElement(commit);

            if (container == null) {
                container = new XElement("Element");
                container.SetAttributeValue("Filename", commit.File.Path);
                container.SetAttributeValue("Branch", commit.Branch);
                container.SetAttributeValue("TotalCommits", 0);
                Map.Add(container);
            }

            var xmlCommit = new XElement("Commit");
            xmlCommit.SetAttributeValue("Code", commit.Code);
            xmlCommit.SetAttributeValue("DateTime", DateTime.Now);
            xmlCommit.SetAttributeValue("Commiter", commit.Commiter);

            IncrementElementCommits(container);
            container.SetAttributeValue("LastCommit", commit.Code);
            container.AddFirst(xmlCommit);
        }
        /// <summary>
        ///     Удаляет элемент из маппинга. Если указан коммит, то только 
        ///     данный коммит, в противном случае - весь элемент
        /// </summary>
        /// <param name="commit">Представление коммита</param>
        public void Remove(VcsCommit commit) {
            var container = GetElement(commit);

            if (!VcsStorageUtils.CorrectCommitCode(commit)) {
                container.SetAttributeValue("Removed", true);
            } else {
                var xmlCommit = container.XPathSelectElement("/Element/Commit[@Code='" + commit.Code + "']");
                if (xmlCommit != null) {
                    xmlCommit.SetAttributeValue("Removed", true);
                }

                if (!container.Elements().All(el => el.IsRemovedElement())) {
                    container.SetAttributeValue("Removed", true);
                }
            }
        }
        /// <summary>
        ///     Проверяет существование элемента в маппинге. Если указан коммит, 
        ///     то проверяет жёстко по коммиту
        /// </summary>
        /// <param name="commit">Представление элемента</param>
        /// <returns></returns>
        public bool Exists(VcsCommit commit) {
            var found = Find(commit);

            if (!found.Any()) {
                return false;
            }
            // если код коммита имеет некорректную форму или не указан, а элемент
            // найден - просто вернём true
            if (!VcsStorageUtils.CorrectCommitCode(commit)) {
                return true;
            }

            return found.Any(c => c.Code == commit.Code);
        }
        /// <summary>
        ///     Подсчитывает количество версий файла
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int Count(IFileEntity file) {
            return Find(new VcsCommit {File = file}).Count();
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
        ///     Возвращает описание элемента из карты, если таковой существует
        /// </summary>
        /// <param name="commit">Представление элемента</param>
        /// <returns>XML-контейнер элемента</returns>
        private XElement GetElement(VcsCommit commit) {
            return Map.XPathSelectElement("/Element[@Filename='" + commit.File.Path + "' and @Branch='" + commit.Branch + "']");
        }
        /// <summary>
        ///     Увеличивает счётчик коммитов в контейнере
        /// </summary>
        /// <param name="container"></param>
        private void IncrementElementCommits(XElement container) {
            container.SetAttributeValue("TotalCommits", container.Attribute("TotalCommits").Value.ToInt() + 1);
        }
        /// <summary>
        ///     Пытается найти все маппинги в хранилище и мягко смержить
        /// </summary>
        private void Initialize() {
            Map = new XElement("Map");
            Map.SetAttributeValue("MapGuid", Guid.NewGuid());
            Map.SetAttributeValue("StartWriting", DateTime.Now);

            MergeCurrentMap();
        }
        /// <summary>
        ///     Мержит текущую карту, если таковая существует
        /// </summary>
        private void MergeCurrentMap() {
            var currentMapStream = Engine.Get(new FileEntity {
                Path = Path.Combine(VcsStorageDefaults.MapFilesDirectory, "master." + VcsStorageDefaults.MapFileExtension)
            }).GetStream(FileAccess.Read);

            if (currentMapStream != null) {
                Map.Add(XElement.Parse(
                    VcsStorageUtils.StreamToString(currentMapStream)    
                ).Elements());
            }
        }
        /// <summary>
        ///     Завершение работы маппера
        /// </summary>
        private void Shutdown() {
            Dump();
        }
        /// <summary>
        ///     Сбрасыват карту на диск
        /// </summary>
        private void Dump() {
            Map.SetAttributeValue("EndWriting", DateTime.Now);
            Engine.Set(
                new FileEntity {
                    Path = Path.Combine(VcsStorageDefaults.MapFilesDirectory, "master." + VcsStorageDefaults.MapFileExtension)
                },
                VcsStorageUtils.StringToStream(Map.ToString())
            );
        }
    }
}
