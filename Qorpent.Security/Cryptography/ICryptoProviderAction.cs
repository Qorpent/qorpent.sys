using System.Collections.Generic;

namespace Qorpent.Security.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public interface ICryptoProviderAction {
        /// <summary>
        /// 
        /// </summary>
        CryptoProviderActionType ActionType { get; }

        /// <summary>
        ///     Certificate
        /// </summary>
        CryptoProviderEntity Entity { get; }

        /// <summary>
        ///     Addictional source config
        /// </summary>
        IDictionary<string, string> Config { get; }
    }
}