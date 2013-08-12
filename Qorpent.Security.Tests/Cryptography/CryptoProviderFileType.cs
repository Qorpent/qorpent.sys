namespace Qorpent.Security.Tests.Cryptography {
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
        ///     Supports all well-knows certificates
        /// </summary>
        AllWellKnown = Pem | Pkcs12
    }
}