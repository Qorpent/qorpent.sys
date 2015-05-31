using Qorpent.Model;

namespace qorpent.v2.security.logon {
    /// <summary>
    ///     Interface for logon workers that must supply real logon functionality
    /// </summary>
    public interface ILogonProvider : ILogon, IWithIndex {
    }
}