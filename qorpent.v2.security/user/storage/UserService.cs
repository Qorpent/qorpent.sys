using System;
using System.Linq;
using Qorpent;
using Qorpent.IoC;

namespace qorpent.v2.security.user.storage {
    [ContainerComponent(Lifestyle.Singleton, "user.service", ServiceType = typeof(IUserService))]
    public class UserService : ExtensibleServiceBase<IUserSource>, IUserService {
        private IUserCache _userCache;

        [Inject]
        public IUserCache UserCache {
            get { return _userCache ??(_userCache = new UserCache()); }
            set { _userCache = value; }
        }

        public int Idx { get; set; }

        public IUser GetUser(string login) {
            var key = login.ToLowerInvariant();
            return UserCache.Get(key, InternalGetUser);
        }

        private IUser InternalGetUser(string key) {
            foreach (var userSource in Extensions) {
                var result = userSource.GetUser(key);
                if (null != result) {
                    var usrcb = result as IUserSourceBound;
                    if (null != usrcb) {
                        usrcb.UserSource = userSource;
                    }
                    var usrvb = result as IUserServiceBound;
                    if (null != usrvb) {
                        usrvb.UserService = this;
                    }
                    return result;
                }
            }
            return null;
        }

        public bool IsDefault {
            get { return true; }
        }

        public IUser Store(IUser user) {
            IWriteableUserSource writeable = ResolveWriteableStore(user);
            if (null == writeable) {
                throw new Exception("no target source to use as store");
            }
            var realuser = writeable.Store(user);
            _userCache.Clear();
            _userCache.Refresh();
            return realuser;
        }

        private IWriteableUserSource ResolveWriteableStore(IUser user) {
            var usrvb = user as IUserSourceBound;
            if (usrvb != null) {
                var src = usrvb.UserSource;
                if (null != src) {
                    if (src is IWriteableUserSource) {
                        return (IWriteableUserSource) src;
                    }
                    else {
                        return null; //if it was loaded from somewhere but it's not writeable  we cannot save
                    }
                }
            }
            var writeables = Extensions.OfType<IWriteableUserSource>().ToArray();
            if (0 == writeables.Length) return null;
            var def = writeables.FirstOrDefault(_ => _.IsDefault);
            if (null != def) return def;
            return writeables[0];
        }
    }
}