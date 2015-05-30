using System.Net;

namespace qorpent.v2.security.authentication {
    public class TrustedOrigin {
        public string LocalAddressName { get; set; }
        public string RemoteAddressName { get; set; }
        public IPAddress LocalAddress { get; set; }
        public IPAddress RemoteAddress { get; set; }
        public int LocalPort { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }

        public bool IsMatch(IPEndPoint remote, IPEndPoint local, string origin) {
            if (!string.IsNullOrWhiteSpace(Origin) && Origin != "*") {
                if (Origin != origin) {
                    return false;
                }
            }
            if (null != LocalAddress) {
                if (!local.Address.Equals(LocalAddress)) return false;
            }
            if (null != RemoteAddress) {
                if (!remote.Address.Equals(RemoteAddress)) return false;
            }
            if (0 != LocalPort) {
                if (local.Port != LocalPort) return false;
            }
            return true;
        }
    }
}