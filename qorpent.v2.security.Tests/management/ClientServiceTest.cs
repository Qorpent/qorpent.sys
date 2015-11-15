using System;
using NUnit.Framework;
using qorpent.Security;
using qorpent.v2.security.logon;
using qorpent.v2.security.logon.providers;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.management;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;
using qorpent.v2.security.user.storage.providers;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.management
{
    [TestFixture]
    public class ClientServiceTest
    {
        private DictionaryUserSource userStore;
        private UserService userService;
        private ClientService clientService;
        private InitClientRecord minimalValidRequest;
        private IPasswordManager passwordManager;
        private InitClientRecord doubleRequest;
        private IPasswordLogon passLogon;
        private IUserStateChecker userStateChecker;


        [SetUp]
        public void Setup() {
            clientService = new ClientService(
                userService = new UserService(
                    userStore = new DictionaryUserSource()));
            userStore[SecurityConst.ROLE_ADMIN.ToLowerInvariant()] = new User {Login = SecurityConst.ROLE_ADMIN,IsAdmin = true}.Activate();
            userStore[SecurityConst.ROLE_SECURITY_ADMIN.ToLowerInvariant()] = new User {Login = SecurityConst.ROLE_SECURITY_ADMIN, Roles = new [] {SecurityConst.ROLE_SECURITY_ADMIN}}.Activate();
            userStore[SecurityConst.ROLE_USER.ToLowerInvariant()] = new User {Login = SecurityConst.ROLE_USER }.Activate();
            userStore["existed@groups"] = new User {Login = "existed@groups",IsGroup = true}.Activate();
            minimalValidRequest = new InitClientRecord {Name = "ОАО Тест",IsDemo = true};
            doubleRequest = new InitClientRecord {Name = "ОАО Existed",IsDemo = true};
            passwordManager = new PasswordManager();
            passLogon = new PasswordLogon {UserService = userService};
            userStateChecker = new UserStateChecker {UserService = userService};
        }

        ClientResult getResult(string userName= null, InitClientRecord record = null) {
            return clientService.Init(userName??SecurityConst.ROLE_ADMIN, record??minimalValidRequest);
        }
        [TestCase(SecurityConst.ROLE_ADMIN)]
        [TestCase(SecurityConst.ROLE_SECURITY_ADMIN)]
        public void AdminAllowed(string username) {
            Assert.True(clientService.Init(username, minimalValidRequest).OK);
        }
        [Test]
        public void NonAdmiNotAllowed()
        {
            Assert.False(clientService.Init(SecurityConst.ROLE_USER, minimalValidRequest).OK);
        }

        [Test]
        public void SetupSysName() {
           Assert.AreEqual("test",getResult().GeneratedSysName);
        }

        [Test]
        public void GroupCreatedAndSetToResult() {
            var g = getResult().Group;
            Assert.NotNull(g);
            Assert.AreEqual("test@groups",g.Login);
            Assert.True(g.Active);
            Assert.True(g.IsGroup);
            Assert.True(DateTime.Now.AddDays(6)< g.Expire);
            Assert.False(DateTime.Now.AddDays(8)< g.Expire);
            Assert.True(g.Roles.Contains(SecurityConst.ROLE_DEMO_ACCESS));
            var saved = userService.GetUser(g.Login);
            Assert.NotNull(saved);
            Assert.True(saved.Equals(g));
        }

        [Test]
        public void UserCreatedAndSetToResult() {
            var r = getResult();
            var g = r.Group;
            var u = r.User;
            Assert.AreEqual("user000@test",u.Login);
            Assert.AreEqual(g.Name,u.Name);
            Assert.AreEqual(r.GeneratedSysName,u.Domain);
            Assert.True(u.Groups.Contains(u.Domain));
            Assert.False(string.IsNullOrWhiteSpace(u.Salt));
            Assert.False(string.IsNullOrWhiteSpace(u.Hash));
            Assert.True(u.Logable);
            var saved = userService.GetUser(u.Login);
            Assert.NotNull(saved);
            Assert.True(saved.Equals(u));
            Assert.True(userStateChecker.GetActivityState(u).HasFlag(UserActivityState.Ok));
        }

        [Test]
        public void UserIsLogableWithGivenPassword() {
            var r = getResult();
            var u = r.User;
            var id = passLogon.Logon(u.Login, r.GeneratedPassword);
            Assert.True(id.IsAuthenticated);
            Assert.AreEqual(u.Login,id.Name);
        }

        [Test]
        public void PasswordIsGenerated() {
            var r = getResult();
            Assert.False(string.IsNullOrWhiteSpace(r.GeneratedPassword));
            Assert.True(passwordManager.GetPolicy(r.GeneratedPassword).Ok);
        }

        [Test]
        public void DoublerNotEnabled() {
            var r = getResult(record: doubleRequest);
            Assert.False(r.OK);
        }

        [Test]
        public void CanMoveToWork() {
            var r = getResult();
            var exp = r.Group.Expire;
            var tw = clientService.ToWork(
                new Identity(userService.GetUser(SecurityConst.ROLE_ADMIN)),
                r.GeneratedSysName);
            Assert.True(tw.OK);
            Assert.NotNull(tw.Group);
            var g = userService.GetUser(tw.Group.Login);
            Assert.False(g.Roles.Contains(SecurityConst.ROLE_DEMO_ACCESS));
            Assert.Greater(g.Expire,exp);
            var user = userService.GetUser("user000@" + r.GeneratedSysName);
            Assert.AreEqual(user.Expire,g.Expire);
        }

        [Test]
        public void CanMoveToDemo()
        {
            var r = getResult();
            var tw = clientService.ToWork(
                new Identity(userService.GetUser(SecurityConst.ROLE_ADMIN)),
                r.GeneratedSysName);
            var g = userService.GetUser(tw.Group.Login);
            var exp = g.Expire;
            var td = clientService.ToDemo(
                new Identity(userService.GetUser(SecurityConst.ROLE_ADMIN)),
                r.GeneratedSysName
                );
            Assert.True(td.OK);
            Assert.NotNull(td.Group);
            g = userService.GetUser(tw.Group.Login);
            Assert.True(g.Roles.Contains(SecurityConst.ROLE_DEMO_ACCESS));
            Assert.Greater(exp, g.Expire);
            var user = userService.GetUser("user000@" + r.GeneratedSysName);
            Assert.AreEqual(user.Expire, g.Expire);
        }
    }
}
