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
            //var msFirst = new MemoryStream();
            //var msSecond = new MemoryStream();
            //msFirst.Write(Encoding.UTF8.GetBytes("test"), 0, 4);
            //msSecond.Write(Encoding.UTF8.GetBytes("tezt"), 0, 4);
            
            //второе зачем "писать" в MemoryStream
            var msFirst = new MemoryStream(Encoding.UTF8.GetBytes("test"));
            var msSecond = new MemoryStream(Encoding.UTF8.GetBytes("tezt"));
            
            _dirtyVersionStorage.Save("testfile", msFirst);
            _dirtyVersionStorage.Save("testfile", msSecond);

            Assert.AreEqual("tezt", new StreamReader(_dirtyVersionStorage.Open("testfile")).ReadToEnd());
        }
    }
}
