using System;
using System.Collections.Generic;
using System.Text;
using qorpent.v2.security.encryption;
using qorpent.v2.security.user;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon.services {
    [ContainerComponent(Lifestyle.Singleton, "pwd.manager", ServiceType = typeof (IPasswordManager))]
    public class PasswordManager : IPasswordManager {
        public void SetPassword(IUser target, string password, bool ignorepolicy = false, string salt = null) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (string.IsNullOrWhiteSpace(target.Login)) {
                throw new Exception("not login");
            }
            if (string.IsNullOrWhiteSpace(password)) {
                throw new Exception("no password");
            }
            var policy = GetPolicy(password);
            if (!ignorepolicy && !policy.Ok) {
                throw new Exception("Invalid password policy " + policy);
            }
            if (!string.IsNullOrWhiteSpace(salt)) {
                target.Salt = salt;
            }
            else {
                var e = new Encryptor();
                target.Salt = e.Encrypt(Guid.NewGuid() + "::" + target.Login + "::" + password).GetMd5();
            }
            var hash = GetHash(target, password);
            target.Hash = hash;
        }

        public bool MatchPassword(IUser target, string password) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (string.IsNullOrWhiteSpace(password)) {
                throw new ArgumentException("empty password");
            }
            if (string.IsNullOrWhiteSpace(target.Salt)) {
                throw new Exception("salt not set on target");
            }
            if (string.IsNullOrWhiteSpace(target.Hash)) {
                throw new Exception("hash not set on target");
            }
            var hash = GetHash(target, password);
            return target.Hash == hash;
        }

        public IPasswordPolicy GetPolicy(string password) {
            return new PasswordPolicy(password);
        }

        public void ResetPassword(IUser target, string password, string key) {
            if (string.IsNullOrWhiteSpace(target.ResetKey)) {
                throw new Exception("no reset requested");
            }
            if (DateTime.Now.ToUniversalTime() > target.ResetExpire) {
                throw new Exception("expire reseting");
            }
            if (key != target.ResetKey) {
                throw new Exception("invalid key");
            }
            SetPassword(target, password);
            target.ResetExpire = DateTime.MinValue.ToUniversalTime();
            target.ResetKey = null;
        }

        public void MakeRequest(IUser target, int expireminutes, string email = null) {
            if (string.IsNullOrWhiteSpace(target.Email)) {
                throw new Exception("email must be set for targets");
            }
            if (null != email) {
                if (email != target.Email) {
                    throw new Exception("wrong email");
                }
            }
            var resetkey = (Guid.NewGuid() + target.Login + DateTime.Now).GetMd5();
            target.ResetKey = resetkey;
            target.ResetExpire = DateTime.Now.ToUniversalTime().AddMinutes(expireminutes);
        }

        Random r = new Random();

        public string Generate() {
            char[] chars = {'z', 'w', 'v', 's', 'f','q'}; //only with nomix ru-eng
            char[] symbols = {'$', '%', '@', '!', '=', '+'};
            var b =new StringBuilder();
            var fst = chars[r.Next(chars.Length)];
            var sec = chars[r.Next(chars.Length)];
              if (r.Next(5) <= 3) {
                fst = char.ToUpperInvariant(fst);
            }
            else {
                sec = char.ToUpperInvariant(sec);
            }
            b.Append(fst);
            b.Append(sec);
            List<int> ints = new List<int> {0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            for (var i = 0; i < 5; i++) {
                var num = r.Next(ints.Count);
                var val = ints[num];
                ints.RemoveAt(num);
                b.Append(val.ToString());
            }
            b.Append(symbols[r.Next(symbols.Length)]);
            return b.ToString();
        }

        private static string GetHash(IUser target, string password) {
            return (target.Salt + "::" + target.Login + "::" + password + "::" + target.Salt).GetMd5();
        }
    }
}