using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.encryption;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.Tests.logon.hash;
using qorpent.v2.security.user;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.logon.services
{

    [TestFixture]
    public class SecureLogonServiceTest
    {
        IUser _sample = new User
        {
            Login = "user",
            PublicKey =
                Convert.ToBase64String(
                    typeof(SimpleHashLogonStorage).Assembly.ReadManifestResourceBytes("public.cer"))
        };

        [Test]
        public void CanGetSalt() {
            var securelogon = new SecureLogonService();
            var salt = securelogon.GetSalt(_sample);
            Console.WriteLine(salt);
            Assert.Less(100, salt.Length);
        }

        [Test]
        public void CanDealWithLogon()
        {
            var securelogon = new SecureLogonService();
            var salt = securelogon.GetSalt(_sample);
            var encryptor = new Encryptor();
            var cert = typeof(HashSecureLogonTest).Assembly.ReadManifestResourceBytes("private.pfx");
            var saltbytes = Convert.FromBase64String(salt);
            var decrypted = encryptor.Decrypt(saltbytes, cert);
            var decryptedstr = Encoding.UTF8.GetString(decrypted);
            var hash = encryptor.SignData(decrypted, cert);
            var hashstr = Convert.ToBase64String(hash);
            securelogon.CheckSecureInfo(new SecureLogonInfo{Salt = decryptedstr,Sign = hashstr}, _sample);
        }

        [Test]
        public void CanDealWithSecureLogonClient() {
            var securelogon = new SecureLogonService();
            var salt = securelogon.GetSalt(_sample);
            var secureclient = new SecureLogonClient();
            var cert = Assembly.GetExecutingAssembly().ReadManifestResourceBytes("private.pfx");
            var logoninfo = secureclient.GetLogonInfo(salt,cert);
            securelogon.CheckSecureInfo(logoninfo, _sample);
        }
    }
}
