using System;
using System.Net;
using System.Xml.Linq;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.IoC;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "httptoken.service", ServiceType = typeof (IHttpTokenService))]
    public class HttpTokenService : InitializeAbleService, IHttpTokenService {
        public HttpTokenService() {
            CookieName = "QHAUTH";
            LeaseTime = 30;
            SlideTime = 10;
            Sliding = true;
        }

        [Inject]
        public ITokenEncryptor TokenEncryptor { get; set; }

        public int LeaseTime { get; set; }
        public int SlideTime { get; set; }
        public bool Sliding { get; set; }
        public bool Clustered { get; set; }
        public bool Secure { get; set; }
        public string Domain { get; set; }
        public string CookieName { get; set; }

        public Token Extract(IHttpRequestDescriptor request) {
            var cookie = ReadCookie(request);
            if (string.IsNullOrWhiteSpace(cookie)) {
                return null;
            }
            return TokenEncryptor.Decrypt(cookie);
        }

        public bool IsValid(IHttpRequestDescriptor request, Token token) {
            if (token.Expire < DateTime.Now.ToUniversalTime()) {
                return false;
            }
            var metrics = GetMetrics(request);
            if (metrics != token.Metrics) {
                return false;
            }
            return true;
        }

        public void Store(IHttpResponseDescriptor response, Uri requestUri, Token token) {
            var cookie = new Cookie();
            if (Secure) {
                cookie.Secure = true;
            }
            cookie.Path = "/";
            cookie.HttpOnly = true;
            cookie.Domain = GetDomain(requestUri);
            cookie.Name = CookieName;
            if (null == token) {
                cookie.Expires = DateTime.Now.AddYears(-1);
                cookie.Value = "";
            }
            else {
                cookie.Expires = token.Expire.AddMinutes(5);
                cookie.Value = TokenEncryptor.Encrypt(token);
            }
            response.Cookies = response.Cookies ?? new CookieCollection();
            response.Cookies.Add(cookie);
        }

        public Token Create(IHttpRequestDescriptor request) {
            var result = new Token();
            result.User = request.User.Identity.Name;
            var qid = request.User.Identity as Identity;
            if (null != qid) {
                if (null != qid.ImpersonationSource) {
                    result.ImUser = qid.ImpersonationSource.Name;
                }
            }
            result.Created = DateTime.Now.ToUniversalTime();
            result.IsAdmin = qid.IsAdmin;
            if (LeaseTime == -1) {
                result.Expire = DateTime.Today.AddDays(1).AddSeconds(-1).ToUniversalTime();
            }
            else {
                result.Expire = DateTime.Now.AddMinutes(LeaseTime).ToUniversalTime();
            }
            result.Metrics = GetMetrics(request);
            if (null != qid.ImpersonationSource) {
                result.ImUser = qid.ImpersonationSource.Name;
            }
            return result;
        }

        public Token Prolongate(Token existedToken) {
            if (Sliding) {
                existedToken.Expire = DateTime.Now.AddMinutes(SlideTime);
            }
            return existedToken;
        }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);
            Clustered = null != e.Element("cluster");
            e = e.Element("logon");

            if (null != e) {
                e = e.Element("token");
            }
            if (null != e) {
                CookieName = e.Attr("cookie", CookieName);
                LeaseTime = e.Attr("leasetime", LeaseTime.ToString()).ToInt();
                SlideTime = e.Attr("slidetime", SlideTime.ToString()).ToInt();
                Sliding = e.Attr("slide", Sliding.ToString()).ToBool();
                Secure = e.Attr("secure", Secure.ToString()).ToBool();
                Domain = e.Attr("domain");
            }
        }

        private string ReadCookie(IHttpRequestDescriptor request) {
            var cookie = request.GetHeader("Cookie");
            if (string.IsNullOrWhiteSpace(cookie)) {
                return null;
            }
            var headerStart = cookie.IndexOf(CookieName);
            if (headerStart == -1) {
                return string.Empty;
            }
            var eq = cookie.IndexOf('=', headerStart);
            if (eq == -1) {
                return string.Empty;
            }
            if (eq == cookie.Length - 1) {
                return string.Empty;
            }
            var finish = cookie.IndexOfAny(new[] {',', ' ', ';'}, eq);
            if (finish == -1) {
                finish = cookie.Length;
            }
            var result = cookie.Substring(eq + 1, finish - (eq + 1));
            return Uri.UnescapeDataString(result);
        }

        private string GetDomain(Uri requestUrl) {
            if (!string.IsNullOrWhiteSpace(Domain)) {
                return Domain;
            }
            var domain = requestUrl.Host;
            if (requestUrl.HostNameType == UriHostNameType.Dns) {
                var parts = domain.Split('.');
                if (parts.Length == 1) {
                    return parts[0];
                }
                return "." + parts[parts.Length - 2] + "." + parts[parts.Length - 1];
            }
            return domain;
        }

        private string GetMetrics(IHttpRequestDescriptor request) {
            if (Clustered) {
                return (request.RemoteEndPoint.Address + "_" +
                        request.UserAgent).GetMd5().Substring(0, 6);
            }
            return (request.RemoteEndPoint.Address + "_" + request.LocalEndPoint.Address + "_" +
                    request.UserAgent).GetMd5().Substring(0, 6);
        }
    }
}