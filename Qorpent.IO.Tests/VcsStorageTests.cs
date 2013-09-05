using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Qorpent.IO.VcsStorage;
using Qorpent.IO.VcsStorage.Engines;

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
            var elementFirst = GetVcsStorageElement();
            var elementSecond = GetVcsStorageElement();

            Persister.Commit(elementFirst, GenerateStreamFromString("test data"));
            Persister.Commit(elementSecond, GenerateStreamFromString("SOMETESTDATA"));

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
            Assert.IsTrue(Persister.Exists(new VcsStorageElement { Filename = "test.file.some-other-data-here-willbe"}));
            Assert.IsFalse(Persister.Exists(new VcsStorageElement { Filename = "NOTEXISTStest.file.some-other-data-here-willbe" }));
        }
        [Test]
        public void Remove() {
            WriteStubElements(new[] { "test.file", "test.file.some-other-data-here-willbe" });
            Assert.IsTrue(Persister.Exists(new VcsStorageElement { Filename = "test.file.some-other-data-here-willbe" }));
            Persister.Remove(new VcsStorageElement { Filename = "test.file.some-other-data-here-willbe" });
            Assert.IsFalse(Persister.Exists(new VcsStorageElement { Filename = "test.file.some-other-data-here-willbe" }));
        }
        [Test]
        public void EnumerateCommits() {
            WriteStubElements(new[] { "test.file", "test.file", "test.file" });
            var enumerated = Persister.EnumerateCommits(new VcsStorageElement {Filename = "test.file"});
            Assert.AreEqual(3, enumerated.Count());
        }
        /// <summary>
        ///     Тест показывает, что при попытке открыть несуществующий файл не произоёдт экзепшена
        /// </summary>
        [Test]
        public void TryOpenNonExistsElement() {
            WriteStubElements(new[] { "test.file", "test.file.some-other-data-here-willbe" });
            var picked = Persister.Pick(new VcsStorageElement { Filename = "test.file.notExists" });
            Assert.IsNull(picked);
        }

        [Test]
        public void CanRevert() {
            var elementFirst = GetVcsStorageElement();
            var elementSecond = GetVcsStorageElement();
            var elementThird = GetVcsStorageElement();

            elementFirst.Filename = "test";
            elementSecond.Filename = "test";
            elementThird.Filename = "test";

            Persister.Commit(elementFirst, GenerateStreamFromString("test data"));
            Persister.Commit(elementSecond, GenerateStreamFromString("SOMETESTDATA"));
            Persister.Commit(elementThird, GenerateStreamFromString("ANOTHERDTA"));

            var picked = Persister.Pick(new VcsStorageElement {Filename = "test"});

            Assert.IsNotNull(picked);
            var sr = new StreamReader(picked);
            var content = sr.ReadToEnd();
            sr.Close();

            Assert.AreEqual("ANOTHERDTA", content);
            Persister.Revert(elementSecond);
            picked = Persister.Pick(new VcsStorageElement { Filename = "test" });
            content = new StreamReader(picked).ReadToEnd();
            Assert.AreEqual("SOMETESTDATA", content);
            // выражает то, что изначально у на было три элемента, а после реверта стало 4, т.е.
            // ревер не сносит все старшие комиты, а накатывает поверх
            Assert.AreEqual(4, Persister.Count(new VcsStorageElement { Filename = "test" }));
        }
        /// <summary>
        ///     Тест показывает, что хранилище сможет забутстрапиться из уже существующего хранилища
        /// </summary>
        [Test]
        public void CanBootstrap() {
            var storage = new VcsStorageFsEngine(new DirectoryInfo(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString())));
            RunCanBootstrap(storage);
            RunCanBootstrap(storage);
            Debug.Print(storage.WorkingDirectory.FullName);
        }
        private void RunCanBootstrap(IVcsStorageEngine engine) {
            Persister = new VcsStoragePersister(engine);
            CanCommitFile();
            Exists();
            Remove();
            Persister.Dispose();
        }
    }
}
