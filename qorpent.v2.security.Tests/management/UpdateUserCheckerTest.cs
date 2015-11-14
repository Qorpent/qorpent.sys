using System;
using NUnit.Framework;
using qorpent.v2.security.authorization;
using qorpent.v2.security.management;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using qorpent.v2.security.user.storage.providers;
using Qorpent.Experiments;
using static qorpent.Security.SecurityConst;
namespace qorpent.v2.security.Tests.management
{
    [TestFixture]
    public class UpdateUserCheckerTest {


        DictionaryUserSource  userSource;
        private IUserService users;
        private IRoleResolverService roles;
        private IUpdateUserChecker updateChecker;
    
        [SetUp]
        public void Setup() {
            roles = new RoleResolverService(
                users = new UserService(userSource = new DictionaryUserSource {IsDefault = true}), 
                new RoleResolver {Users = users});
            updateChecker = new UpdateUserChecker {Roles = roles, Users = users};

            users.Store(new User {Login = ROLE_ADMIN, Roles = new[] { ROLE_ADMIN }, IsAdmin = true}.Activate());
            users.Store(new User {Login = ROLE_GUEST, Roles = new[] { ROLE_GUEST } }.Activate());
            users.Store(new User {Login = ROLE_USER, Roles = new[] { ROLE_USER } }.Activate());
            users.Store(new User {Login = ROLE_SECURITY_ADMIN, Roles = new[] { ROLE_SECURITY_ADMIN } }.Activate());
            users.Store(new User {Login = ROLE_DOMAIN_ADMIN, Roles = new[] { ROLE_DOMAIN_ADMIN } , Domain = ROLE_DOMAIN_ADMIN }.Activate());

        }

        [Test]
        public void AdminAllowed() {
            DoTest(true, ROLE_ADMIN);
        }

        [Test]
        public void SecityAdminAllowed() {
            DoTest(true, ROLE_SECURITY_ADMIN);
        }

        [Test]
        public void DomainAdminCanOwnDomain() {
            DoTest(true, ROLE_DOMAIN_ADMIN, new UserUpdateInfo {Domain = ROLE_DOMAIN_ADMIN, Email = "my@email.ru"});
        }

        [Test]
        public void DomainAdminCanNotNotOwnDomain()
        {
            DoTest(false, ROLE_DOMAIN_ADMIN, new UserUpdateInfo { Domain = ROLE_DOMAIN_ADMIN+"_2", Email = "my@email.ru" });
        }

        [Test]
        public void internal_Is_Admin_Admin() {
            var u = users.GetUser(ROLE_ADMIN);
            Assert.True(roles.IsInRole(new Identity(u),ROLE_ADMIN ));
            u = users.GetUser(ROLE_SECURITY_ADMIN);
            Assert.True(roles.IsInRole(new Identity(u),ROLE_SECURITY_ADMIN));
        }

        private void DoTest(bool expectedResult, string callerName, UserUpdateInfo update = null, User existed = null) {
            update = update ?? new UserUpdateInfo();
            if (string.IsNullOrEmpty(update.Login)) {
                update.Login = "user1";
            }
            userSource.Remove(Identity.GetIdentityKey(update.Login));
            if (null != existed) {
                userSource.Store(existed); //setup as update
            }
            var me = users.GetUser(callerName);
            var result = updateChecker.ValidateUpdate(new Identity(me) {IsAuthenticated = true}, update, existed);
            if (!result.Ok) {
                Console.WriteLine(result.stringify());
            }
            Assert.AreEqual(expectedResult,result.Ok);
        }
    }
}
