using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.DirtyVersion;

namespace Qorpent.IO.Tests.DirtyVersion {
    [TestFixture]
    public class WorkingWithStreamsTests {
        /// <summary>
        /// 
        /// </summary>
        private string _workingDirectory;
        /// <summary>
        /// 
        /// </summary>
        private DirtyVersionStorage _dirtyVersionStorage;
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

            _dirtyVersionStorage = new DirtyVersionStorage(_workingDirectory);
        }
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CanReadFromStreamWriteAndGive() {
            //третье - вот реальное минимальное зло
            _dirtyVersionStorage.Save("testfile", "same");
            _dirtyVersionStorage.Save("testfile", "same");
            //и зачем столь нативно, можно было ведь и так:
            //Assert.AreEqual("same", _dirtyVersionStorage.ReadString("testfile"));
            //но главное - не в этом же зло
        }
    }
}
