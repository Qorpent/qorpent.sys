namespace Qorpent.Security.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public enum CryptoProviderFileType {
        /// <summary>
        ///     PEM
        /// </summary>
        Pem = 0,

        /// <summary>
        ///     PKCS12
        /// </summary>
        Pkcs12 = 1,

        /// <summary>
        ///     PKCS7
        /// </summary>
        Pkcs7 = 2,

        /// <summary>
        ///     Supports all well-knows certificates
        /// </summary>
        AllWellKnown = Pem | Pkcs12
    }
}