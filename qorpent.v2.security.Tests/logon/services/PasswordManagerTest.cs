using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Test]
        public void CanGeneratePasswords() {
            var pm = new PasswordManager();
            var  passwords = new List<string>();
            for (var i = 0; i < 100; i++) {
                var p = pm.Generate();
                if (!pm.GetPolicy(p).Ok) {
                    Console.WriteLine("Pass:" + p);
                }
                Assert.True(pm.GetPolicy(p).Ok);
                passwords.Add(p);
            }
            Assert.AreEqual(passwords.Count,passwords.Distinct().Count());
            Assert.True(passwords.Any(_=>_.Contains("z")));
            Assert.True(passwords.Any(_=>_.Contains("q")));
            Assert.True(passwords.Any(_=>_.Contains("Z")));
            Assert.True(passwords.Any(_=>_.Contains("Q")));
            Assert.True(passwords.Any(_=>_.Contains("0")));
            Assert.True(passwords.Any(_=>_.Contains("9")));
            Assert.True(passwords.Any(_=>_.Contains("$")));
            Assert.True(passwords.Any(_=>_.Contains("+")));
        }
    }
}