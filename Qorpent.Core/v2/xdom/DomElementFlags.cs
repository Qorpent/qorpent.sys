using System;

namespace qorpent.v2.xdom {
    [Flags]
    public enum DomElementFlags {
        None = 0,
        AllowElements = 1,
        AllowText = 1<<1,
        RequireValue = 1<<2,
        PreferHead = 1<<3,
        Default = AllowElements | AllowText
    }
}