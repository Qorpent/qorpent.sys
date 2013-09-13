using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;
using Qorpent.IO.Storages;

namespace Qorpent.IO.Tests {
    class FileStorageDirtyVersionTests {
        /// <summary>
        ///     Экземпляр класса-хранилища
        /// </summary>
        private IFileStorage _dirtyVersionStorage;
        /// <summary>
        ///     Путь до директории хранилища
        /// </summary>
        private string _storageDirectory;
        /// <summary>
        ///     TestFixtureSetUp
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            _storageDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }
        /// <summary>
        ///     SetUp
        /// </summary>
        [SetUp]
        public void SetUp() {
            _dirtyVersionStorage = new FileStorageDirtyVersion(_storageDirectory);
        }
        /// <summary>
        ///     Тест выражает то, что мы можем спокойно сохранять и получать данные через
        ///     обёртку и при необходимости получить клас нативного хранилища
        /// </summary>
        [Test]
        public void CanUse() {
            var setDescr = _dirtyVersionStorage.Set(
                new FileDescriptor {Path = "test"},
                new MemoryStream(Encoding.UTF8.GetBytes("testString"))
            );

            Assert.NotNull(setDescr);
            Assert.AreEqual("test", setDescr.Descriptor.Path);

            var getDescr = _dirtyVersionStorage.Get(new FileDescriptor {Path = "test"});
            
            Assert.NotNull(getDescr);
            Assert.AreEqual("test", getDescr.Descriptor.Path);

            // Попробует прочитать файл из get дескриптора
            using (var sr = new StreamReader(setDescr.GetStream(FileAccess.Read))) {
                Assert.AreEqual("testString", sr.ReadToEnd());
            }
            // Попробует прочитать файл из set дескриптора
            using (var sr = new StreamReader(getDescr.GetStream(FileAccess.Read))) {
                Assert.AreEqual("testString", sr.ReadToEnd());
            }

            var nativeStorage = (IDirtyVersionStorage)_dirtyVersionStorage.GetUnderlinedStorage();
            Assert.NotNull(nativeStorage);

            using (var sr = new StreamReader(nativeStorage.Open("test", getDescr.Descriptor.Version))) {
                Assert.AreEqual("testString", sr.ReadToEnd());
            }
        }
    }
}
