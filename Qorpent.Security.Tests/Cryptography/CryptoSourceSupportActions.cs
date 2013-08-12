using System;

namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CryptoSourceSupportActions {
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
        ///     Can generate requests and certificates
        /// </summary>
        Generate = GenerateRequest | GenerateCertificate,

        /// <summary>
        ///     Encryption
        /// </summary>
        Encrypt = 6,

        /// <summary>
        ///     Decryption
        /// </summary>
        Decrypt = 7,

        /// <summary>
        ///     Supports encription and decription
        /// </summary>
        EncryptAndDecrypt = Encrypt | Decrypt,

        /// <summary>
        /// 
        /// </summary>
        Full = Verify | Sign | Generate | EncryptAndDecrypt
    }
}