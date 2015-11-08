namespace qorpent.IO {
    /// <summary>
    ///     LockFile mode flags
    /// </summary>
    public enum PidFileMode {
        None = 0,

        /// <summary>
        ///     If file exists - will check it's PID to be existed in System - if not - forcely clear it
        /// </summary>
        Forced = 1,

        /// <summary>
        ///     Waits file to be freed ( according to total timeout)
        /// </summary>
        Wait = 2,

        /// <summary>
        ///     Throw exceptions if cannot lock (already locked or timeout on wait lock)
        /// </summary>
        ThrowNoLock = 4,

        /// <summary>
        ///     By default - None - LockFile is not forced without timeout and exceptions
        /// </summary>
        Default = None
    }
}