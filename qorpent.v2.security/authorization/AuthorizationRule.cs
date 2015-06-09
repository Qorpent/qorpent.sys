using System;
using System.Xml.Linq;
using Qorpent.IO.Http;
using Qorpent.Model;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authorization {
    public class AuthorizationRule :IWithIndex{

        public AuthorizationRule() {
            
        }
        public AuthorizationRule(XElement element) {
            Selector = element.Attr("code");
            var role = element.Attr("role");
            if (!string.IsNullOrWhiteSpace(role)) {
                Role = role;
                CheckRole = true;
            }
            var notauth = element.Attr("notauth");
            if (!string.IsNullOrWhiteSpace(notauth)) {
                if (notauth == "deny") {
                    DenyNotAuth = true;
                }
                else if (notauth == "allow") {
                    ProcessNotAuth = true;
                }
                else {
                    RedirectNotAuth = true;
                    RedirectNotAuthUrl = notauth;
                }
                
            }
        }

        public int Idx { get; set; }
        public string Selector { get; set; }
        public bool ProcessNotAuth { get; set; }
        public bool RedirectNotAuth { get; set; }
        public bool DenyNotAuth { get; set; }
        public string RedirectNotAuthUrl { get; set; }
        public bool CheckRole { get; set; }
        public string Role { get; set; }

        public bool IsForNotAuth {
            get { return ProcessNotAuth || RedirectNotAuth || DenyNotAuth; }
        }
        public bool IsMatch(Uri uri) {
            var path = uri.AbsolutePath;
            if (Selector.EndsWith("*")) return path.StartsWith(Selector.Substring(0, Selector.Length - 1));
            return path == Selector;
        }

        public AuthorizationReaction GetNotAuth(IHttpRequestDescriptor request) {
            if (ProcessNotAuth) return AuthorizationReaction.Allow;
            if (DenyNotAuth) return AuthorizationReaction.Deny;
            return new AuthorizationReaction {Redirect = RedirectNotAuthUrl + "?referer=" + request.Uri.AbsolutePath};
        }
        
    }
}