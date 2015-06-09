using NUnit.Framework;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;

namespace qorpent.v2.security.Tests.authorization
{
    [TestFixture]
    public class EvaluatesRolesWithExpressions
    {
        private UserService _us;
        private User _u;
        private Identity _id;
        private RoleResolverService _rr;

        [SetUp]
        public void Setup() {
            _u = new User {Name = "user", Roles = new[] {"r1", "r2", "r3"}};
            _id = new Identity(_u) {IsAuthenticated = true};
            _us = new UserService();
            _us.UserCache.Get("user", _ => _u);
            _rr = new RoleResolverService();
            _rr.RegisterExtension(new RoleResolver());
            _rr.Users = _us;
        }

        [TestCase("r1",true)]
        [TestCase("!r1",false)]
        [TestCase("-r1",false)]
        [TestCase("r2",true)]
        [TestCase("r3",true)]
        [TestCase("r4",false)]
        [TestCase("!r4",true)]
        [TestCase("-r4",true)]
        [TestCase("r1,r2",true)]
        [TestCase("r4,r1",true)]
        [TestCase("r4 | r1",true)]
        [TestCase("r1+r2",true)]
        [TestCase("r1 & r2",true)]
        [TestCase("r1+r4",false)]
        [TestCase("r1 & r4",false)]
        [TestCase("r1 & !r4",true)]
        [TestCase("r1 & !r2",false)]
        [TestCase("r1 & ( r2 | r4) ",true)]
        public void TestExpression(string ex, bool result) {
            Assert.AreEqual(result,_rr.IsInRole(_id,ex));
        }
    }
}
