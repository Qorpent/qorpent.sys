using System;

namespace Qorpent.Tasks {
    [Flags]
    public enum TaskState {
        Init = 0,
        Pending = 1,
        Finished = 2,
        Error = Finished | 4,
        CascadeError = Error | 8,
        Executing = 16,
        Success = Finished | 32,
        SuccessOnce = Success | 64,
        SuccessNotRun = Success | 128,
        SuccessIgnoreErrors = Success | 256
    }
}