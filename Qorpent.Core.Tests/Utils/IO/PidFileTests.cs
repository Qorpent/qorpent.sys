using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.IO;

namespace Qorpent.Core.Tests.Utils.IO {
    [TestFixture]
    public class PidFileTests {
        [Test]
        public void CanGetCurrentPID() {
            var tmp = Path.GetTempFileName();
            try {
                File.WriteAllText(tmp, "1234");
                using (var l = new PidFile(tmp)) {
                    Assert.False(l.IsAquired);
                    Assert.AreEqual(1234, l.GetCurrentLockPid());
                }
            }
            finally {
                File.Delete(tmp);
            }
        }

        [Test]
        public void CanLockFileAndDeleteIt() {
            var tmp = Path.GetTempFileName();
            File.Delete(tmp);
            Assert.False(File.Exists(tmp));
            using (var l = new PidFile(tmp)) {
                Assert.True(l.IsAquired); // flag set
                Assert.True(File.Exists(tmp)); //creates locker file
                var pid = Process.GetCurrentProcess().Id;
                using (var fs = new FileStream(tmp, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    using (var sr = new StreamReader(fs)) {
                        Assert.AreEqual(pid, Convert.ToInt32(sr.ReadToEnd())); // read shared and PID
                    }
                }
            }
            Assert.False(File.Exists(tmp));
        }

        [Test]
        public void CanResetPID() {
            var tmp = Path.GetTempFileName();
            try {
                File.WriteAllText(tmp, "1234");
                using (var l = new PidFile(tmp)) {
                    Assert.False(l.IsAquired);
                    l.Reset();
                    Assert.False(l.IsAquired);
                    File.Delete(tmp);
                    l.Reset();
                    Assert.True(l.IsAquired);
                }
            }
            finally {
                File.Delete(tmp);
            }
        }

        [Test]
        public void ForceResetInvalidPid() {
            var tmp = Path.GetTempFileName();
            try {
                File.WriteAllText(tmp, "-9999");
                using (var l = new PidFile(tmp)) {
                    Assert.False(l.IsAquired);
                    l.ForseReset();
                    Assert.True(l.IsAquired);
                }
            }
            finally {
                File.Delete(tmp);
            }
        }

        [Test]
        public void ForceResetInvalidPidCtor() {
            var tmp = Path.GetTempFileName();
            try {
                File.WriteAllText(tmp, "-9999");
                using (var l = new PidFile(tmp, PidFileMode.Forced)) {
                    Assert.True(l.IsAquired);
                }
            }
            finally {
                File.Delete(tmp);
            }
        }

        [Test]
        public void NotResetExistedPid() {
            var pid = Process.GetProcesses().First().Id;
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmp, pid.ToString());
                using (var l = new PidFile(tmp, PidFileMode.Forced))
                {
                    Assert.False(l.IsAquired);
                }
            }
            finally
            {
                File.Delete(tmp);
            }
        }

        [Test]
        public void CanWait() {
            var sb = new StringBuilder();
            var pid = Process.GetProcesses().First().Id;
            var tmp = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tmp, pid.ToString());
                var t1 = Task.Run(() => {
                    sb.Append("Zero");
                    using (new PidFile(tmp,PidFileMode.Wait)) {
                        sb.Append("Second");
                    }
                });
                System.Threading.Thread.Sleep(300);
                sb.Append("First");
                File.Delete(tmp);
                t1.Wait(TimeSpan.FromMilliseconds(1000));
            }
            finally
            {
                File.Delete(tmp);
            }
            Assert.AreEqual("ZeroFirstSecond",sb.ToString());
        }

        [Test]
        public void WillNoAquireIfExists() {
            var tmp = Path.GetTempFileName();
            try {
                File.WriteAllText(tmp, "data");
                using (var l = new PidFile(tmp)) {
                    Assert.False(l.IsAquired); // flag not set
                }
                Assert.True(File.Exists(tmp)); //will not delete
            }
            finally {
                File.Delete(tmp);
            }
        }

        [Test]
        public void PreventDataLoss_AllowedNames() {
            foreach (var n in PidFile.ALLOWED_NAMES) {
                using (new PidFile(n)) { }
                using (new PidFile("."+n)) { }
                using (new PidFile("xxxx."+n)) { }
                using (new PidFile(n.ToUpperInvariant())) { }
                using (new PidFile("." + n.ToUpperInvariant())) { }
                using (new PidFile("xxxx." + n.ToUpperInvariant())) { }
                Assert.Throws<Exception>(() => {
                    using (new PidFile("x"+n)) { }
                });
                Assert.Throws<Exception>(() => {
                    using (new PidFile(".x" + n)) { }
                });
                Assert.Throws<Exception>(() => {
                    using (new PidFile("xxx.x" + n)) { }
                });
            }
            var ex = Assert.Throws<Exception>(() => {
                using (new PidFile("mybest.pdf")) { }
            });
            Assert.True(ex.Message.Contains("name"));
        }


        [Test]
        public void PreventDataLoss_ExistedDirectory() {
            if (File.Exists(".pid")) {
                File.Delete(".pid");
            }
            Directory.CreateDirectory(".pid");
            try {
                var ex = Assert.Throws<Exception>(() => {
                    using (new PidFile(".pid")) { }
                });
                Assert.True(ex.Message.Contains("dir"));
            }
            finally {
                Directory.Delete(".pid",true);
            }
        }

        [Test]
        public void PreventDataLoss_TooLargeFile() {
            var s = new string('1',PidFile.MAX_PID_INT_SIZE*2);
            File.WriteAllText(".pid",s);
            try {
                var ex = Assert.Throws<Exception>(() => {
                    using (new PidFile(".pid",PidFileMode.Forced)) { }
                });
                Assert.True(ex.Message.Contains("too large"));
            }
            finally {
                File.Delete(".pid");
            }
        }

        [Test]
        public void PreventDataLoss_NoPidInFile()
        {
            File.WriteAllText(".pid", "hello");
            try
            {
                var ex = Assert.Throws<Exception>(() => {
                    using (new PidFile(".pid", PidFileMode.Forced)) { }
                });
                Assert.True(ex.Message.Contains("not valid PID"));
            }
            finally
            {
                File.Delete(".pid");
            }
        }
    }
}