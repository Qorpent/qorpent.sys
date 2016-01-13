using System.Collections.Generic;
using Qorpent.IoC;
using Qorpent.IO.Http;

namespace Qorpent.Host {
    public class RedirectService : ServiceBase,IRedirectService {
        [Inject]
        public IList<IRedirectSource> Redirectors { get; set; }
        public string GetRedirectUrl(IHttpRequestDescriptor req) {
            foreach (var redirectSource in Redirectors) {
                var redirect = redirectSource.GetRedirectUrl(req);
                if (!string.IsNullOrWhiteSpace(redirect)) {
                    return redirect;
                }
            }
            return null;
        }
    }
}