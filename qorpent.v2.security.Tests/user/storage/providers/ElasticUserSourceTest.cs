using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage.providers;
using Qorpent.IO;
using Qorpent.IO.Net;

namespace qorpent.v2.security.Tests.user.storage.providers
{
    [TestFixture]
    public class ElasticUserSourceTest:BaseFixture
    {
        private bool _ignore;
        private ElasticUserSource _es;

        [SetUp]
        public void Setup() {
            var ping = ExecuteCommand("http://127.0.0.1:9200/_cluster/health");
            if (null == ping) {
                _ignore = true;
            }
            else {
                var user = GetUser(0);
                var userJs = UserSerializer.GetJson(user);
                var id = UserSerializer.GetId(user);
                ExecuteCommand("http://127.0.0.1:9200/elasticusersourcetest", method: "DELETE");
                var result = ExecuteCommand("http://127.0.0.1:9200/elasticusersourcetest/pwd/"+id,userJs );
                Assert.NotNull(result);
            }
            _es = new ElasticUserSource();
            _es.Index = "elasticusersourcetest";
        }

        [Test]
        public void CanGetUser() {
            if (_ignore) {
                Assert.Ignore("invalid ES environment");
            }
            var u = _es.GetUser("login0");
            Assert.NotNull(u);
            Assert.AreEqual("login0", u.Id);
            Console.WriteLine(u.ToString());
            Assert.Less(0,u.Version);
        }

        [Test]
        public void CanStoreUser() {
            if (_ignore)
            {
                Assert.Ignore("invalid ES environment");
            }
            IUser u = GetUser(1);
            u = _es.Store(u);
            _es.Refresh();
            var u2 = _es.GetUser("login1");
            Assert.AreNotSame(u,u2);
            Assert.AreEqual(u.GetToken(),u2.GetToken());
            Assert.AreEqual(u,u2);
        }

        [Test]
        [Explicit]
        public void PerformanceTest() {
            CheckRate(i =>
            {
                if (0 == i%100) {
                    _es.Refresh();
                    _es.Clear();
                }
                var u = _es.GetUser("login0");
                Assert.NotNull(u);
            }, 10000, "1/100 refresh");
            CheckRate(i => {
                _es.Refresh();
                _es.Clear();
               var u = _es.GetUser("login0");
                Assert.NotNull(u);
            },10000,"1/1 refresh");
          
        }

        [Test]
        public void WorkWithBadConnection() {
            //we emulate broken connection - change url to bad and restore it
            //it's same as es gain lost
            if (_ignore)
            {
                Assert.Ignore("invalid ES environment");
            }
            _es.PingRate = 200;
            _es.Urls = new []{"http://127.0.0.1:9201"};
            var u = _es.GetUser("login0");
            Assert.Throws<Exception>(() => {
                _es.Store(GetUser(0));
            });
            Assert.Null(u);
            Assert.True(_es.InvalidConnection);
            Assert.NotNull(_es.LastError);
            _es.Urls = new []{ "http://127.0.0.1:9200"};
            u = _es.GetUser("login0");
            Assert.Null(u);
            Thread.Sleep(201);
            u = _es.GetUser("login0");
            Assert.NotNull(u);  
        }

        [Test]
        public void CanSwitchNodes()
        {
            //we emulate broken connection - change url to bad and restore it
            //it's same as es gain lost
            if (_ignore)
            {
                Assert.Ignore("invalid ES environment");
            }
            _es.Urls = new List<string> { "http://127.0.0.1:9201","http://localhost:9200" };
            var u = _es.GetUser("login0");
            Assert.AreEqual("http://localhost:9200",_es.Urls[0]);
            Assert.NotNull(u);
            Assert.False(_es.InvalidConnection);
            Assert.Null(_es.LastError);
            _es.Refresh();
            _es.Urls = new List<string> { "http://127.0.0.1:9200", "http://localhost:9201" };
            u = _es.GetUser("login0");
            Assert.NotNull(u);
            Assert.False(_es.InvalidConnection);
            Assert.Null(_es.LastError);
        }
    }
}
