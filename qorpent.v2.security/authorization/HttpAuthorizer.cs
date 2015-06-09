using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using qorpent.v2.security.authentication;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.v2.security.authorization;

namespace qorpent.v2.security.authorization
{
    [ContainerComponent(Lifestyle.Singleton, "sys.sec.http.authorizer", ServiceType = typeof(IHttpAuthorizer))]
    public class HttpAuthorizer:InitializeAbleService, IHttpAuthorizer
    {
        [Inject]
        public IRoleResolverService Roles { get; set; }

        [Inject]
        public INotAuthProcessProvider NotAuth { get; set; }

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
            if (0 == Rules.Count) return null;
            var uri = request.Uri;
            var identity = request.User.Identity;
            var rule = Rules.Where(_ => _.IsMatch(uri)).OrderByDescending(_ => _.Idx).FirstOrDefault();           
            if (!identity.IsAuthenticated) {
                if (null != rule && rule.IsForNotAuth) {
                    return rule.GetNotAuth(request);
                }
                return NotAuth.GetReaction(request);
            }
            if (null == rule) return AuthorizationReaction.Allow;
            if (!rule.CheckRole) return AuthorizationReaction.Allow;
            if (Roles.IsInRole(request.User.Identity, rule.Role)) return AuthorizationReaction.Allow;
            return AuthorizationReaction.Deny;
        }
    }
}
