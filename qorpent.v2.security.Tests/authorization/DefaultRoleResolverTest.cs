using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.Security;
using qorpent.v2.security.authorization;
using qorpent.v2.security.logon;
using qorpent.v2.security.user;

namespace qorpent.v2.security.Tests.authorization
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class DefaultRoleResolverTest:BaseFixture
    {
        [Test]
        public void CanGetPredefinedUserRoles() {
            var logon = _container.Get<ILogonService>();
            var roles = _container.Get<IRoleResolverService>();
            var id = (Identity)logon.Logon("fadm", FApass);
            Assert.True(id.IsAuthenticated);
            Assert.True(id.IsAdmin);
            Assert.True(roles.IsInRole(id, SecurityConst.ROLE_ADMIN));
            Assert.True(roles.IsInRole(id,"ZZZZ"));
            Assert.False(roles.IsInRole(id,"ZZZZ",true));
            Assert.True(roles.IsInRole(id,"role2",true));
        }
    }
}
