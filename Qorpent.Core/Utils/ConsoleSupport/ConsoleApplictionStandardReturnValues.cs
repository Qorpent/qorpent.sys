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
        /// Application known but not explicitly coded error
        /// </summary>
        AppError =-101,
        /// <summary>
        /// Unknown system error
        /// </summary>
        SysError =-102
    }
}