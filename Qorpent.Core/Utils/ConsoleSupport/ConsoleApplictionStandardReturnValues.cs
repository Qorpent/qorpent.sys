namespace Qorpent.Utils {
    /// <summary>
    /// Default return values
    /// </summary>
    /// <remarks>
    /// Locks default error codes,
    /// application can define it's own in following rule
    /// [1..100] known non-error results but not OK ( exit while singleton and so on)
    /// [-100..-1] known error states
    /// </remarks>
    public enum ConsoleApplictionStandardReturnValues: int {
        

        /// <summary>
        /// Success
        /// </summary>
        Success = 0,

        /// <summary>
        /// Returned if EXE is configured as singleton and exits as doubler
        /// </summary>
        SingletonExit = 1,
        /// <summary>
        /// Returned if EXE decide that is nothing to do
        /// </summary>
        WorkDoneBefore = 2,
        /// <summary>
        /// Situation of aborted execution due to TEST proposes
        /// </summary>
        TestTermination = 3,
        /// <summary>
        /// Application known but not explicitly coded error
        /// </summary>
        AppError =-101,
        /// <summary>
        /// Unknown system error
        /// </summary>
        SysError =-102,
        /// <summary>
        /// Internal none value const
        /// </summary>
        None = int.MinValue,

        
    }
}