using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.encryption;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.Tests.encryption
{
    [TestFixture]
    public class EncryptorMainTest
    {
        [Test]
        public void CanEncryptAndDecrypt() {
            var encryptor = new Encryptor();
            var encrypted = encryptor.Encrypt("test");
            Assert.Less(10,encrypted.Length);
            Assert.AreNotEqual("test",encrypted);
            var decrypted = encryptor.Decrypt(encrypted);
            Assert.AreEqual("test",decrypted);
        }

        [Test]
        public void CanEncryptAndDecryptWithX509() {
            var pubkey = typeof (EncryptorMainTest).Assembly.ReadManifestResourceBytes("public.cer");
            var privkey = typeof(EncryptorMainTest).Assembly.ReadManifestResourceBytes("private.pfx");
            var encryptor = new Encryptor();
            var encrypted = encryptor.Encrypt(Encoding.UTF8.GetBytes("test"), pubkey);
            Console.WriteLine(Convert.ToBase64String(encrypted));
            var decrypted = encryptor.Decrypt(encrypted, privkey);
            var decryptedtext = Encoding.UTF8.GetString(decrypted);
            Assert.AreEqual("test",decryptedtext);
        }

        [Test]
        public void CanSignAndVerifyWithX509() {
            var pubkey = typeof(EncryptorMainTest).Assembly.ReadManifestResourceBytes("public.cer");
            var privkey = typeof(EncryptorMainTest).Assembly.ReadManifestResourceBytes("private.pfx");
            var encryptor = new Encryptor();
            var data = Encoding.UTF8.GetBytes("test");
            var sign = encryptor.SignData(data, privkey);
            Assert.True(encryptor.CheckSign(data,sign,pubkey));
        }
    }
}
