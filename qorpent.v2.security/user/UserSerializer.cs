using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user {
    /// <summary>
    ///     Extensions for user serialization
    /// </summary>
    public static class UserSerializer {
        public static IUser CreateFromJson(object jsonsrc, bool alreadyJsonified = false) {
            var j = alreadyJsonified ? jsonsrc : jsonsrc.jsonify();
            var jtype = j.str("class") ?? j.str("_type") ?? "user";
            if (jtype != "user") {
                return null;
            }
            var netcls = j.str("netclass") ?? (typeof (User).FullName + ", " + typeof (User).Assembly.GetName().Name);
            var type = Type.GetType(netcls);
            if (null == type) {
                throw new Exception("invalid type " + netcls);
            }
            var instance = Activator.CreateInstance(type) as IUser;
            if (null == instance) {
                throw new Exception("invalid class not IUser " + netcls);
            }
            ReadJson(instance, j, true);
            return instance;
        }

        public static void ReadJson(IUser user, object jsonsrc, bool alreadyJsonified = false) {
            if (jsonsrc is XElement) {
                ReadXml(user, (XElement) jsonsrc);
            }
            var j = alreadyJsonified ? jsonsrc : jsonsrc.jsonify();
            j = j.map("_source") ?? j;
            user.Id = j.str("_id");
            user.Version = j.num("_version");
            user.CreateTime = j.date("createtime");
            user.UpdateTime = j.date("updatetime");

            user.Login = j.str("login");
            user.Name = j.str("name");
            user.Email = j.str("email");
            user.IsAdmin = j.bul("admin");
            user.IsGroup = j.bul("isgroup");

            user.Active = j.bul("active");
            user.Expire = j.date("expire");

            user.Hash = j.str("hash");
            user.Salt = j.str("salt");
            user.ResetKey = j.str("resetkey");
            user.ResetExpire = j.date("resetexpire");
            user.PublicKey = j.str("publickey");
            user.Logable = j.bul("logable");


            user.Domain = j.str("domain");
            user.Roles = j.arr("roles").OfType<string>().ToList();
            user.Groups = j.arr("groups").OfType<string>().ToList();
            user.Custom = j.map("custom");
            var extensions = user as IJsonSerializationExtension;
            if (null != extensions) {
                extensions.ReadExtensions(j);
            }
        }

        public static void ReadXml(IUser user, XElement element) {
            user.Login = element.Attr("code").ToLowerInvariant();
            user.Name = element.Attr("name");
            user.IsAdmin = element.Attr("isadmin").ToBool();
            user.Hash = element.Attr("hash");
            user.Salt = element.Attr("salt");
            user.PublicKey = element.Attr("publickey");
            user.Active = element.Attr("active", "1").ToBool();
            user.Email = element.Attr("email");
            user.Expire = element.Attr("expire").ToDate();
            user.Domain = element.Attr("domain");
            user.Roles = element.Elements("role").Select(_ => _.Attr("code")).ToArray();
            user.Groups = element.Elements("group").Select(_ => _.Attr("code")).ToArray();
            user.Logable = element.Attr("logable").ToBool();
            user.Active = element.Attr("active").ToBool();
            user.IsGroup = element.Attr("isgroup").ToBool();
            user.CreateTime = element.Attr("createtime").ToDate();
            user.UpdateTime = element.Attr("updatetime").ToDate();
            var custom = element.Element("custom");

            if (null != custom) {
                user.Custom = (IDictionary<string, object>) custom.jsonify();
            }
        }

        public static string GetJson(IUser user, string usermode = "") {
            var sw = new StringWriter();
            WriteJson(user, sw, usermode);
            return sw.ToString();
        }

        public static string GetId(IUser user) {
            if (!string.IsNullOrWhiteSpace(user.Id)) {
                return user.Id;
            }
            return GetId(user.Login);
        }

        public static string GetId(string login) {
            if (string.IsNullOrWhiteSpace(login)) {
                return "0";
            }
            return
                login.ToLowerInvariant()
                    .Replace("/", "_rs_")
                    .Replace("\\", "_ls_")
                    .Replace("@", "_at_")
                    .Replace(".", "_")
                    .Replace("-", "_");
        }

        public static void WriteJson(IUser user, TextWriter output, string usermode = "") {
            if (string.IsNullOrWhiteSpace(usermode)) {
                usermode = "admin";
            }
            var notnullonly = usermode != "store";
            var jw = new JsonWriter(output);
            if (null == user) {
                jw.WriteValue(null);
                return;
            }
            jw.OpenObject();
            jw.WriteProperty("_id", user.Id, notnullonly);
            jw.WriteProperty("_version", user.Version, notnullonly);
            jw.WriteProperty("createtime", user.CreateTime.ToUniversalTime(), notnullonly);
            jw.WriteProperty("updatetime", user.UpdateTime.ToUniversalTime(), notnullonly);

            if (usermode == "admin" || usermode == "store") {
                jw.WriteProperty("class", "user");
                jw.WriteProperty("_type", "user");
                var type = user.GetType();
                jw.WriteProperty("netclass", type.FullName + ", " + type.Assembly.GetName().Name);
            }


            jw.WriteProperty("login", user.Login, notnullonly);
            jw.WriteProperty("name", user.Name, notnullonly);
            jw.WriteProperty("email", user.Email, notnullonly);
            jw.WriteProperty("admin", user.IsAdmin, notnullonly);
            jw.WriteProperty("isgroup", user.IsGroup, notnullonly);
            jw.WriteProperty("active", user.Active, notnullonly);
            jw.WriteProperty("expire", user.Expire.ToUniversalTime(), notnullonly);
            jw.WriteProperty("publickey", user.PublicKey, notnullonly);

            if (usermode == "store" || usermode == "admin") {
                
                jw.WriteProperty("hash", user.Hash, notnullonly);
                jw.WriteProperty("salt", user.Salt, notnullonly);
                jw.WriteProperty("resetkey", user.ResetKey, notnullonly);
                jw.WriteProperty("resetexpire", user.ResetExpire.ToUniversalTime(), notnullonly);
                
                jw.WriteProperty("logable", user.Logable, notnullonly);
            }
            jw.WriteProperty("domain", user.Domain, notnullonly);
            jw.WriteProperty("roles", user.Roles.OrderBy(_ => _).ToArray(), notnullonly);
            jw.WriteProperty("groups", user.Groups.OrderBy(_ => _).ToArray(), notnullonly);
            jw.WriteProperty("custom", user.Custom, notnullonly);

            var extensions = user as IJsonSerializationExtension;
            if (null != extensions) {
                extensions.WriteExtensions(jw, usermode, null);
            }

            jw.CloseObject();
        }
    }
}