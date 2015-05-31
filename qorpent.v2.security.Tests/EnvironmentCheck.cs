using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.messaging;
using qorpent.v2.security.messaging.queues;
using qorpent.v2.security.user.storage;
using qorpent.v2.security.user.storage.providers;
using Qorpent;
using Qorpent.Utils;

namespace qorpent.v2.security.Tests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class EnvironmentCheck : BaseFixture
    {
        [Test]
        public void HasUserService() {
            var es = _container.Get<IUserService>();
            Assert.NotNull(es);
        }
        [Test]
        public void UserSourcesAreConfigured()
        {
            var es = _container.Get<IUserService>();
            Assert.AreEqual(2,es.GetExtensions().Count());
            var leasesources = ((UserService) es).UserCache.GetExtensions().ToList();
            foreach (var extension in es.GetExtensions()) {
                Assert.True(leasesources.Contains((IUserCacheLease)extension));
            }
        }

        [Test]
        public void FileUserSourceWellConfigured() {
            var pwd = new PasswordManager();
            var fsrc = _container.Get<IUserService>().GetExtensions().OfType<FileUserSource>().First();
            Assert.NotNull(fsrc.ResolvedFilePath);
            Assert.NotNull(mainpwd);
            Assert.AreEqual(EnvironmentInfo.ResolvePath( mainpwd),fsrc.ResolvedFilePath);
            var user = fsrc.GetUser("fuser");
            Assert.NotNull(user);
            Assert.True(pwd.MatchPassword(user, "A123456$")); 
        }

        [Test]
        public void ElasticUserSourceWellConfigured()
        {
            if (ignoreelastic) {
                Assert.Ignore("elastic not configured");
            }
            var pwd = new PasswordManager();
            var fsrc = _container.Get<IUserService>().GetExtensions().OfType<ElasticUserSource>().First();
            Assert.AreEqual(esindex, fsrc.Index);
            var user = fsrc.GetUser("esuser");
            Assert.NotNull(user);
            Assert.True(pwd.MatchPassword(user, "B123456$"));
        }

        [Test]
        public void SupportWriteUsers() {
            if (ignoreelastic)
            {
                Assert.Ignore("elastic not configured");
            }
            var es = _container.Get<IUserService>();
            Assert.True(es.WriteUsersEnabled);
        }

        [Test]
        public void CanGetFileBasedUserWithFacade()
        {
            var pwd = new PasswordManager();
            var es = _container.Get<IUserService>();
            var user = es.GetUser("fuser");
            Assert.NotNull(user);
            Assert.True(pwd.MatchPassword(user, "A123456$")); 
        }
        [Test]
        public void CanGetEsBasedUserWithFacade()
        {
            if (ignoreelastic)
            {
                Assert.Ignore("elastic not configured");
            }
            var pwd = new PasswordManager();
            var es = _container.Get<IUserService>();
            var user = es.GetUser("esuser");
            Assert.NotNull(user);
            Assert.True(pwd.MatchPassword(user, "B123456$"));
        }

        [Test]
        public void CanLogonToFuser() {
            var logon = _container.Get<ILogonService>();
            var identity = logon.Logon("fuser", "A123456$");
            Assert.True(identity.IsAuthenticated);
        }
        [Test]
        public void CanLogonToEsuser()
        {
            if (ignoreelastic)
            {
                Assert.Ignore("elastic not configured");
            }
            var logon = _container.Get<ILogonService>();
            var identity = logon.Logon("esuser", "B123456$");
            Assert.True(identity.IsAuthenticated);
        }


        [Test]
        public void MessageQueueEnabled() {
            var mq = _container.Get<IMessageQueue>();
            Assert.NotNull(mq);
            var emq = mq as ElasticSearchMessageQueue;
            Assert.NotNull(emq);
            Assert.True(emq.Enabled);
            Assert.AreEqual("v2securetest", emq.Index);
        }

        [Test]
        public void DefaultAuth() {
            var defauth = (HttpDefaultIdentitySource)_container.Get<IHttpDefaultIdentitySource>();
            Assert.True(defauth.AllowGuest);
            Assert.True(defauth.AllowLocalGuest);
            Assert.True(defauth.LocalGuestIsAdmin);
            Assert.False(defauth.AnyGuestIsAdmin);
            Assert.AreEqual(2,defauth.TrustedOrigins.Count);
            var tr1 = defauth.TrustedOrigins[0];
            Assert.True(tr1.IsMatch(
                new IPEndPoint(IPAddress.Parse("127.0.0.1"),2322), 
                new IPEndPoint(IPAddress.Parse("192.168.0.3"),3450),
                "http://127.0.0.1:9200"
                ));
            var tr2 = defauth.TrustedOrigins[1];
            Assert.True(tr2.IsMatch(
                new IPEndPoint(IPAddress.Parse("192.168.0.2"), 2322),
                new IPEndPoint(IPAddress.Parse("192.168.0.3"), 3450),
               null
                ));

        }

    }
}
