using System;
using qorpent.v2.security.user;

namespace qorpent.v2.security.management {
    public class ClientResult {
        public IUser Group { get; set; }
        public IUser User { get; set; }
        public string GeneratedPassword { get; set; }
        public bool OK { get; set; }
        public string GeneratedSysName { get; set; }
        public Exception Error { get; set; }
    }
}