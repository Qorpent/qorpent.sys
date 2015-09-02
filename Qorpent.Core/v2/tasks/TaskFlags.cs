using System;

namespace qorpent.tasks {
    [Flags]
    public enum TaskFlags {
        None = 0,
        Async = 1
    }
}