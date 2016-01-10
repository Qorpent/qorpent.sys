using System;

namespace Qorpent {
    public interface ICacheLease {
        bool Refresh(bool force);
        string ETag { get; }
        DateTime Version { get; }
    }
}