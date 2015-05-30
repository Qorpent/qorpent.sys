using System.Collections.Generic;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.authentication;
using qorpent.v2.security.authorization;
using qorpent.v2.security.logon;
using qorpent.v2.security.user.storage;
using Qorpent.IO.Http;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.Tests
{
    [TestFixture]
    public class PerformaceCheck:BaseFixture
    {
        [Test]
        [Explicit]
        public void LogonAuthCycleWithTokenAndRefresh() {
            Loggy.Default.Level = LogLevel.Error;

            CheckRate(i => {
                if (i%100 == 0) {
                    var es = (UserService) _container.Get<IUserService>();
                    es.UserCache.Clear();
                }
                var identity = _container.Get<ILogonService>().Logon("esuser", Epass);
                var req = new HttpRequestDescriptor
                {
                    User = new GenericPrincipal(identity, null),
                    UserAgent = "testagent",
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
                };
                var roles =(RoleResolverService) _container.Get<IRoleResolverService>();
                roles.Cache.Clear();
                var isinrole = roles.IsInRole(identity, "ADMIN");
                isinrole = roles.IsInRole(identity, "role2");
                isinrole = roles.IsInRole(identity, "role1");
                

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


        [Test]
        [Explicit]
        public void ParallelLoading()
        {
            Loggy.Default.Level = LogLevel.Error;
            var tasks = new List<Task>();
            CheckRate(i =>
            {
                tasks.Add(Task.Run(()=>{
                if (i % 100 == 0)
                {
                    var es = (UserService)_container.Get<IUserService>();
                    es.UserCache.Clear();
                }
                var identity = _container.Get<ILogonService>().Logon("esuser", Epass);
                var req = new HttpRequestDescriptor
                {
                    User = new GenericPrincipal(identity, null),
                    UserAgent = "testagent",
                    RemoteEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3456)
                };
                var roles = (RoleResolverService)_container.Get<IRoleResolverService>();
                roles.Cache.Clear();
                var isinrole = roles.IsInRole(identity, "ADMIN");
                isinrole = roles.IsInRole(identity, "role2");
                isinrole = roles.IsInRole(identity, "role1");


                var hts = _container.Get<IHttpTokenService>();
                var token = hts.Create(req);
                var enc = _container.Get<ITokenEncryptor>();
                var cookie = enc.Encrypt(token);
                req.Headers = new Dictionary<string, string>();
                req.Headers["Cookie"] = "testauth=" + cookie;
                var token2 = hts.Extract(req);
                hts.IsValid(req, token2);
                }));
                if (i%2 == 0 || i==9999) {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            });
            Loggy.Default.Level = LogLevel.All;
        }
    }
}
