using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Utils.Tests
{
    [TestFixture]
    public class FileSystemHelperTests
    {
        [Test]
        public void KillDirectory() {
            var dir = FileSystemHelper.ResetTemporaryDirectory();
            Directory.Delete(dir);
            FileSystemHelper.KillDirectory(dir); //no error
            FileSystemHelper.ResetTemporaryDirectory();
            File.WriteAllText(Path.Combine(dir,"test.txt"),"xxx");
            Assert.True(Directory.Exists(dir));
            FileSystemHelper.KillDirectory(dir);
            Assert.False(Directory.Exists(dir));
        }

        [Test]
        public void ResetTemporaryDirectory()
        {
            var dir = FileSystemHelper.ResetTemporaryDirectory();
            Assert.True(dir.Replace("\\", "/").EndsWith("/FileSystemHelperTests_ResetTemporaryDirectory"));
            Assert.True(Directory.Exists(dir));
            FileSystemHelper.KillDirectory(dir);
        }
    }
}
