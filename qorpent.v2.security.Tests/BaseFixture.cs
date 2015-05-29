using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.logon;
using qorpent.v2.security.Tests.user;
using qorpent.v2.security.user;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests
{
    public class BaseFixture
    {
        private Container _container;

        [SetUp]
        public virtual void Setup() {
            this._container = new Container();
            _container.GetLoader().LoadAssembly(typeof (ILogonService).Assembly);
            Loggy.Default.Level = LogLevel.Debug;
            if (Loggy.Default.Appenders.Count == 0) {
                Loggy.Default.Appenders.Add(new ConsoleAppender {
                    Format = "${Minute}:${Second} ${Message}",
                    Level = LogLevel.All
                });
            }
        }

        public static User GetUser(int idx) {
            return GetUser<User>(idx);
        }
        public static T GetUser<T>(int idx)where T:User,new() {
            var user = new T {
                Login = "login" + idx,
                Name = "myname" + idx,
                IsAdmin = idx%5 == 0,
                Logable = idx%3 == 0,
                IsGroup = idx%4 == 0,
                MasterGroup = "master" + idx,
                Roles = new[] {"role1_" + idx, "role2_" + idx},
                Groups = new[] {"grp1_" + idx, "grp2_" + idx},
                Email = "email" + idx,
                Salt = "salt" + idx
            };
            user.ResetExpire = user.ResetExpire.ToUniversalTime();
            user.Hash = (user.Salt + "pass" + idx + user.Salt).GetMd5();
            user.PublicKey =
                Convert.ToBase64String(
                    typeof (UserSerializationTest).Assembly.ReadManifestResourceBytes("public.cer"));
            user.Custom = new {a = "1_" + idx, b = "2_" + idx}.jsonify() as IDictionary<string, object>;
            user.Expire = new DateTime(2016, 1, 1).AddMonths(idx);
            return user;
        }
    }
}
