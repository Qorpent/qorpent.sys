using System;
using System.Security;
using System.Security.Principal;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace qorpent.v2.security.authorization {
    [ContainerComponent(Lifestyle.Singleton, "notauth.provider", ServiceType = typeof (INotAuthProcessProvider))]
    public class NotAuthProcessProvider : ServiceBase, INotAuthProcessProvider {
        public AuthorizationReaction GetReaction(IIdentity notauthidentity, IHttpRequestDescriptor request) {
            if (notauthidentity.IsAuthenticated) {
                return new AuthorizationReaction {Process = true};
            }
            var uri = request.Uri;
            if (IsProcessAble(uri)) {
                return new AuthorizationReaction {Process = true};
            }
            var redir = RedirectHtml(uri);
            if (null != redir) {
                return redir;
            }
            return new AuthorizationReaction {Error = new SecurityException("not authenticated")};
        }

        private static AuthorizationReaction RedirectHtml(Uri uri) {
            if (uri.AbsolutePath.EndsWith(".html")) {
                return new AuthorizationReaction {Redirect = "/login.html?referer=" + uri.PathAndQuery};
            }
            return null;
        }

        private bool IsProcessAble(Uri uri) {
            var path = uri.AbsolutePath;

            if (path.EndsWith(".js")) {
                return true;
            }
            if (path.EndsWith(".png"))
            {
                return true;
            }
            if (path.EndsWith(".jpg"))
            {
                return true;
            }
            if (path.EndsWith(".jpeg"))
            {
                return true;
            }
            if (path.EndsWith(".css")) {
                return true;
            }
            if (path.EndsWith("login.html")) {
                return true;
            }
            if (path.EndsWith("resetpwd.html")) {
                return true;
            }
            if (path.EndsWith("resetpwdreq.html")) {
                return true;
            }
            if (path.EndsWith(".css.map")) {
                return true;
            }
            if (path.EndsWith(".js.map")) {
                return true;
            }
            if (path.StartsWith("/views/") && path.EndsWith("html")) {
                return true;
            }
            if (path == "/logon") {
                return true;
            }
            if (uri.AbsolutePath == "/info/app") {
                return true;
            }
            if (uri.AbsolutePath == "/logout") {
                return true;
            }
            if (uri.AbsolutePath == "/isauth") {
                return true;
            }
            if (uri.AbsolutePath == "/resetpwd") {
                return true;
            }
            if (uri.AbsolutePath == "/resetpwdreq") {
                return true;
            }
            return false;
        }
    }
}