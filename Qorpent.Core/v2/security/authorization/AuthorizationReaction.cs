using System;
using System.Security;

namespace qorpent.v2.security.authorization {
    public class AuthorizationReaction {
        public string Redirect { get; set; }
        public bool Process { get; set; }
        public Exception Error { get; set; }
        public static readonly AuthorizationReaction Allow = new AuthorizationReaction { Process = true };
        public static readonly AuthorizationReaction Deny = new AuthorizationReaction { Error = new SecurityException("not auth") };
    }
}