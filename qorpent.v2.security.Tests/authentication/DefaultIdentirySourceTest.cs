using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using qorpent.v2.security.authentication;
using qorpent.v2.security.user;
using Qorpent.IO.Http;

namespace qorpent.v2.security.Tests.authentication
{
    [TestFixture]
    public class HttpDefaultIdentitySourceTest : BaseFixture
    {
        private HttpDefaultIdentitySource di;

        [SetUp]
        public override void Setup() {
            base.Setup();
            this.di = (HttpDefaultIdentitySource)_container.Get<IHttpDefaultIdentitySource>();
        }

        [Test]
        public void GuestAuth() {
            var req = new HttpRequestDescriptor {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("234.43.123.25"), 21324),
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.100"), 14141)
            };
            var i = (Identity)di.GetUserIdentity(req);
            Assert.True(i.IsGuest);
            Assert.True(i.IsAuthenticated);
            Assert.AreEqual("guest",i.AuthenticationType);
           Assert.False(i.IsAdmin);
            Assert.AreEqual(di.AnyGuestName,i.Name);

        }

        [Test]
        public void LocalGuestAuth()
        {
            var req = new HttpRequestDescriptor
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21324),
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 14141)
            };
            var i = (Identity)di.GetUserIdentity(req);
            Assert.True(i.IsGuest);
            Assert.True(i.IsAuthenticated);
            Assert.AreEqual("localguest", i.AuthenticationType);
            Assert.True(i.IsAdmin);
            Assert.AreEqual(di.LocalGuestName, i.Name);

        }

        [Test]
        public void TrustAuthOne()
        {
            var req = new HttpRequestDescriptor
            {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 21324),
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 3450),
            };
            req.Headers = new Dictionary<string, string>();
            req.Headers["Origin"] = "http://127.0.0.1:9200";
            var i = (Identity)di.GetUserIdentity(req);
            Assert.False(i.IsGuest);
            Assert.True(i.IsAuthenticated);
            Assert.AreEqual("trusted", i.AuthenticationType);
            Assert.True(i.IsAdmin);
            Assert.AreEqual("sense", i.Name);

        }

        [Test]
        public void TrustAuthTwo()
        {
            var req = new HttpRequestDescriptor {
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.2"), 21324),
                LocalEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 3450),
                Headers = new Dictionary<string, string>{{"Origin","http://127.0.0.1:9200"}}
            };
            var i = (Identity)di.GetUserIdentity(req);
            Assert.False(i.IsGuest);
            Assert.True(i.IsAuthenticated);
            Assert.AreEqual("trusted", i.AuthenticationType);
            Assert.True(i.IsAdmin);
            Assert.AreEqual("controller", i.Name);

        }
    }
}
