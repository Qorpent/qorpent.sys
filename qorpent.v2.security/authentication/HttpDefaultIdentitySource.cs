using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Xml.Linq;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "http.defaultidentitysource", ServiceType = typeof(IHttpDefaultIdentitySource))]   
    public class HttpDefaultIdentitySource : ServiceBase, IHttpDefaultIdentitySource {
        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        public HttpDefaultIdentitySource() {
            //by default we use full-trust schema
            ApplyFullTrustSchema();
            TrustedOrigins = new List<TrustedOrigin>();
            AnyGuestName = "guest";
            LocalGuestName = "localguest";

        }

        public bool AllowGuest { get; set; }
        public bool AllowLocalGuest { get; set; }
        public bool LocalGuestIsAdmin { get; set; }
        public bool AnyGuestIsAdmin { get; set; }
        public IList<TrustedOrigin> TrustedOrigins { get; set; }
        public string AnyGuestName { get; set; }
        public string LocalGuestName { get; set; }

        public void Initialize() {
            if (null != ConfigProvider) {
                var e = ConfigProvider.GetConfig();
                if (null != e) {
                    e = e.Element("logon");
                }
                if (null != e) {
                    e = e.Element("default");
                }
                if (null != e) {
                    InitializeFromXml(e);
                }
            }
        }

        public void InitializeFromXml(XElement element) {
            var schema = element.ChooseAttr("schema","code");
            if (!string.IsNullOrWhiteSpace(schema)) {
                if ("localguest"==schema)
                {
                    ApplyLocalGuestSchema();
                }
                else if ("anyguest"==schema)
                {
                    ApplyAnyGuestSchema();
                }
                else if ("localtrust"==schema)
                {
                    ApplyLocalTrustSchema();
                }
                else if ("fulltrust"==schema)
                {
                    ApplyFullTrustSchema();
                    
                }
                else if ("secure" == schema)
                {
                    ApplySecureSchema();

                }
            }
            AllowGuest = element.Attr("guest", AllowGuest.ToString()).ToBool();
            AllowLocalGuest = element.Attr("localguest", AllowLocalGuest.ToString()).ToBool();
            LocalGuestIsAdmin = element.Attr("localguestadmin", LocalGuestIsAdmin.ToString()).ToBool();
            AnyGuestIsAdmin = element.Attr("guestadmin", AnyGuestIsAdmin.ToString()).ToBool();

            AllowGuest = AllowGuest && AllowLocalGuest;
            LocalGuestIsAdmin = AllowLocalGuest && LocalGuestIsAdmin;
            AnyGuestIsAdmin = AnyGuestIsAdmin && LocalGuestIsAdmin;
            

            AnyGuestName = element.Attr("guestname", AnyGuestName);
            if (string.IsNullOrWhiteSpace(AnyGuestName)) {
                AnyGuestName = "guest";
            }

            LocalGuestName = element.Attr("localguestname", LocalGuestName);
            if (string.IsNullOrWhiteSpace(AnyGuestName))
            {
                AnyGuestName = "localguest";
            }
            var i = 0;
            foreach (var e in element.Elements("trusted")) {
                
                var rip = e.Attr("remote", "127.0.0.1");
                if (string.IsNullOrWhiteSpace(rip)) {
                    rip = "127.0.0.1";
                }
                
                var lip = e.Attr("local", "127.0.0.1");
                if (string.IsNullOrWhiteSpace(lip)) {
                    lip = "127.0.0.1";
                }
                var lport = e.Attr("port", "0").ToInt();
                var origin = e.Attr("origin", "*");
                if (string.IsNullOrWhiteSpace(origin)) {
                    origin = "*";
                }
                IPAddress remote = null;
                IPAddress local = null;

                if (rip != "*") {
                    remote = IPAddress.Parse(rip);
                }
                if (lip != "*") {
                    local =IPAddress.Parse(lip);
                }
                var name = e.Attr("code");
                if (string.IsNullOrWhiteSpace(name)) {
                    name = "trusted" + i;
                }
                
                var trusted = new TrustedOrigin {
                    LocalAddressName = lip,
                    RemoteAddressName = rip,
                    LocalAddress =  local,
                    RemoteAddress = remote,
                    LocalPort = lport,
                    Origin = origin,
                    Name = name
                };
                TrustedOrigins.Add(trusted);
                i++;
            }

        }

        private void ApplyAnyGuestSchema() {
            AllowGuest = true;
            AllowLocalGuest = true;
            LocalGuestIsAdmin = false;
            AnyGuestIsAdmin = false;
        }
        private void ApplySecureSchema()
        {
            AllowGuest = false;
            AllowLocalGuest = false;
            LocalGuestIsAdmin = false;
            AnyGuestIsAdmin = false;
        }

        private void ApplyLocalGuestSchema() {
            AllowGuest = false;
            AllowLocalGuest = true;
            LocalGuestIsAdmin = false;
            AnyGuestIsAdmin = false;
        }

        private void ApplyLocalTrustSchema() {
            AllowGuest = false;
            AllowLocalGuest = true;
            LocalGuestIsAdmin = true;
            AnyGuestIsAdmin = false;
        }

        private void ApplyFullTrustSchema() {
            AllowGuest = true;
            AllowLocalGuest = true;
            LocalGuestIsAdmin = true;
            AnyGuestIsAdmin = true;
        }


        public IIdentity GetUserIdentity(IHttpRequestDescriptor request) {
            var name = AnyGuestName;
            bool auth = AllowGuest;
            bool isadmin = AnyGuestIsAdmin;
            string type = "guest";
            bool guest = true;
            if (IsLocal(request.LocalEndPoint)) {
                name = LocalGuestName;
                auth = AllowLocalGuest;
                isadmin = LocalGuestIsAdmin;
                type = "localguest";
            }
            TrustedOrigin origin = null;
            if (!isadmin && (origin = GetTrust(request))!=null) {
                guest = false;
                auth = true;
                isadmin = true;
                type = "trusted";
                name = origin.Name;
            }

            return new Identity {
                Name = name,
                IsAuthenticated = auth,
                AuthenticationType = type,
                IsAdmin = isadmin,
                IsGuest = guest
            };
        }

        private TrustedOrigin GetTrust(IHttpRequestDescriptor request) {
            var origin = request.GetHeader("Origin") ?? "";
            return TrustedOrigins.FirstOrDefault(_ => _.IsMatch(request.RemoteEndPoint, request.LocalEndPoint, origin));
        }

        private static readonly IPAddress LocalIp4 = IPAddress.Parse("127.0.0.1");
        private static readonly IPAddress LocalIp6 = IPAddress.Parse("::1");

        private bool IsLocal(IPEndPoint ep) {
            return ep.Address.Equals(LocalIp4) || ep.Address.Equals( LocalIp6);
        }
    }
}