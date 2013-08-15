using System;

namespace Qorpent.Security.Tests.Cryptography {
    /// <summary>
    /// 
    /// </summary>
    public class CryptoProvider {
        public ICryptoSource CryptoSource { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cryptoSource"></param>
        public CryptoProvider(ICryptoSource cryptoSource) {
            CryptoSource = cryptoSource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public ICryptoProviderResult Execute(ICryptoProviderAction action) {
            if (!(CheckCryptoSourceCapability(action))) {
                throw new Exception("This action or file type is not supports by this cryptography source");
            }

            return Roll(action);
        }

        /// <summary>
        ///     Real action rolling
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private ICryptoProviderResult Roll(ICryptoProviderAction action) {
            return CryptoSource.Execute(action);
        }

        /// <summary>
        ///     Check that our cryptography supports needed actions and file types
        /// </summary>
        /// <param name="cryptoProviderAction">Crypto action</param>
        /// <returns>true if all going well</returns>
        private bool CheckCryptoSourceCapability(ICryptoProviderAction cryptoProviderAction) {
            if (!CryptoSource.SupportActions.HasFlag(cryptoProviderAction.ActionType)) {
                return false;
            }

            if (!CryptoSource.SupportTypes.HasFlag(cryptoProviderAction.Entity.FileType)) {
                return false;
            }

            return true;
        }
    }
}
