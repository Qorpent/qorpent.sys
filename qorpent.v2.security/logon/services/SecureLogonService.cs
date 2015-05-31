using System;
using System.Security;
using System.Text;
using qorpent.v2.security.encryption;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.logon.services {
    /// <summary>
    /// </summary>
    [ContainerComponent(Lifestyle.Transient, "security.logon", ServiceType = typeof (ISecureLogonService))]
    public class SecureLogonService : ISecureLogonService {
        public SecureLogonService() {
            CheckTimeout = 5000;
            Encryptor = new Encryptor();
        }

        public int CheckTimeout { get; set; }
        public Encryptor Encryptor { get; set; }

        public string GetSalt(IUser record, IScope context = null) {
            var data = record.Login + ":" + DateTime.Now.ToOADate();
            var timeout = context == null ? CheckTimeout : context.Get("logontimeout", CheckTimeout);
            data += ":" + timeout;
            var salt = Encryptor.Encrypt(data);
            var pubkey = Convert.FromBase64String(record.PublicKey);
            var saltbytes = Encoding.UTF8.GetBytes(salt);
            var encsalt = Encryptor.Encrypt(saltbytes, pubkey);
            var encsaltstr = Convert.ToBase64String(encsalt);
            return encsaltstr;
        }

        public void CheckSecureInfo(SecureLogonInfo info, IUser record, IScope context = null) {
            var resalt = "";
            try {
                //it means that it was encrypted with our private key
                resalt = Encryptor.Decrypt(info.Salt);
            }
            catch (Exception e) {
                throw new SecurityException("salt was forbidden");
            }
            var saltparts = resalt.Split(':');
            if (3 != saltparts.Length) {
                throw new Exception("invalid salt");
            }
            if (saltparts[0] != record.Login) {
                throw new Exception("invalid salt");
            }
            var dbl = Convert.ToDouble(saltparts[1]);
            var basetime = DateTime.FromOADate(dbl);
            var timeout = Convert.ToInt32(saltparts[2]);
            var expire = basetime.AddMilliseconds(timeout);
            if (expire < DateTime.Now) {
                throw new Exception("timeouted");
            }
            var opencert = Convert.FromBase64String(record.PublicKey);
            var hashbytes = Convert.FromBase64String(info.Sign);
            try {
                if (!Encryptor.CheckSign(Encoding.UTF8.GetBytes(info.Salt), hashbytes, opencert)) {
                    throw new SecurityException("invalid sign");
                }
            }
            catch {
                throw new SecurityException("invalid sign");
            }
        }
    }
}