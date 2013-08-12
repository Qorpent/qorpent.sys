namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public enum CryptoProviderActionType {
        /// <summary>
        ///     Do nothing
        /// </summary>
        None = 0,

        /// <summary>
        ///     Verify the certificate
        /// </summary>
        Verify = 1,

        /// <summary>
        ///     Sign the certificate
        /// </summary>
        Sign = 2,

        /// <summary>
        ///     Generate an CRT file
        /// </summary>
        GenerateRequest = 3,

        /// <summary>
        ///     Generate a certificate by request
        /// </summary>
        GenerateCertificate = 4
    }
}
