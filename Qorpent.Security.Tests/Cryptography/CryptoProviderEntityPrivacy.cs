namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    ///     Entity privacy
    /// </summary>
    public enum CryptoProviderEntityPrivacy {
        /// <summary>
        ///     Public key, certificate or message
        /// </summary>
        Public = 0,

        /// <summary>
        ///     Private key, certificate or message
        /// </summary>
        Private = 1
    }
}