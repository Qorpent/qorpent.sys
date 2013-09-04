using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    /// 
    /// </summary>
    public class VcsStoragePersister : IVcsStoragePersister, IDisposable {
        /// <summary>
        ///     Бинарный журнал
        /// </summary>
        private readonly VcsStorageBinLog _binLog;
        /// <summary>
        ///     Маппер
        /// </summary>
        private readonly VcsStorageMapper _mapper;
        /// <summary>
        /// 
        /// </summary>
        public IVcsStorageEngine Engine { get; private set; }
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        /// <param name="engine"></param>
        public VcsStoragePersister(IVcsStorageEngine engine) {
            Engine = engine;

            _binLog = new VcsStorageBinLog(Engine);
            _mapper = new VcsStorageMapper(Engine);
        }
        /// <summary>
        ///     Деструктор
        /// </summary>
        public void Dispose() {
            _mapper.Dispose();
            _binLog.Dispose();
        }
        /// <summary>
        ///     Коммит файла в хранилище
        /// </summary>
        /// <param name="element">Путь файла от корня хранилища</param>
        /// <param name="stream">Данные для записи</param>
        /// <returns>Внутренний идентификатор коммита</returns>
        public void Commit(IVcsStorageElement element, Stream stream) {
            element.Commit = ComputeCommitCode(stream);
            Transaction(element, VcsStorageTransactionType.Commit);
            RollRealWriting(element, stream);
        }
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public Stream Pick(IVcsStorageElement element) {
            return VcsStorageUtils.CorrectCommitCode(element) ? PickCommit(element) : PickLatestCommit(element);
        }
        /// <summary>
        ///     Удалить элемент из хранилища
        /// </summary>
        /// <param name="element">Целевой элемент</param>
        public void Remove(IVcsStorageElement element) {
            Remove(element, EnumerateCommits(element));
        }
        /// <summary>
        ///     Удалить определённые версии файла из хранилища
        /// </summary>
        /// <param name="element">Целевой элемент</param>
        /// <param name="commits">Перечисление коммитов</param>
        public void Remove(IVcsStorageElement element, IEnumerable<string> commits) {
            foreach (var commit in commits) {
                Transaction(
                    new VcsStorageElement {
                        Filename = element.Filename,
                        Commit = commit
                    },
                    VcsStorageTransactionType.Remove
                );
            }
        }
        /// <summary>
        ///     Производит реверт элемента на коммит, указанный в поле IVcsStorageElement::commit
        /// </summary>
        /// <param name="element">Представление элемента</param>
        public IVcsStorageElement Revert(IVcsStorageElement element) {
            if (!VcsStorageUtils.CorrectCommitCode(element)) {
                throw new VcsStorageException("Incorrect commit code!");
            }

            if (!CommitExists(element)) {
                throw new VcsStorageException("Transaction not exists!");
            }

            var sourceStream = Engine.Get(new VcsStorageElementDescriptor {
                Filename = element.Commit,
                RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
            }).Stream;

            var reverted = new VcsStorageElement {
                Filename = element.Filename,
                Commit = ComputeCommitCode(sourceStream)
            };

            Transaction(element, VcsStorageTransactionType.Revert);
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = reverted.Commit,
                    RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
                },
                StreamAccess = FileAccess.Read,
                Stream = sourceStream
            });
            
            return reverted;
        }
        /// <summary>
        ///     Подсчитывает количество версий элемента в хранилище
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns></returns>
        public int Count(IVcsStorageElement element) {
            return _mapper.Count(element);
        }
        /// <summary>
        ///     проверяет существование файла в хранилище
        /// </summary>
        /// <returns></returns>
        public bool Exists(IVcsStorageElement element) {
            return _mapper.Exists(element);
        }
        /// <summary>
        ///     Перечисляет идентификаторы коммитов файла
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns>Перечисление идентификаторов коммитов</returns>
        public IEnumerable<string> EnumerateCommits(IVcsStorageElement element) {
            return _mapper.Find(element).Select(el => el.Commit);
        }
        /// <summary>
        ///     Устанавливает движок низкоуровневого хранилища для
        ///     маппера
        /// </summary>
        /// <param name="engine">Движок</param>
        public void SetMapperEngine(IVcsStorageEngine engine) {
            if (_mapper != null) {
                _mapper.SetEngine(engine);
            }
        }
        /// <summary>
        ///     Устанавливает движок низкоуровневого хранилища для
        ///     бинарного журнала
        /// </summary>
        /// <param name="engine">Движок</param>
        public void SetBinLogEngine(IVcsStorageEngine engine) {
            if (_binLog != null) {
                _binLog.SetEngine(engine);
            }
        }
        /// <summary>
        ///     Проверяет наличие коммита в системе
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns>True, если коммит присутствует</returns>
        private bool CommitExists(IVcsStorageElement element) {
            return _mapper.Find(element).Any(el => el.Commit == element.Commit);
        }
        /// <summary>
        ///     Возвращает актуальную версию элемента
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private IVcsStorageElement GetLatestVersion(IVcsStorageElement element) {
            return _mapper.Find(element).ToList().FirstOrDefault();
        }
        /// <summary>
        ///     Возвращает поток до версии файла, если он существует
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns></returns>
        private Stream PickCommit(IVcsStorageElement element) {
            if (_mapper.Exists(element)) {
                return Engine.Get(new VcsStorageElementDescriptor {
                    Filename = element.Commit,
                    RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
                }).Stream;
            }
            
            return null;
        }
        /// <summary>
        ///     Возвращает наиболее позднюю версию элемента из хранилища
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <returns></returns>
        private Stream PickLatestCommit(IVcsStorageElement element) {
            var latestVersion = GetLatestVersion(element);

            if (latestVersion == null) {
                return null;
            }

            return Engine.Get(new VcsStorageElementDescriptor {
                Filename = latestVersion.Commit,
                RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
            }).Stream;
        }
        /// <summary>
        ///     Производит реальной прокат записи на диск
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <param name="stream">Исходный поток</param>
        private void RollRealWriting(IVcsStorageElement element, Stream stream) {
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = element.Commit,
                    RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
                },
                Stream = stream,
                StreamAccess = FileAccess.Read
            });
        }
        /// <summary>
        ///     Регистрирует транзакцию
        /// </summary>
        /// <param name="element">Представление элемента</param>
        /// <param name="type">Тип транзакции</param>
        private void Transaction(IVcsStorageElement element, VcsStorageTransactionType type) {
            _binLog.Transaction(new VcsStorageTransaction {
                Commit = element.Commit,
                DateTime = DateTime.Now,
                Filename = element.Filename,
                Type = type
            });

            _mapper.Transaction(element, type);
        }
        /// <summary>
        ///     Вычисляет код коммита по содержимому, добавляю компонент случайности
        ///     во избежание коллизий
        /// </summary>
        /// <param name="stream">Исходный поток</param>
        /// <returns></returns>
        private static string ComputeCommitCode(Stream stream) {
            using (var internalStream = new MemoryStream()) {
                stream.CopyTo(internalStream);
                return VcsStorageUtils.ComputeShaFromString(
                    VcsStorageUtils.StreamToString(internalStream) + Guid.NewGuid().ToString()
                );
            }
        }
    }
}
