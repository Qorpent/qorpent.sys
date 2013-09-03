namespace Qorpent.Security.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public interface ICryptoSource {
        /// <summary>
        ///     Типы действий, поддерживаемых источником
        /// </summary>
        CryptoSourceSupportActions SupportActions { get; }
        
        /// <summary>
        ///     Типы файлов хранения сертификатов, которые поддерживает источник
        /// </summary>
        CryptoSourceSupportTypes SupportTypes { get; }

        /// <summary>
        ///     Execute an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        ICryptoProviderResult Execute(ICryptoProviderAction action);
    }
}