using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authorization
{
    [ContainerComponent(Lifestyle.Singleton, "sys.sec.http.authorizer", ServiceType = typeof(IHttpAuthorizer))]
    public class HttpAuthorizer:InitializeAbleService, IHttpAuthorizer
    {
        private IRoleResolverService _roles;
        private INotAuthProcessProvider _notAuth;

        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public IRoleResolverService Roles {
            get { return _roles ?? (_roles = new RoleResolverService()); }
            set { _roles = value; }
        }

        [Inject]
        public INotAuthProcessProvider NotAuth {
            get { return _notAuth ?? (_notAuth = new NotAuthProcessProvider()); }
            set { _notAuth = value; }
        }

        [Inject]
        public IRedirectService Redirector { get; set; }

        public HttpAuthorizer() {
            Rules =new List<AuthorizationRule>();
        }
        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);
            var rules = e.Elements("authrule");
            var idx = 0;
            foreach (var element in rules) {
                idx += 10;
                var r = new AuthorizationRule(element);
                if (0 == r.Idx) {
                    r.Idx = idx;
                }
                Rules.Add(r);
            }

        }
        public IList<AuthorizationRule> Rules { get; private set; }
      

        public AuthorizationReaction Authorize(IHttpRequestDescriptor request) {
            var uri = request.Uri;
            var identity = request.User.Identity;
            var rule = Rules.Where(_ => _.IsMatch(uri)).OrderByDescending(_ => _.Idx).FirstOrDefault();
            AuthorizationReaction result;
            if (!identity.IsAuthenticated) {
                if (null != rule && rule.IsForNotAuth) {
                    result = rule.GetNotAuth(request);
                }
                else {
                    result = NotAuth.GetReaction(request);

                }
            }
            else {
                if (null == rule) result = AuthorizationReaction.Allow;
                else if (!rule.CheckRole) result = AuthorizationReaction.Allow;
                else if (Roles.IsInRole(request.User.Identity, rule.Role)) result = AuthorizationReaction.Allow;
                else return AuthorizationReaction.Deny; 
            }

            if (result.Process && null != Redirector && string.IsNullOrWhiteSpace(result.Redirect)) {
                var redirect = Redirector.GetRedirectUrl(request);
                if (!string.IsNullOrWhiteSpace(redirect)) {
                    result = new AuthorizationReaction {
                        Redirect = redirect
                    };
                }
            }
            return result;
        }
    }
}
