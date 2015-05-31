using System;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;

namespace qorpent.v2.security.management {
    public class UserPolicy {
        public bool Logable { get; set; }
        public int ExpirationDays { get; set; }
        public bool Active { get; set; }
        public bool MakePassRequest { get; set; }
        public int RequestMinutes { get; set; }

        public void Apply(IUser user) {
            if (Logable) {
                user.Logable = true;
            }
            if (0 < ExpirationDays) {
                user.Expire = DateTime.Now.ToUniversalTime().AddDays(ExpirationDays);
            }
            if (Active) {
                user.Active = true;
            }
            if (MakePassRequest) {
                if (RequestMinutes <= 0) {
                    RequestMinutes = 60*24;
                }
                new PasswordManager().MakeRequest(user, RequestMinutes);
            }
        }
    }
}