using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Qorpent.Experiments;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Security {
    public interface ILoginInfo {
        string Login { get; set; }
        int Version { get; set; }
        string Name { get; set; }
        bool IsGroup { get; set; }
        string Email { get; set; }
        string Salt { get; set; }
        string Hash { get; set; }
        DateTime Expire { get; set; }
        string ResetPasswordKey { get; set; }
        DateTime ResetPasswordExpire { get; set; }
        IList<string> Groups { get; set; }
        IList<string> Roles { get; set; }
        IDictionary<string, string> Tags { get; set; }
        bool IsActive { get; set; }
        string MasterGroup { get; set; }
        string Id { get; set; }
        bool IsAdmin { get; set; }
        IDictionary<string, object> Custom { get; set; }
    }


    

    [Serialize]
    public class LoginInfo : ILoginInfo {
        public LoginInfo Clone() {
            var result = (LoginInfo) this.MemberwiseClone();
            result.Roles = new List<string>();
            result.Groups = new List<string>();
            result.Tags = new Dictionary<string, string>();
            result.Custom = new Dictionary<string, object>();
            foreach (var role in this.Roles) {
                result.Roles.Add(role);
            }
            foreach (var @group in Groups) {
                result.Groups.Add(@group);
            }
            foreach (var tag in Tags) {
                result.Tags[tag.Key] = tag.Value;
            }
            JsonExtend.Extend(result.Custom, this.Custom);
            return result;
        }

        

        protected bool Equals(LoginInfo other) {
            var result = string.Equals(Login, other.Login) && Version == other.Version &&
                         string.Equals(Name, other.Name) && IsGroup.Equals(other.IsGroup) &&
                         string.Equals(Email, other.Email) && string.Equals(Salt, other.Salt) &&
                         string.Equals(Hash, other.Hash) && IsAdmin.Equals(other.IsAdmin) && Expire.Equals(other.Expire) &&
                         string.Equals(ResetPasswordKey, other.ResetPasswordKey) &&
                         ResetPasswordExpire.Equals(other.ResetPasswordExpire) && string.Equals(Id, other.Id);
            
            if (!result) return false;
            if (IsActive != other.IsActive) return false;
            if (other.Roles.Count != Roles.Count) return false;
            if (other.Tags.Count != Tags.Count) return false;
            if (other.Groups.Count != Groups.Count) return false;
            if (other.MasterGroup != this.MasterGroup) return false;
            if (Experiments.Json.Stringify(Custom) != Experiments.Json.Stringify(other.Custom)) {
                return false;
            }
            if (Roles.Any(role => !other.Roles.Contains(role))) {
                return false;
            }

            
            if (Groups.Any(role => !other.Groups.Contains(role)))
            {
                return false;
            }

            
            foreach (var tag in Tags) {
                if (!other.Tags.ContainsKey(tag.Key)) return false;
                if (!Object.Equals(other.Tags[tag.Key], tag.Value)) {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (_groups != null ? _groups.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Login != null ? Login.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Version;
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsGroup.GetHashCode();
                hashCode = (hashCode*397) ^ (Email != null ? Email.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Salt != null ? Salt.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Hash != null ? Hash.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ IsAdmin.GetHashCode();
                hashCode = (hashCode*397) ^ Expire.GetHashCode();
                hashCode = (hashCode*397) ^ (ResetPasswordKey != null ? ResetPasswordKey.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ ResetPasswordExpire.GetHashCode();
                hashCode = (hashCode*397) ^ (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ RawIsActive.GetHashCode();
                return hashCode;
            }
        }

        private IList<string> _groups;
        private IList<string> _roles;
        private IDictionary<string, string> _tags;
        private readonly ConcurrentDictionary<string, bool> _roleCache = new ConcurrentDictionary<string, bool>();

        public LoginInfo() {
            Expire = new DateTime(3000, 1, 1);
            Version = -1;
        }

        [IgnoreSerialize]
        public ILoginSourceProvider Provider { get; set; }

        public string Login { get; set; }

        [IgnoreSerialize]
        public int Version { get; set; }

        [SerializeNotNullOnly]
        public string Name { get; set; }

        [SerializeNotNullOnly]
        public bool IsGroup { get; set; }

        [SerializeNotNullOnly]
        public string Email { get; set; }

        [IgnoreSerialize]
        public string Salt { get; set; }

        [IgnoreSerialize]
        public string Hash { get; set; }

        [SerializeNotNullOnly]
        public bool IsAdmin { get; set; }

        [IgnoreSerialize]
        public bool? RawIsActive { get; set; }

        [SerializeNotNullOnly]
        public DateTime Expire { get; set; }

        [IgnoreSerialize]
        public string ResetPasswordKey { get; set; }

        [IgnoreSerialize]
        public DateTime ResetPasswordExpire { get; set; }

        [IgnoreSerialize]
        public string GroupList { get; set; }

        [IgnoreSerialize]
        public string RolesList { get; set; }

        [IgnoreSerialize]
        public string TagList { get; set; }

        [SerializeNotNullOnly]
        public IList<string> Groups {
            get { return _groups ?? (_groups = new List<string>(GroupList.SmartSplit(false, true, ',', ' ', '/', ';'))); }
            set { _groups = value; }
        }

        [SerializeNotNullOnly]
        public IList<string> Roles {
            get { return _roles ?? (_roles = new List<string>(RolesList.SmartSplit(false,true,',',' ','/',';'))); }
            set { _roles = value; }
        }

        [SerializeNotNullOnly]
        public IDictionary<string, string> Tags {
            get { return _tags ?? (_tags = TagHelper.Parse(TagList)); }
            set { _tags = value; }
        }

        [Serialize]
        public bool IsActive {
            get {
                if (RawIsActive.HasValue && !RawIsActive.Value) {
                    return false;
                }
                return true;
            }
            set { RawIsActive = value; }
        }

        [IgnoreSerialize]
        public string Password {
            set { SetPassword(value); }
        }

        [SerializeNotNullOnly]
        public string MasterGroup { get; set; }

        [IgnoreSerialize]
        public string Id { get; set; }

        [SerializeNotNullOnly]
        public IDictionary<string, object> Custom { get; set; }

        public LogonAuthenticationResult Logon(string password) {
            if (IsGroup) {
                return LogonAuthenticationResult.InvalidLoginInfo;
            }
            if (RawIsActive.HasValue && !RawIsActive.Value) {
                return LogonAuthenticationResult.Inactive;
            }
            if (Expire <= DateTime.Now) {
                return LogonAuthenticationResult.Expired;
            }
            if (string.IsNullOrWhiteSpace(Hash) || string.IsNullOrWhiteSpace(Salt)) {
                return LogonAuthenticationResult.InvalidRecord;
            }
            var hash = (Salt + password + Salt).GetMd5();
            if (hash != Hash) {
                return LogonAuthenticationResult.InvalidLoginInfo;
            }
            if (!string.IsNullOrWhiteSpace(MasterGroup)) {
                var group = Provider.Get(MasterGroup + "@groups");

                if (null != group) {
                    if (!group.IsActive) return LogonAuthenticationResult.GroupInactive;
                    if (group.Expire < DateTime.Now) {
                        return LogonAuthenticationResult.GroupExpired;
                    }
                }
                else {
                    return LogonAuthenticationResult.InvalidMasterGroup;

                }
            }
            return LogonAuthenticationResult.Ok;
        }

        public void ResetPassword(string newpassword, string validationkey) {
            CheckValidationKey(validationkey);
            SetPassword(newpassword);
            ResetPasswordKey = null;
            ResetPasswordExpire = DateTime.MinValue;
        }

        public void CheckValidationKey(string validationkey) {
            if (string.IsNullOrWhiteSpace(ResetPasswordKey)) {
                throw new Exception("password not pending to reset");
            }
            if (string.IsNullOrWhiteSpace(validationkey)) {
                throw new Exception("validation string not provided");
            }
            if (validationkey != ResetPasswordKey) {
                throw new Exception("invalid validation key");
            }
            if (ResetPasswordExpire < DateTime.Now) {
                throw new Exception("Reset password timeout expired");
            }
        }

        public void InitResetPassword(TimeSpan timespan = default (TimeSpan)) {
            if (timespan.TotalMinutes < 1) {
                timespan = TimeSpan.FromMinutes(10);
            }
            ResetPasswordKey = Guid.NewGuid().ToString().GetMd5();
            ResetPasswordExpire = DateTime.Now + timespan;
        }

        public void SetPassword(string newpassword) {
            Salt = Guid.NewGuid().ToString().GetMd5();
            var basestring = Salt + newpassword + Salt;
            Hash = basestring.GetMd5();
        }

        public bool IsInRole(string role, bool exact = false) {
            return _roleCache.GetOrAdd(role + "_" + exact, _ => {
                if (string.IsNullOrWhiteSpace(role)) {
                    return true;
                }
                role = role.ToLowerInvariant();
                if (role == "default") {
                    return true;
                }
                if (IsAdmin && (role == "admin" || !exact)) {
                    return true;
                }
                if (role == "admin") {
                    return false;
                }
                var prefixSearch = role.EndsWith("*");
                if (prefixSearch) {
                    role = role.Substring(0, role.Length - 1);
                }
                foreach (var r in Roles.Where(r => r.Length >= role.Length)) {
                    if (string.Compare(r, role, StringComparison.InvariantCultureIgnoreCase) == 0) {
                        return true;
                    }
                    if (prefixSearch && string.Compare(r, 0, role, 0, role.Length, true) == 0) {
                        return true;
                    }
                }
                if (null != Provider && 0 != Groups.Count) {
                    foreach (var grp in Groups) {
                        var grpInfo = Provider.Get(grp + "@groups");
                        if (null != grpInfo) {
                            if (grpInfo.IsInRole(role, exact)) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            });
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != this.GetType()) {
                return false;
            }
            return Equals((LoginInfo) obj);
        }

        public void Merge(LoginInfo other) {
            if (string.IsNullOrWhiteSpace(Name)) {
                Name = other.Name;
            }
            if (string.IsNullOrWhiteSpace(Email)) {
                Email = other.Email;
            }
            if (null == RawIsActive && IsActive) {
                RawIsActive = other.RawIsActive;
            }
            if (string.IsNullOrWhiteSpace(Hash)) {
                Hash = other.Hash;
                Salt = other.Salt;
            }
            if (string.IsNullOrWhiteSpace(Salt)) {
                Salt = other.Salt;
            }

            if (!IsAdmin) {
                IsAdmin = other.IsAdmin;
            }

            if (other.Expire < Expire) {
                Expire = other.Expire;
            }

            foreach (var grp in other.Groups) {
                if (!Groups.Contains(grp)) {
                    Groups.Add(grp);
                }
            }

            foreach (var role in other.Roles) {
                if (!Roles.Contains(role)) {
                    Roles.Add(role);
                }
            }

            foreach (var tag in other.Tags) {
                if (!string.IsNullOrWhiteSpace(tag.Value)) {
                    Tags[tag.Key] = tag.Value;
                }
            }

            JsonExtend.Extend(Custom, other.Custom);

        }
    }
}