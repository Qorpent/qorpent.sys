﻿using System;

namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum CryptoSourceSupportTypes {
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