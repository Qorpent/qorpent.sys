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
        GroupInactive =1<<5,
        GroupExpired =1<<6,
        InvalidMasterGroup = 1<<7
    }
}