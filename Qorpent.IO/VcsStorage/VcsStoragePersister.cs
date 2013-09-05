using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Qorpent.IO.VcsStorage {
    /// <summary>
    /// 
    /// </summary>
    public class VcsStoragePersister : IVcsStoragePersister, IDisposable, IFileStorage {
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
        public IFileStorage Engine { get; private set; }
        /// <summary>
        ///     Поддерживаемый функционал
        /// </summary>
        public FileStorageAbilities Abilities { get; private set; }
        /// <summary>
        ///     Конструктор по умолчанию
        /// </summary>
        /// <param name="engine"></param>
        public VcsStoragePersister(IFileStorage engine) {
            Engine = engine;
            Abilities = FileStorageAbilities.Vcs;
            
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
        /// <param name="file">Путь файла от корня хранилища</param>
        /// <param name="stream">Данные для записи</param>
        /// <returns>Внутренний идентификатор коммита</returns>
        public VcsCommit Commit(IFileEntity file, Stream stream) {
            var commit = new VcsCommit {File = file, Code = ComputeCommitCode(stream)};

            Transaction(commit, VcsStorageTransactionType.Commit);
            RollRealWriting(commit, stream);
            return commit;
        }
        /// <summary>
        ///     Возвращает поток на чтение файла из хранилища
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        public Stream Pick(VcsCommit commit) {
            return VcsStorageUtils.CorrectCommitCode(commit) ? PickCommit(commit) : PickLatestCommit(commit);
        }
        /// <summary>
        ///     Удалить элемент из хранилища
        /// </summary>
        /// <param name="file">Целевой элемент</param>
        public void Remove(IFileEntity file) {
            Remove(new VcsCommit {File = file}, EnumerateCommits(file));
        }
        /// <summary>
        ///     Удалить определённые версии файла из хранилища
        /// </summary>
        /// <param name="commit">Целевой элемент</param>
        /// <param name="commits">Перечисление коммитов</param>
        public void Remove(VcsCommit commit, IEnumerable<string> commits) {
            foreach (var c in commits) {
                Transaction(
                    new VcsCommit {
                        Code = c,
                        File = commit.File
                    }, 
                    VcsStorageTransactionType.Remove
                );
            }
        }
        /// <summary>
        ///     Производит реверт элемента на коммит, указанный в поле IVcsStorageElement::commit
        /// </summary>
        /// <param name="commit">Представление элемента</param>
        public VcsCommit Revert(VcsCommit commit) {
            if (!VcsStorageUtils.CorrectCommitCode(commit)) {
                throw new VcsStorageException("Incorrect commit code!");
            }

            if (!CommitExists(commit)) {
                throw new VcsStorageException("Transaction not exists!");
            }

            var sourceStream = Engine.Get(new VcsStorageElementDescriptor {
                Filename = commit.Code,
                RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
            }).Stream;

            var reverted = new VcsCommit {
                File = new FileEntity {
                    Path = commit.File.Path
                },
                Code = ComputeCommitCode(sourceStream)
            };

            Transaction(commit, VcsStorageTransactionType.Revert);
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = reverted.Code,
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
        /// <param name="file"></param>
        /// <returns></returns>
        public int Count(IFileEntity file) {
            return _mapper.Count(file);
        }
        /// <summary>
        ///     проверяет существование файла в хранилище
        /// </summary>
        /// <returns></returns>
        public bool Exists(IFileEntity file) {
            return _mapper.Exists(new VcsCommit {File = file});
        }
        /// <summary>
        ///     Перечисляет идентификаторы коммитов файла
        /// </summary>
        /// <param name="file">Представление элемента</param>
        /// <returns>Перечисление идентификаторов коммитов</returns>
        public IEnumerable<string> EnumerateCommits(IFileEntity file) {
            return _mapper.Find(new VcsCommit {File = file}).Select(el => el.Code);
        }
        /// <summary>
        ///     Устанавливает движок низкоуровневого хранилища для
        ///     маппера
        /// </summary>
        /// <param name="engine">Движок</param>
        public void SetMapperEngine(IFileStorage engine) {
            if (_mapper != null) {
                _mapper.SetEngine(engine);
            }
        }
        /// <summary>
        ///     Устанавливает движок низкоуровневого хранилища для
        ///     бинарного журнала
        /// </summary>
        /// <param name="engine">Движок</param>
        public void SetBinLogEngine(IFileStorage engine) {
            if (_binLog != null) {
                _binLog.SetEngine(engine);
            }
        }
        /// <summary>
        ///     Проверяет наличие коммита в системе
        /// </summary>
        /// <param name="commit">Представление элемента</param>
        /// <returns>True, если коммит присутствует</returns>
        private bool CommitExists(VcsCommit commit) {
            return _mapper.Find(commit).Any(c => c.Code == commit.Code);
        }
        /// <summary>
        ///     Возвращает актуальную версию элемента
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private VcsCommit GetLatestVersion(IFileEntity file) {
            return _mapper.Find(new VcsCommit {File = file}).ToList().FirstOrDefault();
        }
        /// <summary>
        ///     Возвращает поток до версии файла, если он существует
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        private Stream PickCommit(VcsCommit commit) {
            if (_mapper.Exists(commit)) {
                return Engine.Get(new VcsStorageElementDescriptor {
                    Filename = commit.Code,
                    RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
                }).Stream;
            }
            
            return null;
        }
        /// <summary>
        ///     Возвращает наиболее позднюю версию элемента из хранилища
        /// </summary>
        /// <param name="commit"></param>
        /// <returns></returns>
        private Stream PickLatestCommit(VcsCommit commit) {
            var latestVersion = GetLatestVersion(commit.File);

            if (latestVersion == null) {
                return null;
            }

            return Engine.Get(new VcsStorageElementDescriptor {
                Filename = latestVersion.Code,
                RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
            }).Stream;
        }
        /// <summary>
        ///     Производит реальной прокат записи на диск
        /// </summary>
        /// <param name="commit">Представление элемента</param>
        /// <param name="stream">Исходный поток</param>
        private void RollRealWriting(VcsCommit commit, Stream stream) {
            Engine.Set(new VcsStorageEngineElement {
                Descriptor = new VcsStorageElementDescriptor {
                    Filename = commit.Code,
                    RelativeDirectory = VcsStorageDefaults.ObjFilesDirectory
                },
                Stream = stream,
                StreamAccess = FileAccess.Read
            });
        }
        /// <summary>
        ///     Регистрирует транзакцию
        /// </summary>
        /// <param name="commit">Представление коммита</param>
        /// <param name="type">Тип транзакции</param>
        private void Transaction(VcsCommit commit, VcsStorageTransactionType type) {
            _binLog.Transaction(new VcsStorageTransaction {
                Commit = commit.Code,
                DateTime = DateTime.Now,
                Filename = commit.File.Path,
                Type = type
            });

            _mapper.Transaction(commit, type);
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
        /// <summary>
        ///     Сохранение в хранилище
        /// </summary>
        /// <param name="engineElement"></param>
        public void Set(IVcsStorageEngineElement engineElement) {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Получение из хранилища
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public IVcsStorageEngineElement Get(IVcsStorageElementDescriptor descriptor) {
            throw new NotImplementedException();
        }
    }
}
