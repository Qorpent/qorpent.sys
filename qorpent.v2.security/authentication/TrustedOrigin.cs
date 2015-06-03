using System.Net;
using Qorpent.Experiments;
using Qorpent.Log.NewLog;

namespace qorpent.v2.security.authentication {
    public class TrustedOrigin {
        private ILoggy logg;
        private IPAddress _localAddress;
        private IPAddress _remoteAddress;

        public TrustedOrigin() {
            this.logg = Loggy.Get("sys.sec.trusted");

        }
        public string LocalAddressName { get; set; }
        public string RemoteAddressName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IPAddress LocalAddress {
            get {
                if (null == _localAddress && !string.IsNullOrWhiteSpace(LocalAddressName) && LocalAddressName!="*") {
                    _localAddress = IPAddress.Parse(LocalAddressName);
                }
                return _localAddress;
            }
            set { _localAddress = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IPAddress RemoteAddress {
            get {
                if (null == _remoteAddress && !string.IsNullOrWhiteSpace(RemoteAddressName) && RemoteAddressName != "*")
                {
                    _remoteAddress = IPAddress.Parse(RemoteAddressName);
                }
                return _remoteAddress;
            }
            set { _remoteAddress = value; }
        }

        public int LocalPort { get; set; }
        public string Origin { get; set; }
        public string Name { get; set; }

        public bool IsMatch(IPEndPoint remote, IPEndPoint local, string origin) {
            
            var result =  InternalMatch(remote, local, origin);
            if (logg.IsForDebug()) {
                var loggm = new {
                    lan = LocalAddressName,
                    ran = RemoteAddressName,
                    lp = LocalPort,
                    o = Origin,
                    n = Name,
                    rm = remote.ToString(),
                    lc = local.ToString(),
                    or = origin,
                    rs = result
                };
                logg.Debug(loggm.stringify());
            }
            return result;
        }

        private bool InternalMatch(IPEndPoint remote, IPEndPoint local, string origin) {
            if (!string.IsNullOrWhiteSpace(Origin) && Origin != "*") {
                if (Origin != origin) {
                    return false;
                }
            }
            if (null != LocalAddress) {
                if (!local.Address.Equals(LocalAddress)) {
                    return false;
                }
            }
            if (null != RemoteAddress) {
                if (!remote.Address.Equals(RemoteAddress)) {
                    return false;
                }
            }
            if (0 != LocalPort) {
                if (local.Port != LocalPort) {
                    return false;
                }
            }
            return true;
        }
    }
}