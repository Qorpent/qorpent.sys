using System;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Qorpent.Host.Security;
using Qorpent.IO.Net;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        private HostServer host;
        private HttpClient cl;
        private DefaultLogonProvider lp;
        private DefaultAuthenticationProvider auth;
        private DefaultLoginSourceProvider ls;

        

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
            ls = auth.LoginSourceProvider as DefaultLoginSourceProvider;
            ls.Sources = new[] {new TestLoginSource()};
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
        public void CanLogonThrowLoginSource() {

            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=adm&pass=adm"));
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/isauth"));
        }

        [Test]
        public void CanGetMyLoginInfo() {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=adm&pass=adm"));
            var str = cl.GetString("http://127.0.0.1:14990/mylogin").Simplify(SimplifyOptions.NoWs|SimplifyOptions.SingleQuotes);
            Console.WriteLine(str);
            Assert.True(str.Contains("'Login':'adm'"));
        }

        [Test]
        public void AdminCanGetOtherLoginInfo()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=adm&pass=adm"));
            var str = cl.GetString("http://127.0.0.1:14990/myinfo?login=usr").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Console.WriteLine(str);
            Assert.True(str.Contains("{'Login':'usr','Name':'UU','Groups':['y'],'Roles':['viewver','x.all'],'IsActive':true}"));
        }

        [Test]
        public void UserCannotGetOtherLoginInfo()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=usr&pass=usr"));
            var str = cl.GetString("http://127.0.0.1:14990/myinfo?login=adm").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Console.WriteLine(str);
            Assert.True(str.Contains("System.Exception:notadmin"));
        }

        [Test]
        public void CanCheckRole()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=usr&pass=usr"));
            var str = cl.GetString("http://127.0.0.1:14990/isrole?role=viewver").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Assert.AreEqual("true",str);
        }

        [Test]
        public void AdminCanCheckOtherRole()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=adm&pass=adm"));
            var str = cl.GetString("http://127.0.0.1:14990/isrole?login=usr&exact=true&role=viewver").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Assert.AreEqual("true", str);
        }

        [Test]
        public void NonAdminCannotCheckOtherRole()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=usr&pass=usr"));
            var str = cl.GetString("http://127.0.0.1:14990/isrole?login=adm&exact=true&role=admin").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Assert.True(str.Contains("System.Exception:notadmin"));
        }

        [Test]
        public void CanGetMyUserInfo()
        {
            cl.Cookies = new CookieCollection();
            Assert.AreEqual("true", cl.GetString("http://127.0.0.1:14990/logon?login=usr&pass=usr"));
            var str = cl.GetString("http://127.0.0.1:14990/myinfo").Simplify(SimplifyOptions.NoWs | SimplifyOptions.SingleQuotes);
            Console.WriteLine(str);
            Assert.True(str.Contains("{'Login':'usr','Name':'UU','Groups':['y'],'Roles':['viewver','x.all'],'IsActive':true}"));
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
            Assert.AreEqual("\"error\"", cl.GetString("http://127.0.0.1:14990/logon?login=a&pass=xxx"));
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
