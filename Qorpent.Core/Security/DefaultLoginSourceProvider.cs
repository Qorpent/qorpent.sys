using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.Security {
    [ContainerComponent(Lifestyle.Singleton, ServiceType = typeof(ILoginSourceProvider))]
    public class DefaultLoginSourceProvider : ILoginSourceProvider {
        public IEnumerable<LoginInfo> Query(LoginInfo match = null) {
            IList<LoginInfo> result = new List<LoginInfo>();
            foreach (var source in Sources) {
                foreach (var loginInfo in source.Query(match)) {
                    _cache[loginInfo.Login] = loginInfo;
                    loginInfo.Provider = this;
                    result.Add(loginInfo);
                }
            }
            return result.ToArray();
        }

        public LoginInfo Get(string login) {
            return _cache.GetOrAdd(login, _ => {
                LoginInfo result = null;
                foreach (var loginSource in Sources) {
                    var loginInfo = loginSource.Get(login);
                    if (null != loginInfo) {
                         if (null == result) {
                            result = loginInfo;
                        }
                        else {
                            result.Merge(loginInfo);
                        }
                    }
                }
                if (null != result) {
                    result.Provider = this;
                }
                return result;
            });            
        }

        readonly ConcurrentDictionary<string, LoginInfo> _cache = new ConcurrentDictionary<string, LoginInfo>();
        [Inject]
        public ILoginSource[] Sources { get; set; }

        public object Reset(ResetEventData data) {
            if (data.All || data.IsSet("login-source")) {
                foreach (var source in Sources) {
                    source.Reset(data);
                }
            }
            _cache.Clear();
            return true;
        }

        public object GetPreResetInfo() {
            return null;
        }
    }
}