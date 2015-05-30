using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.authentication;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.IO.Http;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.authentication
{

    [TestFixture]
    public class HttpTokenSeriveTest:BaseFixture
    {
        [Test]
        public void CanCreateAndSet() {
            var logon = _container.Get<ILogonService>();
            var req = new HttpRequestDescriptor {
                User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                UserAgent = "testagent",
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"),3456 )
            };
            var hts = _container.Get<IHttpTokenService>();
            var token = hts.Create(req);
            Assert.True(hts.IsValid(req,token));
            var res = new HttpResponseDescriptor();
            hts.Store(res,new Uri("http://my.best.com"),token);
            var cookie = res.Cookies["testauth"];
            Assert.NotNull(cookie);
            Assert.True(cookie.Secure);
            Assert.True(cookie.HttpOnly);
            Assert.AreEqual(".best.com",cookie.Domain);
            Assert.AreEqual("/",cookie.Path);
            Assert.Less(100,cookie.Value.Length);
        }

        [Test]
        public void CanRetrieve() {
            var logon = _container.Get<ILogonService>();
            var req = new HttpRequestDescriptor
            {
                User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                UserAgent = "testagent",
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
            };
            
            var hts = _container.Get<IHttpTokenService>();
            var token = hts.Create(req);
            var enc = _container.Get<ITokenEncryptor>();
            var cookie = enc.Encrypt(token);
            req.Headers = new Dictionary<string, string>();
            req.Headers["Cookie"] = "testauth=" + cookie;
            var token2 = hts.Extract(req);
            Assert.True(hts.IsValid(req,token2));
            Assert.AreEqual(token.stringify(),token2.stringify());
        }

        [Test]
        public void CanCheckExpiration()
        {
            var logon = _container.Get<ILogonService>();
            var req = new HttpRequestDescriptor
            {
                User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                UserAgent = "testagent",
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
            };

            var hts = _container.Get<IHttpTokenService>();
            var token = hts.Create(req);
            token.Expire = token.Expire.AddDays(-1);
            var enc = _container.Get<ITokenEncryptor>();
            var cookie = enc.Encrypt(token);
            req.Headers = new Dictionary<string, string>();
            req.Headers["Cookie"] = "testauth=" + cookie;
            var token2 = hts.Extract(req);
            Assert.False(hts.IsValid(req, token2));
        }

        [Test]
        public void CanCheckUserAgentChange()
        {
            var logon = _container.Get<ILogonService>();
            var req = new HttpRequestDescriptor
            {
                User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                UserAgent = "testagent",
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
            };

            var hts = _container.Get<IHttpTokenService>();
            var token = hts.Create(req);
            token.Expire = token.Expire.AddDays(-1);
            var enc = _container.Get<ITokenEncryptor>();
            var cookie = enc.Encrypt(token);
            req.Headers = new Dictionary<string, string>();
            req.Headers["Cookie"] = "testauth=" + cookie;
            req.UserAgent = "testagent2";
            var token2 = hts.Extract(req);
            Assert.False(hts.IsValid(req, token2));
        }
        [Test]
        public void CanCheckRemoteAddressChange()
        {
            var logon = _container.Get<ILogonService>();
            var req = new HttpRequestDescriptor
            {
                User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                UserAgent = "testagent",
                RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
            };

            var hts = _container.Get<IHttpTokenService>();
            var token = hts.Create(req);
            token.Expire = token.Expire.AddDays(-1);
            var enc = _container.Get<ITokenEncryptor>();
            var cookie = enc.Encrypt(token);
            req.Headers = new Dictionary<string, string>();
            req.Headers["Cookie"] = "testauth=" + cookie;
            req.RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.2"), 3456);
            var token2 = hts.Extract(req);
            Assert.False(hts.IsValid(req, token2));
        }

        [Test]
        [Explicit]
        public void Performance() {
            Loggy.Default.Level = LogLevel.Error;
            CheckRate(i => {
                var logon = _container.Get<ILogonService>();
                var req = new HttpRequestDescriptor
                {
                    User = new GenericPrincipal(logon.Logon("fuser", Fpass), null),
                    UserAgent = "testagent",
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
                };

                var hts = _container.Get<IHttpTokenService>();
                var token = hts.Create(req);
                var enc = _container.Get<ITokenEncryptor>();
                var cookie = enc.Encrypt(token);
                req.Headers = new Dictionary<string, string>();
                req.Headers["Cookie"] = "testauth=" + cookie;
                var token2 = hts.Extract(req);
                hts.IsValid(req, token2);
            });
            Loggy.Default.Level = LogLevel.All;
        }
    }


    [TestFixture]
    public class TokenEncryptorTest:BaseFixture
    {
        [Test]
        public void CanEncryptAndDecrypt() {
            var token = new Token {
                User = "esuser",
                Created = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Expire = new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                Metrics = "test"
            };
            var tokenenc = (TokenEncryptor)_container.Get<ITokenEncryptor>();
            Assert.AreEqual("1234567", tokenenc.Encryptor.KeySource);
            var tokenstr = tokenenc.Encrypt(token);
            Console.WriteLine(tokenstr);
            var token2 = tokenenc.Decrypt(tokenstr);
            Console.WriteLine(token2.stringify());
            Assert.AreEqual(token.stringify(), token2.stringify());
        }

        [Test]
        [Explicit]
        public void Performance() {
           
            CheckRate(i => {
                var token = new Token
                {
                    User = "esuser",
                    Created = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Expire = new DateTime(2015, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    Metrics = "test"
                };
                var tokenenc = (TokenEncryptor)_container.Get<ITokenEncryptor>();
                var tokenstr = tokenenc.Encrypt(token);
                var token2 = tokenenc.Decrypt(tokenstr);
            });
            
        }
    }
}
