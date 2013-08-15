using System.Collections.Generic;

namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    class CryptoProviderAction : ICryptoProviderAction {
        /// <summary>
        /// 
        /// </summary>
        public CryptoProviderActionType ActionType { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public CryptoProviderEntity Entity { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> Config { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="entity"></param>
        public CryptoProviderAction(CryptoProviderActionType actionType, CryptoProviderEntity entity) {
            ActionType = actionType;
            Entity = entity;
            Config = new Dictionary<string, string>();
        }
    }
}
