using System;

namespace qorpent.v2.security.user {
    [Flags]
    public enum UserActivityState {
        Ok = 0,
        Baned = 1,
        Expired = 2,
        MasterBaned = 4,
        MasterExpired = 8,
        InvalidMaster = 16,
        MasterNotChecked = 32
    }
}