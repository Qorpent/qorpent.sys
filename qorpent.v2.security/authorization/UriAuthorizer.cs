using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent;
using Qorpent.IoC;
using Qorpent.v2.security.authorization;

namespace qorpent.v2.security.authorization
{

    public class UriAuthorizationRule {
        public string Selector { get; set; }
        public bool ProcessNotAuth { get; set; }
        public bool RedirectNotAuth { get; set; }
        public bool DenyNotAuth { get; set; }
        public string RedirectNotAuthUrl { get; set; }
        public bool CheckRole { get; set; }
        public bool CheckRoleExpression { get; set; }

    }

    [ContainerComponent(Lifestyle.Singleton,"sys.sec.uriauth", ServiceType=typeof(IUriAuthorizer))]
    public class UriAuthorizer:InitializeAbleService, IUriAuthorizer
    {
        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);

        }

        public AuthorizationReaction AuthorizeUri(IIdentity identity, Uri uri) {
            throw new NotImplementedException();
        }
    }
}
