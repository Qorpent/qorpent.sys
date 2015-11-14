using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qorpent.Security
{
    public static class SecurityConst {
        public const string ROLE_ADMIN = "ADMIN";
        public const string ROLE_SECURITY_ADMIN = "SECURE_SECURITY_ADMIN";
        public const string ROLE_GUEST = "GUEST";
        public const string ROLE_USER = "DEFAULT";
        public const string ROLE_DOMAIN_ADMIN = "SECURE_DOMAIN_ADMIN";
        public const string ROLE_DEMO_ACCESS = "SECURE_DEMO";
        public static TimeSpan LEASE_DEMO = TimeSpan.FromDays(7);
        public static TimeSpan LEASE_USER = TimeSpan.FromDays(365);
    }
}
