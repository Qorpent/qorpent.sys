using System;

namespace Qorpent {
    public interface ICacheLease {
        bool Refresh();
        string ETag { get; }
        DateTime Version { get; }
    }
}