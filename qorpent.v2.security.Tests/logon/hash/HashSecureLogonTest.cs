using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.encryption;
using qorpent.v2.security.logon.providers;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent.Events;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.logon.hash
{
    [TestFixture]
    public class HashSecureLogonTest
    {
        [Test]
        public void CanGetSalt() {
            var logon = new SecureLogon {UserService = new SimpleHashLogonStorage()};
            var salt = logon.GetSalt("user");
            Console.WriteLine(salt);
            Assert.Less(100,salt.Length);
        }

        [Test]
        public void CanDealWithLogon() {
            var logon = new SecureLogon { UserService = new SimpleHashLogonStorage() };
            var salt = logon.GetSalt("user");
            var encryptor = new Encryptor();
            var cert = typeof (HashSecureLogonTest).Assembly.ReadManifestResourceBytes("private.pfx");
            var saltbytes = Convert.FromBase64String(salt);
            var decrypted = encryptor.Decrypt(saltbytes, cert);
            var decryptedstr = Encoding.UTF8.GetString(decrypted);
            var hash = encryptor.SignData(decrypted, cert);
            var hashstr = Convert.ToBase64String(hash);
            var result = logon.Logon("user", new SecureLogonInfo{Salt = decryptedstr,Sign = hashstr});
            Assert.True(result.IsAuthenticated);
        }
    }

    public class SimpleHashLogonStorage : IUserService {
        public IEnumerable<IUserSource> GetExtensions() {
            throw new NotImplementedException();
        }

        public void RegisterExtension(IUserSource extension) {
            throw new NotImplementedException();
        }

        public void RemoveExtension(IUserSource extension) {
            throw new NotImplementedException();
        }

        public void ClearExtensions() {
            throw new NotImplementedException();
        }

        public int Idx {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public IUser GetUser(string login) {
            return new User {
                Login = login,
                Logable = true,
                Active = true,
                Expire = DateTime.Now.AddDays(1),
                PublicKey =
                    Convert.ToBase64String(
                        typeof (SimpleHashLogonStorage).Assembly.ReadManifestResourceBytes("public.cer"))
            };
        }

        public IEnumerable<IUser> SearchUsers(UserSearchQuery query) {
            yield break;
        }

        public bool IsDefault {
            get { throw new NotImplementedException(); }
        }

        public bool WriteUsersEnabled {
            get { throw new NotImplementedException(); }
        }

        public IUser Store(IUser user) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }
    }
}
