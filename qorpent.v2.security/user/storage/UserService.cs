using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.security.user.storage {
    [ContainerComponent(Lifestyle.Singleton, "user.service", ServiceType = typeof (IUserService))]
    public class UserService : ExtensibleServiceBase<IUserSource>, IUserService {
        private IUserCache _userCache;

        [Inject]
        public IUserCache UserCache {
            get { return _userCache ?? (_userCache = new UserCache()); }
            set { _userCache = value; }
        }

        public UserService() {
            
        }

        public UserService(params IUserSource[] sources) {
            if (null != sources) {
                foreach (var userSource in sources) {
                    RegisterExtension(userSource);
                }
            }
        }

        public int Idx { get; set; }

        public IUser GetUser(string login) {
            var key = login.ToLowerInvariant();
            return UserCache.Get(key, InternalGetUser);
        }

        public IEnumerable<IUser> SearchUsers(UserSearchQuery query) {
            return Extensions.SelectMany(_ => _.SearchUsers(query));
        }

        public bool IsDefault {
            get { return true; }
        }

        public bool WriteUsersEnabled {
            get { return Extensions.OfType<IWriteableUserSource>().Any(_ => _.WriteUsersEnabled); }
        }

        public IUser Store(IUser user) {
            if (!WriteUsersEnabled) {
                throw new Exception("Storing not enabled");
            }
            var writeable = ResolveWriteableStore(user);
            if (null == writeable) {
                throw new Exception("no target source to use as store");
            }
            if (!writeable.WriteUsersEnabled) {
                throw new Exception("requested storage not enabled");
            }
            var realuser = writeable.Store(user);
            if (null != _userCache) {
                _userCache.Clear();
                _userCache.Refresh();
            }
            return realuser;
        }

        public void Clear() {
            _userCache.Clear();
        }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            InitializeLeases();
        }

        private void InitializeLeases() {
            var cacheleases = UserCache.GetExtensions().ToList();
            foreach (var userSource in Extensions) {
                var lease = userSource as IUserCacheLease;
                if (null != lease) {
                    if (!cacheleases.Contains(lease)) {
                        UserCache.RegisterExtension(lease);
                    }
                }
            }
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

        private IWriteableUserSource ResolveWriteableStore(IUser user) {
            var usrvb = user as IUserSourceBound;
            if (usrvb != null) {
                var src = usrvb.UserSource;
                if (null != src) {
                    if (src is IWriteableUserSource) {
                        return (IWriteableUserSource) src;
                    }
                    return null; //if it was loaded from somewhere but it's not writeable  we cannot save
                }
            }
            var writeables = Extensions.OfType<IWriteableUserSource>().Where(_ => _.WriteUsersEnabled).ToArray();
            if (0 == writeables.Length) {
                return null;
            }
            var def = writeables.FirstOrDefault(_ => _.IsDefault);
            if (null != def) {
                return def;
            }
            return writeables[0];
        }
    }
}