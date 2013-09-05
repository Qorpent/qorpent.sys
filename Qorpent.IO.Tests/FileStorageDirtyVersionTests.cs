using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                new FileEntity {Path = "test"},
                new MemoryStream(Encoding.UTF8.GetBytes("testString"))
            );

            Assert.NotNull(setDescr);
            Assert.AreEqual("test", setDescr.FileEntity.Path);

            var getDescr = _dirtyVersionStorage.Get(new FileEntity {Path = "test"});
            
            Assert.NotNull(getDescr);
            Assert.AreEqual("test", getDescr.FileEntity.Path);

            // Попробует прочитать файл из get дескриптора
            using (var sr = new StreamReader(setDescr.GetStream(FileAccess.Read))) {
                Assert.AreEqual("testString", sr.ReadToEnd());
            }
            // Попробует прочитать файл из set дескриптора
            using (var sr = new StreamReader(getDescr.GetStream(FileAccess.Read))) {
                Assert.AreEqual("testString", sr.ReadToEnd());
            }
        }
    }
}
