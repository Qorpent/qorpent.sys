using System;
using System.Collections.Generic;
using System.Linq;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using Qorpent.Experiments;

namespace qorpent.v2.security.management {
    public class UserUpdateInfo {
        private string[] _addGroups;
        private string[] _addRoles;
        private string[] _removeGroups;
        private string[] _removeRoles;
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool? Active { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsGroup { get; set; }
        public bool? Logable { get; set; }
        public DateTime? Expire { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string PublicKey { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> Groups { get; set; }
        public object Custom { get; set; }

        /// <summary>
        /// </summary>
        public string[] AddRoles {
            get {
                if (_addRoles == null) {
                    if (null != Roles) {
                        _addRoles = Roles.Where(_ => !_.StartsWith("-")).ToArray();
                    }
                    else {
                        _addRoles = new string[] {};
                    }
                }
                return _addRoles;
            }
            set { _addRoles = value; }
        }

        /// <summary>
        /// </summary>
        public string[] RemoveRoles {
            get {
                if (_removeRoles == null) {
                    if (null != Roles) {
                        _removeRoles = Roles.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
                    }
                    else {
                        _removeRoles = new string[] {};
                    }
                }
                return _removeRoles;
            }
            set { _removeRoles = value; }
        }

        /// <summary>
        /// </summary>
        public string[] AddGroups {
            get {
                if (_addGroups == null) {
                    if (null != Groups) {
                        _addGroups = Groups.Where(_ => !_.StartsWith("-")).ToArray();
                    }
                    else {
                        _addGroups = new string[] {};
                    }
                }
                return _addGroups;
            }
            set { _addGroups = value; }
        }

        /// <summary>
        /// </summary>
        public string[] RemoveGroups {
            get {
                if (_removeGroups == null) {
                    if (null != Groups) {
                        _removeGroups = Groups.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
                    }
                    else {
                        _removeGroups = new string[] {};
                    }
                }
                return _removeGroups;
            }
            set { _removeGroups = value; }
        }

        public bool HasDelta(IUser targetUser) {
            if (null == targetUser) {
                return true;
            }
            if (ChangedName(targetUser)) {
                return true;
            }
            if (ChangedPublicKey(targetUser))
            {
                return true;
            }
            if (ChangedEmail(targetUser)) {
                return true;
            }
            if (ChangedActive(targetUser)) {
                return true;
            }
            if (ChangedIsAdmin(targetUser)) {
                return true;
            }
            if (ChangedLogable(targetUser)) {
                return true;
            }
            if (ChangedIsGroup(targetUser)) {
                return true;
            }
            if (ChangedExpire(targetUser)) {
                return true;
            }
            if (ChangedDomain(targetUser)) {
                return true;
            }
            if (ChangedRoles(targetUser)) {
                return true;
            }
            if (ChangedGroups(targetUser)) {
                return true;
            }
            if (ChangedPassword(targetUser)) {
                return true;
            }
            if (ChangedCustom(targetUser)) {
                return true;
            }
            return false;
        }

        public bool ChangedPublicKey(IUser targetUser) {
            if (!string.IsNullOrWhiteSpace(PublicKey))
            {
                if (PublicKey != targetUser.PublicKey)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsExpireIncreased(IUser targetUser) {
            if (!ChangedExpire(targetUser)) {
                return false;
            }
            return Expire.Value.ToUniversalTime() > targetUser.Expire.ToUniversalTime();
        }

        public bool ChangedCustom(IUser targetUser) {
            if (null != Custom) {
                if (Custom.stringify() != targetUser.Custom.stringify()) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedPassword(IUser targetUser) {
            if (!string.IsNullOrWhiteSpace(Password)) {
                if (string.IsNullOrWhiteSpace(targetUser.Salt)) {
                    return true;
                }
                if (string.IsNullOrWhiteSpace(targetUser.Hash)) {
                    return true;
                }
                var pwd = new PasswordManager();
                if (!pwd.MatchPassword(targetUser, Password)) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedGroups(IUser targetUser) {
            if (0 != AddGroups.Length) {
                if (!AddGroups.All(_ => targetUser.Groups.Contains(_))) {
                    return true;
                }
            }
            if (0 != RemoveGroups.Length) {
                if (RemoveGroups.Any(_ => targetUser.Groups.Contains(_))) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedRoles(IUser targetUser) {
            if (0 != AddRoles.Length) {
                if (!AddRoles.All(_ => targetUser.Roles.Contains(_))) {
                    return true;
                }
            }
            if (0 != RemoveRoles.Length) {
                if (RemoveRoles.Any(_ => targetUser.Roles.Contains(_))) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedDomain(IUser targetUser) {
            if (!string.IsNullOrWhiteSpace(Domain)) {
                if (Domain != targetUser.Domain) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedExpire(IUser targetUser) {
            if (null != Expire) {
                if (Expire.Value.ToUniversalTime() != targetUser.Expire.ToUniversalTime()) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedIsGroup(IUser targetUser) {
            if (null != IsGroup) {
                if (IsGroup.Value != targetUser.IsGroup) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedIsAdmin(IUser targetUser) {
            if (null != IsAdmin) {
                if (IsAdmin.Value != targetUser.IsAdmin) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedLogable(IUser targetUser) {
            if (null != Logable) {
                if (Logable.Value != targetUser.Logable) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedActive(IUser targetUser) {
            if (null != Active) {
                if (Active.Value != targetUser.Active) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedEmail(IUser targetUser) {
            if (!string.IsNullOrWhiteSpace(Email)) {
                if (Email != targetUser.Email) {
                    return true;
                }
            }
            return false;
        }

        public bool ChangedName(IUser targetUser) {
            if (!string.IsNullOrWhiteSpace(Name)) {
                if (Name != targetUser.Name) {
                    return true;
                }
            }
            return false;
        }

        public void Apply(IUser targetUser) {
            if (ChangedActive(targetUser)) {
                targetUser.Active = Active.Value;
            }
            if (ChangedLogable(targetUser)) {
                targetUser.Logable = Logable.Value;
            }
            if (ChangedCustom(targetUser)) {
                var target = targetUser.Custom ?? new Dictionary<string, object>();
                JsonExtend.Extend(target, Custom);
            }
            if (ChangedEmail(targetUser)) {
                targetUser.Email = Email;
            }
            if (ChangedPublicKey(targetUser)) {
                targetUser.PublicKey = PublicKey;
            }
            if (ChangedExpire(targetUser)) {
                targetUser.Expire = Expire.Value.ToUniversalTime();
            }
            if (ChangedGroups(targetUser)) {
                foreach (var item in AddGroups) {
                    targetUser.Groups.Remove(item);
                    targetUser.Groups.Add(item);
                }
                foreach (var item in RemoveGroups) {
                    targetUser.Groups.Remove(item);
                }
            }
            if (ChangedRoles(targetUser)) {
                foreach (var item in AddRoles) {
                    targetUser.Roles.Remove(item);
                    targetUser.Roles.Add(item);
                }
                foreach (var item in RemoveRoles) {
                    targetUser.Roles.Remove(item);
                }
            }
            if (ChangedIsAdmin(targetUser)) {
                targetUser.IsAdmin = IsAdmin.Value;
            }

            if (ChangedIsGroup(targetUser)) {
                targetUser.IsGroup = IsGroup.Value;
            }

            if (ChangedDomain(targetUser)) {
                targetUser.Domain = Domain;
            }
            if (ChangedPassword(targetUser)) {
                var pwd = new PasswordManager();
                pwd.SetPassword(targetUser, Password, true);
            }

            if (ChangedName(targetUser)) {
                targetUser.Name = Name;
                ;
            }
        }
    }
}