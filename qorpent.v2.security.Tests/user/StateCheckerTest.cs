using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using qorpent.v2.security.user;
using qorpent.v2.security.user.services;
using qorpent.v2.security.user.storage;

namespace qorpent.v2.security.Tests.user
{
    [TestFixture]
    public class StateCheckerTest
    {
        

        [Test]
        public void LoggableTest() {
            var checker = new UserStateChecker();
            var user = new User() {Logable = false};
            Assert.False(checker.IsLogable(user));
            user.Logable = true;
            Assert.True(checker.IsLogable(user));
            user.IsGroup = true;
            Assert.False(checker.IsLogable(user));
        }

        [Test]
        public void PasswordLogable() {
            var checker = new UserStateChecker();
            var user = new User {Logable = true};
            Assert.False(checker.IsPasswordLogable(user));
            user.Salt = "z";
            user.Hash = "d";
            Assert.True(checker.IsPasswordLogable(user));
            user.Salt = "";
            Assert.False(checker.IsPasswordLogable(user));
            user.Salt = "z";
            user.Hash = "";
            Assert.False(checker.IsPasswordLogable(user));
        }


        [Test]
        public void SecureLogable()
        {
            var checker = new UserStateChecker();
            var user = new User { Logable = true };
            Assert.False(checker.IsSecureLogable(user));
            user.PublicKey = "dada";
            Assert.True(checker.IsSecureLogable(user));
        }

        [Test]
        public void OkActivityState() {
            var checker = new UserStateChecker();
            var user = new User {Logable = true, Active = true, Expire = DateTime.Now.AddDays(10)};
            Assert.AreEqual(UserActivityState.Ok,checker.GetActivityState(user));
        }
        [Test]
        public void BanedActivityState()
        {
            var checker = new UserStateChecker();
            var user = new User { Logable = true, Active = false, Expire = DateTime.Now.AddDays(10) };
            Assert.AreEqual(UserActivityState.Baned, checker.GetActivityState(user));
        }
        [Test]
        public void ExpireActivityState()
        {
            var checker = new UserStateChecker();
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(-10) };
            Assert.AreEqual(UserActivityState.Expired, checker.GetActivityState(user));
        }

        [Test]
        public void MasterNotCheckedState()
        {
            var checker = new UserStateChecker();
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(10),Domain = "m1"};
            Assert.AreEqual(UserActivityState.MasterNotChecked, checker.GetActivityState(user));
        }

        class UserSource : IUserSource {
            public UserSource(params User[] users) {
                foreach (var user in users) {
                    _cache[user.Login] = user;
                }
            }
            IDictionary<string,User> _cache = new ConcurrentDictionary<string, User>(); 
            public int Idx { get; set; }

            public IUser GetUser(string login) {
                if (!_cache.ContainsKey(login)) {
                    return null;
                }
                return _cache[login];
            }

            public IEnumerable<IUser> SearchUsers(UserSearchQuery query) {
                return _cache.Values.Where(query.IsMatch);
            }
        }

        [Test]
        public void InvalidMasterState() {
            var us = new UserService();
            var checker = new UserStateChecker{UserService = us };
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(10), Domain = "m1" };
            Assert.AreEqual(UserActivityState.InvalidMaster, checker.GetActivityState(user));
        }


        [Test]
        public void OkWithMasterState()
        {
            var us = new UserService();
            us.RegisterExtension(new UserSource(new User{Login = "m1@groups",IsGroup = true,Active = true,Expire = DateTime.Now.AddDays(10)}));
            var checker = new UserStateChecker { UserService = us };
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(10), Domain = "m1" };
            Assert.AreEqual(UserActivityState.Ok, checker.GetActivityState(user));
        }

        [Test]
        public void MasterBanedState()
        {
            var us = new UserService();
            us.RegisterExtension(new UserSource(new User { Login = "m1@groups", IsGroup = true, Active = false, Expire = DateTime.Now.AddDays(10) }));
            var checker = new UserStateChecker { UserService = us };
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(10), Domain = "m1" };
            Assert.AreEqual(UserActivityState.MasterBaned, checker.GetActivityState(user));
        }
        [Test]
        public void MasterExpireState()
        {
            var us = new UserService();
            us.RegisterExtension(new UserSource(new User { Login = "m1@groups", IsGroup = true, Active = true, Expire = DateTime.Now.AddDays(-10) }));
            var checker = new UserStateChecker { UserService = us };
            var user = new User { Logable = true, Active = true, Expire = DateTime.Now.AddDays(10), Domain = "m1" };
            Assert.AreEqual(UserActivityState.MasterExpired, checker.GetActivityState(user));
        }
    }
}
