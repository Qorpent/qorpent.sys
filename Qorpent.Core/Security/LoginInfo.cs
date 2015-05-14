using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Security {
    [Serialize]
    public class LoginInfo {
        private IList<string> _groups;
        private IList<string> _roles;
        private IDictionary<string, string> _tags;
        private readonly ConcurrentDictionary<string, bool> _roleCache = new ConcurrentDictionary<string, bool>();

        public LoginInfo() {
            Expired = new DateTime(3000, 1, 1);
        }

        [IgnoreSerialize]
        public ILoginSourceProvider Provider { get; set; }

        public string Login { get; set; }

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

        [SerializeNotNullOnly]
        public bool? RawIsActive { get; set; }

        [SerializeNotNullOnly]
        public DateTime Expired { get; set; }

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
        }

        [IgnoreSerialize]
        public string Password {
            set { SetPassword(value); }
        }

        public LogonAuthenticationResult Logon(string password) {
            if (IsGroup) {
                return LogonAuthenticationResult.InvalidLoginInfo;
            }
            if (RawIsActive.HasValue && !RawIsActive.Value) {
                return LogonAuthenticationResult.Inactive;
            }
            if (Expired <= DateTime.Now) {
                return LogonAuthenticationResult.Expired;
            }
            if (string.IsNullOrWhiteSpace(Hash) || string.IsNullOrWhiteSpace(Salt)) {
                return LogonAuthenticationResult.InvalidRecord;
            }
            var hash = (Salt + password + Salt).GetMd5();
            if (hash != Hash) {
                return LogonAuthenticationResult.InvalidLoginInfo;
            }
            return LogonAuthenticationResult.Ok;
        }

        public void ResetPassword(string newpassword, string validationkey) {
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
            SetPassword(newpassword);
            ResetPasswordKey = null;
            ResetPasswordExpire = DateTime.MinValue;
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

            if (other.Expired < Expired) {
                Expired = other.Expired;
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
        }
    }
}