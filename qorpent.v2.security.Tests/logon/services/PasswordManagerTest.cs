using System;
using NUnit.Framework;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;

namespace qorpent.v2.security.Tests.logon.services {
    [TestFixture]
    public class PasswordManagerTest {
        [Test]
        public void CanSetAndCheckPassword() {
            var pwd = new PasswordManager();
            var user = new User{Login = "user1"};
            pwd.SetPassword(user,"cde3$RFV");
            Assert.Less(20,user.Salt.Length);
            Assert.Less(20,user.Hash.Length);
            Console.WriteLine(user.Salt+"::"+user.Hash);
            Assert.True(pwd.MatchPassword(user,"cde3$RFV"));
            Assert.False(pwd.MatchPassword(user,"cde3$RFv"));
            var s = user.Salt;
            var h = user.Hash;
            pwd.SetPassword(user, "cde3$RFV"); //check that hash changed
            Assert.AreNotEqual(s,user.Salt);
            Assert.AreNotEqual(h,user.Hash);
            Assert.True(pwd.MatchPassword(user, "cde3$RFV"));
            Assert.False(pwd.MatchPassword(user, "cde3$RFv"));
            Console.WriteLine(user.Salt + "::" + user.Hash);
        }

        [Test]
        public void CheckPolicyOnSet() {
            var pwd = new PasswordManager();
            var user = new User { Login = "user1" };
            Assert.Throws<Exception>(() => {
                pwd.SetPassword(user, "12345678");
            });
            pwd.SetPassword(user, "a234567$");
        }

        [Test]
        public void CanAvoidPolicyExplicit() {
            var pwd = new PasswordManager();
            var user = new User { Login = "user1" };
            pwd.SetPassword(user, "12345678",ignorepolicy:true);
        }

        [Test]
        public void CanUseExplicitSalt()
        {
            var pwd = new PasswordManager();
            var user = new User { Login = "user1" };
            pwd.SetPassword(user,"A1234567*", salt: "!23");
            Assert.AreEqual("!23",user.Salt);
        }
    }
}