using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using qorpent.v2.security.authorization;

namespace Qorpent.v2.security.authorization
{

    public interface IUriAuthorizer {
        AuthorizationReaction AuthorizeUri(IIdentity identity, Uri uri);
    }

}
