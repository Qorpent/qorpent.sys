using Qorpent;

namespace qorpent.v2.security.logon {
    /// <summary>
    ///     External interface of Logon Service
    /// </summary>
    public interface ILogonService : ILogonProvider, IPasswordLogon, ISecureLogon, IExtensibleService<ILogonProvider> {
        /// <summary>
        ///     True if SecureLogon is configured
        /// </summary>
        bool SupportSecureLogon { get; }
    }
}