namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public interface ICryptoProviderResult {
        /// <summary>
        ///     Is success
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        ///     Source action
        /// </summary>
        ICryptoProviderAction Action { get; }

        /// <summary>
        ///     Responce of the Crypto Provider
        /// </summary>
        CryptoProviderEntity Entity { get; }
    }

    class CryptoProviderResult : ICryptoProviderResult {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ICryptoProviderAction Action { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public CryptoProviderEntity Entity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="action"></param>
        /// <param name="isSuccess"></param>
        public CryptoProviderResult(CryptoProviderEntity entity, ICryptoProviderAction action, bool isSuccess) {
            IsSuccess = isSuccess;
            Action = action;
            Entity = entity;
        }
    }
}