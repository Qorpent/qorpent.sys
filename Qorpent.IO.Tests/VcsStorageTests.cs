using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Qorpent.IO.Storages;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Tests {
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class VcsStorageTests : VcsStorageTestsBase {
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CanCommitFile() {
            var elementFirst = Persister.Commit(new VcsCommit {File = new FileEntity {Path = Guid.NewGuid().ToString()}}, GenerateStreamFromString("test data"));
            var elementSecond = Persister.Commit(new VcsCommit {File = new FileEntity { Path = Guid.NewGuid().ToString() }}, GenerateStreamFromString("SOMETESTDATA"));

            var pickedFirst = Persister.Pick(elementFirst);
            var pickedSecond = Persister.Pick(elementSecond);

            Assert.IsNotNull(pickedFirst);
            Assert.IsNotNull(pickedSecond);

            var contentFirst = new StreamReader(pickedFirst).ReadToEnd();
            var contentSecond = new StreamReader(pickedSecond).ReadToEnd();

            Assert.AreEqual("test data", contentFirst);
            Assert.AreEqual("SOMETESTDATA", contentSecond);
        }
        /// <summary>
        ///     Тест выражает то, что мы можем заисать два файла с похожими именами
        ///     типа test.file и test.file.some-other-data-here-willbe,
        ///     а метод Exists корректно проверит их существование
        /// </summary>
        [Test]
        public void Exists() {
            WriteStubElements(new[] { "test.file", "test.file.some-other-data-here-willbe" });
            Assert.IsTrue(Persister.Exists(new FileEntity { Path = "test.file.some-other-data-here-willbe"}));
            Assert.IsFalse(Persister.Exists(new FileEntity { Path = "NOTEXISTStest.file.some-other-data-here-willbe" }));
        }
        [Test]
        public void Remove() {
            WriteStubElements(new[] { "test.file", "test.file.some-other-data-here-willbe" });
            Assert.IsTrue(Persister.Exists(new FileEntity { Path = "test.file.some-other-data-here-willbe" }));
            Persister.Remove(new FileEntity { Path = "test.file.some-other-data-here-willbe" });
            Assert.IsFalse(Persister.Exists(new FileEntity { Path = "test.file.some-other-data-here-willbe" }));
        }
        [Test]
        public void EnumerateCommits() {
            WriteStubElements(new[] { "test.file", "test.file", "test.file" });
            var enumerated = Persister.EnumerateCommits(new FileEntity { Path = "test.file" });
            Assert.AreEqual(3, enumerated.Count());
        }
        /// <summary>
        ///     Тест показывает, что при попытке открыть несуществующий файл не произоёдт экзепшена
        /// </summary>
        [Test]
        public void TryOpenNonExistsElement() {
            WriteStubElements(new[] { "test.file", "test.file.some-other-data-here-willbe" });
            var picked = Persister.Pick(new VcsCommit { File = new FileEntity { Path = "test.file.notExists" } });
            Assert.IsNull(picked);
        }
        
        [Test]
        public void CanRevert() {


            var elementFirst = Persister.Commit(new VcsCommit { File = new FileEntity { Path = "test" } }, GenerateStreamFromString("test data"));
            var elementSecond = Persister.Commit(new VcsCommit { File = new FileEntity { Path = "test" } }, GenerateStreamFromString("SOMETESTDATA"));
            var elementThird = Persister.Commit(new VcsCommit { File = new FileEntity { Path = "test" } }, GenerateStreamFromString("ANOTHERDTA"));

            var picked = Persister.Pick(new VcsCommit { File = new FileEntity { Path = "test" } });

            Assert.IsNotNull(picked);
            var sr = new StreamReader(picked);
            var content = sr.ReadToEnd();
            sr.Close();

            Assert.AreEqual("ANOTHERDTA", content);
            Persister.Revert(elementSecond);
            picked = Persister.Pick(new VcsCommit { File = new FileEntity { Path = "test" } });
            content = new StreamReader(picked).ReadToEnd();
            Assert.AreEqual("SOMETESTDATA", content);
            // выражает то, что изначально у на было три элемента, а после реверта стало 4, т.е.
            // ревер не сносит все старшие комиты, а накатывает поверх
            Assert.AreEqual(4, Persister.Count(new FileEntity { Path = "test" }));
        }
        /// <summary>
        ///     Тест показывает, что хранилище сможет забутстрапиться из уже существующего хранилища
        /// </summary>
        [Test]
        public void CanBootstrap() {
            var storage = new FileStorageFs(new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())));
            RunCanBootstrap(storage);
            RunCanBootstrap(storage);
            Debug.Print(storage.WorkingDirectory.FullName);
        }
        [Test]
        public void Branching() {
            var br1Commit = Persister.Commit(new VcsCommit { File = new FileEntity { Path = "test" }, Branch = "br1"}, GenerateStreamFromString("test data"));
            var br2Commit = Persister.Commit(new VcsCommit { File = new FileEntity { Path = "test" }, Branch = "br2"}, GenerateStreamFromString("SOMETESTDATA"));


            var pickedFirst = Persister.Pick(new VcsCommit {Branch = "br1", File = new FileEntity {Path = "test"}});
            var pickedSecond = Persister.Pick(new VcsCommit { Branch = "br2", File = new FileEntity { Path = "test" } });

            Assert.IsNotNull(pickedFirst);
            Assert.IsNotNull(pickedSecond);

            var contentFirst = new StreamReader(pickedFirst).ReadToEnd();
            var contentSecond = new StreamReader(pickedSecond).ReadToEnd();

            Assert.AreEqual("test data", contentFirst);
            Assert.AreEqual("SOMETESTDATA", contentSecond);
        }
        private void RunCanBootstrap(IFileStorage engine) {
            Persister = new VcsStoragePersister(engine);
            CanCommitFile();
            Exists();
            Remove();
            Persister.Dispose();
        }
    }
}
