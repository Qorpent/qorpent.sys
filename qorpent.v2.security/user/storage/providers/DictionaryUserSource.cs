using System.Collections.Generic;

namespace qorpent.v2.security.user.storage.providers {
    /// <summary>
    /// In memory dictionary-based user source
    /// </summary>
    public class DictionaryUserSource : Dictionary<string,IUser>, IUserSource, IWriteableUserSource
    {
        public int Idx { get; set; }
        public IUser GetUser(string login) {
            var key = Identity.GetIdentityKey(login);
            return ContainsKey(key) ? this[key] : null;
        }
        public bool IsDefault { get; set; }
        public bool WriteUsersEnabled { get; } = true;
        public IUser Store(IUser user) => this[Identity.GetIdentityKey(user.Login)] = user;
    }
}