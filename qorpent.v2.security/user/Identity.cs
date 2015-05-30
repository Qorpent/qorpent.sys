using System;
using System.Security.Principal;
using qorpent.v2.security.authentication;

namespace qorpent.v2.security.user {
    /// <summary>
    /// </summary>
    public class Identity : IIdentity {
        /// <summary>
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// </summary>
        public Exception Error { get; set; }

        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
        public IIdentity Native { get; set; }
        public IUser User { get; set; }
        public IIdentity ImpersonationSource { get; set; }
        public Token Token { get; set; }


        
    }
}