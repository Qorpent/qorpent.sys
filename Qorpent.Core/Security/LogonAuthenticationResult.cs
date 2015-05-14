using System;

namespace Qorpent.Security {
    [Flags]
    public enum LogonAuthenticationResult
    {
        None = 0,
        Ok = 1,
        Inactive = 1<<1,
        Expired = 1<<2,
        InvalidLoginInfo = 1<<3,
        InvalidRecord = 1<<4,
    }
}