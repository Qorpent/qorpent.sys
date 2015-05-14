using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Host.Security;
using Qorpent.IO.Net;
using Qorpent.Security;

namespace Qorpent.Host.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        private HostServer host;
        private HttpClient cl;
        private DefaultLogonProvider lp;
        private DefaultAuthenticationProvider auth;

        class TestLogon:ILogon   {
            public int Idx { get; set; }

            public bool IsAuth(string username, string password) {
                return username == password;
            }

            public IIdentity Logon(string username, string password) {
                if(username==password)return new GenericIdentity(username);
                return null;
            }
        }
        [SetUp]
        public void Setup() {
            this.host = new HostServer(14990);
            host.OnBeforeInitializeServices += c => {
                c.Register(c.NewComponent<ILogon,TestLogon>());
            };
            host.Initialize();
            auth = host.Auth as DefaultAuthenticationProvider;
            Assert.NotNull(auth);
            Assert.NotNull(auth.LogonProvider);
            lp = auth.LogonProvider as DefaultLogonProvider;
            Assert.True(lp.Logons.OfType<TestLogon>().Any()); //this code is just tests that Auth uses logons from IoC
            lp.Logons = new[] {new TestLogon()}; //we require just one logon extension
            this.cl = new HttpClient();
            host.Start();
            Thread.Sleep(200);
        }

        [TearDown]
        public void TearDown() {
            host.Stop();
        }

        [Test]
        public void NotAuthenticatedByDefault() {
            Assert.AreEqual("false",cl.GetString("http://127.0.0.1:14990/isauth"));
        }

        [Test]
        public void CanLogon() {
           cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=a&pass=a"));
            var cookie = cl.Cookies["QHAUTH"];
            Assert.NotNull(cookie);
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth"));
        }

        [Test]
        public void CanLogout()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=a&pass=a"));
            var cookie = cl.Cookies["QHAUTH"];
            Assert.NotNull(cookie);
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth"));
            cl.Call("http://127.0.0.1:14990/logout");
            Assert.AreEqual("false", cl.GetString("http://127.0.0.1:14990/isauth"));
        }

        [Test]
        public void CanPostLogon() {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon","login=a&pass=a"));
            var cookie = cl.Cookies["QHAUTH"];
            Assert.NotNull(cookie);
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth"));
        }

        [Test]
        public void CanPreventInvalidLogon() {
            Assert.AreEqual("'error'", cl.GetString("http://127.0.0.1:14990/logon?login=a&pass=xxx"));
        }

        [Test]
        public void CanValidateUserAgent() {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a", r => {
                r.UserAgent = "u1";
            }));
            var cookie = cl.Cookies["QHAUTH"];
            Assert.NotNull(cookie);
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth", setup: r => {
                r.UserAgent = "u1";
            }));
            Assert.AreEqual("false", cl.GetString("http://127.0.0.1:14990/isauth", setup: r =>
            {
                r.UserAgent = "u2";
            }));

        }

        [Test]
        public void CanValidateEndPoint()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a"));
            var cookie = cl.Cookies["QHAUTH"];
            Assert.NotNull(cookie);
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth"));
            string sHostName = Dns.GetHostName();
            IPHostEntry ipE = Dns.GetHostByName(sHostName);
            IPAddress[] IpA = ipE.AddressList; 

            Assert.AreEqual("false", cl.GetString("http://"+IpA[0]+":14990/isauth"));
            
            Assert.AreEqual("false", cl.GetString("http://127.0.0.1:14990/isauth"),"must drop cookies in any invalid login case");

        }

        [Test]
        public void AllowNonExclusiveConnections() {
            var h1 = new HttpClient {Cookies = new CookieCollection()};
            var h2 = new HttpClient {Cookies = new CookieCollection()};
            h1.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a", r =>
            {
                r.UserAgent = "agent1";
            });
            Assert.AreEqual("true", h1.GetString("http://127.0.0.1:14990/isauth", null, r =>
            {
                r.UserAgent = "agent1";
            }));
            h2.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a", r => {
                r.UserAgent = "agent2";
            });
            Assert.AreEqual("true", h2.GetString("http://127.0.0.1:14990/isauth", null, r =>
            {
                r.UserAgent = "agent2";
            }));
            Assert.AreEqual("true", h1.GetString("http://127.0.0.1:14990/isauth", null, r => {
                r.UserAgent = "agent1";
            }));
           

        }

        [Test]
        public void ExclusiveMode() {
            auth.ExclusiveAuth = true;
            var h1 = new HttpClient { Cookies = new CookieCollection() };
            var h2 = new HttpClient { Cookies = new CookieCollection() };
            h1.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a", r =>
            {
                r.UserAgent = "agent1";
            });
            Assert.AreEqual("true", h1.GetString("http://127.0.0.1:14990/isauth", null, r =>
            {
                r.UserAgent = "agent1";
            }));
            h2.GetString("http://127.0.0.1:14990/logon", "login=a&pass=a", r =>
            {
                r.UserAgent = "agent2";
            });

            Assert.AreEqual("true", h2.GetString("http://127.0.0.1:14990/isauth", null, r =>
            {
                r.UserAgent = "agent2";
            }));
            Assert.AreEqual("false", h1.GetString("http://127.0.0.1:14990/isauth", null, r =>
            {
                r.UserAgent = "agent1";
            }));
           

        }
    }
}
