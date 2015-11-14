using System.Runtime.Remoting;
using System.Security.Principal;

namespace qorpent.v2.security.management
{
    /// <summary>
    /// Service for standard client registration
    /// </summary>
    public interface IClientService {
        /// <summary>
        /// Initializes group and main user for group
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        InitClientResult Init(IIdentity caller, InitClientRecord record);
    }
}
