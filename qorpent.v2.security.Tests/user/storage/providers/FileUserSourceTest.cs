using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using qorpent.v2.security.user.storage.providers;
using Qorpent.Utils;

namespace qorpent.v2.security.Tests.user.storage.providers
{
    [TestFixture]
    public class FileUserSourceTest:BaseFixture
    {
        private FileUserSource _fus;

        [Test]
        public void CanReadUsersFromSource() {
            Setup();
            Assert.Null(_fus.GetUser("u2"));
            var u = _fus.GetUser("u1");
            Assert.NotNull(u);
            Assert.AreEqual("u1",u.Login);
            Assert.AreEqual("n1",u.Name);
            Assert.True(u.Logable);
            Assert.True(u.Active);
            Assert.AreEqual("s",u.Salt);
            Assert.AreEqual("h",u.Hash);
            Assert.AreEqual("p",u.PublicKey);
            Assert.AreEqual("e1",u.Email);
            Assert.True(u.Roles.Contains("r1"));
            Assert.True(u.Groups.Contains("g1"));

        }

        [Test]
        public void WorkAsLeaseForUserStorage() {
            var u = _fus.GetUser("u1");
            var dt = _fus.Version;
            File.AppendAllText(_fus.DefaultFilePath,"\r\n#comment");
            Assert.AreEqual(dt,_fus.Version);
            _fus.Refresh();
            Assert.Less(dt,_fus.Version);
        }

        [Test]
        public void AutoRefresh() {
            var u = _fus.GetUser("u1");
            _fus.CheckRate = 200;
            var lc = _fus.LastCheck;
            Console.WriteLine(_fus.LastCheck);
            Thread.Sleep(201); //time gone but not changed
            var u_ = _fus.GetUser("u1");
            Assert.AreSame(u,u_);
            var dt = _fus.Version;
            Assert.Less(lc,_fus.LastCheck);
            Console.WriteLine(_fus.LastCheck);
            File.AppendAllText(_fus.DefaultFilePath, "\r\n#comment");
            Assert.AreEqual(dt, _fus.Version);
            var u2 = _fus.GetUser("u1");
            Console.WriteLine(_fus.LastCheck);
            Assert.AreSame(u,u2); //cached 
            Thread.Sleep(201); //time gone and changed
            
            var u3 = _fus.GetUser("u1");
            Assert.Less(dt, _fus.Version);
            Assert.AreNotSame(u,u3);
        }

        [SetUp]
        public void  Setup() {
            var dir = FileSystemHelper.ResetTemporaryDirectory("FileUserSourceTest");
            var file = Path.Combine(dir, "test.bxls");
            File.WriteAllText(file, @"
class main prototype=pwd
    usr u1 n1 hash=h salt=s publickey=p email=e1 logable active
        role r1
        group g1
");
            var fus = new FileUserSource {DefaultFilePath = file};
            _fus = fus;
        }


        [Test]
        [Explicit]
        public void PerformanceTest()
        {
            CheckRate(i =>
            {
                if (0 == i % 100)
                {
                    _fus.Refresh();
                    _fus.Clear();
                }
                var u = _fus.GetUser("u1");
                Assert.NotNull(u);
            }, 10000, "1/100 refresh");
            CheckRate(i =>
            {
                _fus.Refresh();
                _fus.Clear();
                var u = _fus.GetUser("u1");
                Assert.NotNull(u);
            }, 10000, "1/1 refresh");

        }
        
    }
}
