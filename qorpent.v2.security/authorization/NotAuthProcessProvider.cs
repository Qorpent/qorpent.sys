using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authorization
{

    public class NotAuthReaction {
        public string Redirect { get; set; }
        public bool Process { get; set; }
        public Exception Error { get; set; }

        
    }
    public interface INotAuthProcessProvider {
        NotAuthReaction GetReaction(IIdentity notauthidentity, IHttpRequestDescriptor request);
    }

    [ContainerComponent(Lifestyle.Singleton,"notauth.provider",ServiceType=typeof(INotAuthProcessProvider))]
    public class NotAuthProcessProvider : ServiceBase, INotAuthProcessProvider {
        public NotAuthReaction GetReaction(IIdentity notauthidentity, IHttpRequestDescriptor request) {
            if (notauthidentity.IsAuthenticated) return new NotAuthReaction {Process = true};
            var uri = request.Uri;
            if (IsProcessAble(uri)) {
                return new NotAuthReaction {Process = true};
            }
            var redir = RedirectHtml(uri);
            if (null != redir) return redir;
            return new NotAuthReaction {Error = new SecurityException("not authenticated")};
        }

        private static NotAuthReaction RedirectHtml(Uri uri) {
            if (uri.AbsolutePath.EndsWith(".html")) {
                return new NotAuthReaction {Redirect = "/login.html?referer=" + uri.PathAndQuery};
            }
            return null;
        }

        private bool IsProcessAble(Uri uri) {
            var path = uri.AbsolutePath;

            if (path.EndsWith(".js"))
            {
                return true;
            }
            if (path.EndsWith(".css"))
            {
                return true;
            }
            if (path.EndsWith("login.html"))
            {
                return true;
            }
            if (path.EndsWith(".css.map"))
            {
                return true;
            }
            if (path.EndsWith(".js.map"))
            {
                return true;
            }
            if (path.StartsWith("/views/") && path.EndsWith("html")) {
                return true;
            }
            if (path == "/login") {
                return true;
            }
            if (uri.AbsolutePath == "/logout")
            {
                return true;
            }
            if (uri.AbsolutePath == "/isauth")
            {
                return true;
            }
            return false;
        }
    }
}
