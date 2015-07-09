using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Experiments;
using Qorpent.IO.Http;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.management {
    public static class UserUpdateInfoSerializer {
        /// <summary>
        ///     ¬ытаскивает сведени€ об обновлении из параметров
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static UserUpdateInfo[] ExtractFromParameters(RequestParameters parameters) {
            if (null != parameters.Json) {
                return ExtractFromJson(parameters.Json, true);
            }
            var _active = parameters.Get("active");
            var _logable = parameters.Get("logable");
            var _isgroup = parameters.Get("isgroup");
            var _admin = parameters.Get("isadmin");
            var _expire = parameters.Get("expire");
            var _custom = parameters.ReadDict("custom");
            var _roles = parameters.ReadArray("roles");
            var _groups = parameters.ReadArray("groups");
           
            var result = new[] {
                new UserUpdateInfo {
                    Login = parameters.Get("login"),
                    Name = parameters.Get("name"),
                    Email = parameters.Get("email"),
                    IsGroup = string.IsNullOrWhiteSpace(_isgroup) ? (bool?) null : _isgroup.ToBool(),
                    Active = string.IsNullOrWhiteSpace(_active) ? (bool?) null : _active.ToBool(),
                    IsAdmin = string.IsNullOrWhiteSpace(_admin) ? (bool?) null : _admin.ToBool(),
                    Logable = string.IsNullOrWhiteSpace(_logable) ? (bool?) null : _logable.ToBool(),
                    Expire = string.IsNullOrWhiteSpace(_expire) ? (DateTime?) null : _expire.ToDate(),
                    PublicKey = parameters.Get("publickey"),
                    Password = parameters.Get("password"),
                    Domain = parameters.Get("domain"),
                    Roles = _roles.Length == 0 ? null : _roles.Select(_ => _.ToStr()).ToList(),
                    Groups = _groups.Length == 0 ? null : _groups.Select(_ => _.ToStr()).ToList(),
                    Custom = _custom.Count == 0 ? null : _custom
                }
            };

            return result;
        }

        public static UserUpdateInfo[] ExtractFromJson(object json, bool alreadyJsonified = false) {
            var j = alreadyJsonified ? json : json.jsonify();
            var items = j.arr("items") ?? j;
            if (items is object[])
            {
                return ((object[])items).Select(ExtractSingle).ToArray();
            }
            return new[] {ExtractSingle(j)};
        }

        private static UserUpdateInfo ExtractSingle(object j) {
            var _active = j.str("active");
            var _admin = j.str("isadmin");
            var _expire = j.str("expire");
            var _custom = j.get("custom") as IDictionary<string, object>;
            var _roles = j.get("roles") as object[];
            var _groups = j.get("groups") as object[];
            var _isgroup = j.str("isgroup");
            var _logable = j.str("logable");
            var result = new UserUpdateInfo {
                Login = j.str("login"),
                Name = j.str("name"),
                Email = j.str("email"),
                Active = string.IsNullOrWhiteSpace(_active) ? (bool?) null : _active.ToBool(),
                IsGroup = string.IsNullOrWhiteSpace(_isgroup) ? (bool?) null : _isgroup.ToBool(),
                IsAdmin = string.IsNullOrWhiteSpace(_admin) ? (bool?) null : _admin.ToBool(),
                Logable = string.IsNullOrWhiteSpace(_logable) ? (bool?) null : _logable.ToBool(),
                Expire = string.IsNullOrWhiteSpace(_expire) ? (DateTime?) null : _expire.ToDate(),
                Password = j.str("password"),
                Domain = j.str("domain"),
                PublicKey = j.str("publickey"),
                Roles = (_roles == null || _roles.Length == 0) ? null : _roles.Select(_ => _.ToStr()).ToList(),
                Groups = (_groups == null || _groups.Length == 0) ? null : _groups.Select(_ => _.ToStr()).ToList(),
                Custom = (_custom == null || _custom.Count == 0) ? null : _custom
            };
            return result;
        }
    }
}