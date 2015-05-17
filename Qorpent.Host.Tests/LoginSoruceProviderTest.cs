using System;
using NUnit.Framework;
using Qorpent.Security;

namespace Qorpent.Host.Tests {
    [TestFixture]
    public class LoginSoruceProviderTest {
        private DefaultLoginSourceProvider ls;

        [SetUp]
        public void Setup() {
            this.ls = new DefaultLoginSourceProvider {Sources = new[] {new TestLoginSource(),}};
        }
        [Test]
        public void CanGet()
        {
            var info = ls.Get("zadm");
            Assert.Null(info);
            info = ls.Get("adm");
            Assert.NotNull(info);
            Assert.NotNull(info.Provider);
            
        }

        [Test]
        public void CanAuth() {
            var info = ls.Get("adm");
            Assert.AreEqual(LogonAuthenticationResult.Ok, info.Logon("adm"));
            Assert.AreEqual(LogonAuthenticationResult.InvalidLoginInfo, info.Logon("adm2"));
        }

        [Test]
        public void CheckActivity()
        {
            var info = ls.Get("adm");
            info.RawIsActive = false;
            Assert.AreEqual(LogonAuthenticationResult.Inactive, info.Logon("adm"));
        }

        [Test]
        public void CheckExpiration()
        {
            var info = ls.Get("adm");
            info.Expire = DateTime.Now-TimeSpan.FromMinutes(10);
            Assert.AreEqual(LogonAuthenticationResult.Expired, info.Logon("adm"));
        }


        [Test]
        public void CanGetAdminRole() {
            var info = ls.Get("adm");
            Assert.True(info.IsInRole("ADMIN"));
            Assert.True(info.IsInRole("ANY"));
            Assert.False(info.IsInRole("ANY",true));
        }
        [Test]
        public void CanGetUsrRole()
        {
            var info = ls.Get("usr");
            Assert.False(info.IsInRole("ADMIN"));
            Assert.True(info.IsInRole("viewver"));
            Assert.True(info.IsInRole("viewver",true));
            Assert.False(info.IsInRole("x"));
            Assert.True(info.IsInRole("x.*"));
        }

        [Test]
        public void CanGetRoleFromGroup()
        {
            var info = ls.Get("usr");
            Assert.True(info.IsInRole("reporter"));
            Assert.True(info.IsInRole("y"));
        }

        [Test]
        public void CannotResetPasswordWithoutRequest() {
            var info = ls.Get("usr");
            var message =Assert.Throws<Exception>(()=>info.ResetPassword("1243","dsafdffs")).Message;
            Assert.True(message.Contains("password not pending to reset"));
        }

        [Test]
        public void CannotResetWithInvalidRequestKey() {
            var info = ls.Get("usr");
            info.InitResetPassword();
            var message = Assert.Throws<Exception>(() => info.ResetPassword("1243", info.ResetPasswordKey+"x")).Message;
            Assert.True(message.Contains("invalid validation key"));
        }

        [Test]
        public void CanResetWithValidRequestKey()
        {
            var info = ls.Get("usr");
            Assert.AreEqual(LogonAuthenticationResult.InvalidLoginInfo,info.Logon("1234"));
            info.InitResetPassword();
            info.ResetPassword("1234",info.ResetPasswordKey);
            Assert.AreEqual(LogonAuthenticationResult.Ok, info.Logon("1234"));
        }

        [Test]
        public void CannotReuseRequestKeyTwice()
        {
            var info = ls.Get("usr");
            Assert.AreEqual(LogonAuthenticationResult.InvalidLoginInfo, info.Logon("1234"));
            info.InitResetPassword();
            var rq = info.ResetPasswordKey;
            info.ResetPassword("1234", rq);
            Assert.AreEqual(LogonAuthenticationResult.Ok, info.Logon("1234"));
            var message = Assert.Throws<Exception>(() => info.ResetPassword("1243", rq)).Message;
            Assert.True(message.Contains("password not pending to reset"));
        }
    }
}