using System;
using System.Text;
using qorpent.v2.security.encryption;
using Qorpent.IoC;

namespace qorpent.v2.security.logon.services {
    [ContainerComponent(Lifestyle.Transient, "securelogon.client", ServiceType = typeof (ISecureLogonClient))]
    public class SecureLogonClient : ISecureLogonClient {
        public SecureLogonInfo GetLogonInfo(string salt, byte[] certificate) {
            if (string.IsNullOrWhiteSpace(salt)) {
                throw new ArgumentException("salt not supplyed");
            }
            if (null == certificate || 0 == certificate.Length) {
                throw new ArgumentException("certificate not supplyed");
            }
            var encryptor = new Encryptor();
            var saltbytes = Convert.FromBase64String(salt);
            var decrypted = encryptor.Decrypt(saltbytes, certificate);
            var decryptedstr = Encoding.UTF8.GetString(decrypted);
            var hash = encryptor.SignData(decrypted, certificate);
            var hashstr = Convert.ToBase64String(hash);
            return new SecureLogonInfo {Salt = decryptedstr, Sign = hashstr};
        }
    }
}