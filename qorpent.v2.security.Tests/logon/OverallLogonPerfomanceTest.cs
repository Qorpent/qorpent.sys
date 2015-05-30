using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.logon;
using Qorpent.Log;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.Tests.logon
{
    [TestFixture]
    public class OverallLogonPerfomanceTest:BaseFixture
    {
        [Test]
        [Explicit]
        public void TestOf10000Logins() {
            Loggy.Default.Level = LogLevel.Error;
            CheckRate(i => {
               Assert.True( _container.Get<ILogonService>().Logon("fuser", Fpass).IsAuthenticated);
               Assert.True(_container.Get<ILogonService>().Logon("esuser", Epass).IsAuthenticated);
            });
            Loggy.Default.Level = LogLevel.Debug;
        }
    }
}
