namespace Qorpent.Security.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public enum CryptoProviderActionType {
        /// <summary>
        /// 
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
        GenerateCertificate = 4,

        /// <summary>
        ///     Encryption
        /// </summary>
        Encrypt = 5,

        /// <summary>
        ///     Decryption
        /// </summary>
        Decrypt = 6,
    }
}
