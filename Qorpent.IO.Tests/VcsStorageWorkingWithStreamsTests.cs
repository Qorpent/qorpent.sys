using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Storages;
using Qorpent.IO.VcsStorage;

namespace Qorpent.IO.Tests {
    [TestFixture]
    public class VcsStorageWorkingWithStreamsTests {
        /// <summary>
        /// 
        /// </summary>
        private string _workingDirectory;
        /// <summary>
        /// 
        /// </summary>
        private IVcsStoragePersister _vcsStorage;
        /// <summary>
        /// 
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp() {
            _workingDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        public void SetUp() {
            if (Directory.Exists(_workingDirectory)) {
                Directory.Delete(_workingDirectory);
            }

            _vcsStorage = new VcsStoragePersister(new FileStorageFs(new DirectoryInfo(_workingDirectory)));
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CanReadFromStreamWriteAndGive() {
            var msFirst = new MemoryStream();
            var msSecond = new MemoryStream();
            msFirst.Write(Encoding.UTF8.GetBytes("test"), 0, 4);
            msSecond.Write(Encoding.UTF8.GetBytes("tezt"), 0, 4);

            _vcsStorage.Commit(new VcsCommit {File = new FileEntity {Path = "test"}}, msFirst);
            var got = _vcsStorage.Commit(new VcsCommit {File = new FileEntity {Path = "test"}}, msSecond);

            Assert.AreEqual("tezt", new StreamReader(_vcsStorage.Pick(got)).ReadToEnd());
        }
    }
}
