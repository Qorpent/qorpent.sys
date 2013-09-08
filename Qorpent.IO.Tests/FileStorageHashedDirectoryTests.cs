using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Storages;

namespace Qorpent.IO.Tests {
    [TestFixture]
    class FileStorageHashedDirectoryTests {
        private const string ComplexUrlName = "http://someuser:bla-bla@www.example.com:81/test.page?u=i";
        /// <summary>
        ///     Экземпляр класса-хранилища
        /// </summary>
        private IFileStorage _hashedDirectoryStorage;
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
            _hashedDirectoryStorage = new FileStorageHashedDirectory(_storageDirectory);
        }
        /// <summary>
        ///     Тест выражает то, что мы можем спокойно сохранять и получать данные через
        ///     обёртку и при необходимости получить клас нативного хранилища
        /// </summary>
        [Test]
        public void CanUse() {
            var setDescr = _hashedDirectoryStorage.Set(
                new FileEntity {Path = "test"},
                new MemoryStream(Encoding.UTF8.GetBytes("testString"))
            );

            Assert.NotNull(setDescr);
            Assert.AreEqual("test", setDescr.FileEntity.Path);

            var getDescr = _hashedDirectoryStorage.Get(new FileEntity {Path = "test"});
            
            Assert.NotNull(getDescr);
            Assert.AreEqual("test", getDescr.FileEntity.Path);

        }
        /// <summary>
        ///     Тест выражает то, что мы можем использовать URL в виде имени файла
        /// </summary>
        [Test]
        public void CanUseWithUrlNames() {
            var setDescr = _hashedDirectoryStorage.Set(
                new FileEntity { Path = ComplexUrlName },
                new MemoryStream(Encoding.UTF8.GetBytes("testString"))
            );

            Assert.NotNull(setDescr);
            Assert.AreEqual(ComplexUrlName, setDescr.FileEntity.Path);

            var getDescr = _hashedDirectoryStorage.Get(new FileEntity { Path = ComplexUrlName });

            Assert.NotNull(getDescr);
            Assert.AreEqual(ComplexUrlName, getDescr.FileEntity.Path);

        }
    }
}
