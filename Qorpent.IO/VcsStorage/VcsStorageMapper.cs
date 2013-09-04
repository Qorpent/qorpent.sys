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
        ///     Текущая карта хранилища
        /// </summary>
        private XElement Map { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IVcsStorageEngine Engine { get; private set; }
        /// <summary>
        ///     Указатель на то, что класс уже разрушен
        /// </summary>
        private bool _disposed;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        public VcsStorageMapper(IVcsStorageEngine engine) {
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
        /// <param name="element"></param>
        /// <param name="transactionType"></param>
        public void Transaction(IVcsStorageElement element, VcsStorageTransactionType transactionType) {
            switch (transactionType) {
                case VcsStorageTransactionType.Commit: Insert(element); break;
                case VcsStorageTransactionType.Remove: Remove(element); break;
            }
        }
        /// <summary>
        ///     Поиск представления элемента в хранилище
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IVcsStorageElement> Find(IVcsStorageElement element) {
            return Find(element, true);
        }
        /// <summary>
        ///     Установить движок
        /// </summary>
        /// <param name="engine"></param>
        public void SetEngine(IVcsStorageEngine engine) {
            Engine = engine;
        }
        /// <summary>
        ///     Поиск представления элемента в хранилище
        /// </summary>
        /// <param name="element"></param>
        /// <param name="excludeRemoved"></param>
        /// <returns></returns>
        private IEnumerable<IVcsStorageElement> Find(IVcsStorageElement element, bool excludeRemoved) {
            var container = GetElement(element);

            if (container == null) {
                return new List<IVcsStorageElement>();
            }

            return new List<IVcsStorageElement>(container.Descendants("Commit").Where(
                el => el.IsRemovedElement() == !excludeRemoved
            ).Select(
                el => new VcsStorageElement {
                    Commit = el.Attribute("Code").Value,
                    Filename = container.Attribute("Filename").Value
                }
            ));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        public void Insert(IVcsStorageElement element) {
            var container = GetElement(element);

            if (container == null) {
                container = new XElement("Element");
                container.SetAttributeValue("Filename", element.Filename);
                container.SetAttributeValue("TotalCommits", 0);
                Map.Add(container);
            }

            var commit = new XElement("Commit");
            commit.SetAttributeValue("Code", element.Commit);
            commit.SetAttributeValue("DateTime", DateTime.Now);

            IncrementElementCommits(container);
            container.SetAttributeValue("LastCommit", element.Commit);
            container.AddFirst(commit);
        }
        /// <summary>
        ///     Удаляет элемент из маппинга. Если указан коммит, то только 
        ///     данный коммит, в противном случае - весь элемент
        /// </summary>
        /// <param name="element">Представление элемента</param>
        public void Remove(IVcsStorageElement element) {
            var container = GetElement(element);

            if (!VcsStorageUtils.CorrectCommitCode(element)) {
                container.SetAttributeValue("Removed", true);
            } else {
                var commit = container.XPathSelectElement("/Element/Commit[@Code='" + element.Commit + "']");
                if (commit != null) {
                    commit.SetAttributeValue("Removed", true);
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
        /// <param name="element">Представление элемента</param>
        /// <returns></returns>
        public bool Exists(IVcsStorageElement element) {
            var found = Find(element);

            if (!found.Any()) {
                return false;
            }
            // если код коммита имеет некорректную форму или не указан, а элемент
            // найден - просто вернём true
            if (!VcsStorageUtils.CorrectCommitCode(element)) {
                return true;
            }

            return found.Any(el => el.Commit == element.Commit);
        }
        /// <summary>
        ///     Подсчитывает количество версий файла
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public int Count(IVcsStorageElement element) {
            return Find(element).Count();
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
        /// <param name="element">Представление элемента</param>
        /// <returns>XML-контейнер элемента</returns>
        private XElement GetElement(IVcsStorageElement element) {
            return Map.XPathSelectElement("/Element[@Filename='" + element.Filename + "']");
        }

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
            var currentMapStream = Engine.Get(new VcsStorageElementDescriptor {
                Filename = "master." + VcsStorageDefaults.MapFileExtension,
                RelativeDirectory = VcsStorageDefaults.MapFilesDirectory
            });

            if (currentMapStream.Stream != null) {
                Map.Add(XElement.Parse(
                    VcsStorageUtils.StreamToString(currentMapStream.Stream)    
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
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = "master." + VcsStorageDefaults.MapFileExtension,
                    RelativeDirectory = VcsStorageDefaults.MapFilesDirectory
                },
                StreamAccess = FileAccess.Read,
                Stream = VcsStorageUtils.StringToStream(Map.ToString())
            });
        }
    }
}
