using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using Qorpent.Experiments;
using Qorpent.IO.Http;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Security {
    public class UpdateLoginInfoRequest {
        private string login;
        private bool isadmin;
        private string currentuser;
        public bool Forced { get; private set; }
        private LoginInfo info;
        private string name;
        private string email;
        private string pass;
        private bool initreq;
        private string reqkey;
        private bool active;
        private DateTime expire;
        private string[] removeroles;
        private string[] addroles;
        private string oldpass  ;
        private string[] removegroups;
        private string[] addgroups;
        private bool setadmin;
        private bool noadmin;
        private IDictionary<string, string> tags;
        private bool isgroup;
        private IDictionary<string, object> custom;
        private string mastergroup;


        public static UpdateLoginInfoRequest Create(IScope scope) {
            return new UpdateLoginInfoRequest(scope);
        }

        public static UpdateLoginInfoRequest Create(WebContext context, RequestParameters data, IRoleResolver _roles,
            ILoginSourceProvider _logins) {
            return new UpdateLoginInfoRequest(context,data,_roles,_logins);
        }

        private UpdateLoginInfoRequest(WebContext context, RequestParameters data, IRoleResolver _roles, ILoginSourceProvider _logins):this(
            _roles,_logins,context.User
            ) {
               
            LoadFields(data);
            Initialize();
      
        }

        private void Initialize() {
            isauth = this._principal.Identity.IsAuthenticated;
            currentuser = this._principal.Identity.Name;
            if (String.IsNullOrWhiteSpace(login)) {
                login = currentuser;
            }
            isadmin = this._roles.IsInRole(currentuser, "ADMIN");


            info = this._logins.Get(login);
            if (null == info) {
                info = new LoginInfo {Login = login};
            }
            else {
                info = info.Clone();
            }
            currentinfo = info;
            if (login != currentuser) {
                currentinfo = this._logins.Get(currentuser);
            }
            allowgroup = "";
            if (!String.IsNullOrWhiteSpace(currentinfo.MasterGroup))
            {
                if (this._roles.IsInRole(currentuser, "secure_" + currentinfo.MasterGroup + "_admin"))
                {
                    allowgroup = currentinfo.MasterGroup;
                }
            }
        }

        private void LoadFields(RequestParameters data) {
            login = data.Get("login");
            Forced = data.Get("forced").ToBool();
            name = data.Get("name");
            email = data.Get("email");
            pass = data.Get("pass");
            oldpass = data.Get("oldpass");
            expire = data.Get("expire").ToDate();
            initreq = data.Get("initreq").ToBool();
            noadmin = data.Get("noadmin").ToBool();
            setadmin = data.Get("setadmin").ToBool() && !noadmin;
            reqkey = data.Get("reqkey");
            isgroup = data.Get("isgroup").ToBool() || login.EndsWith("@groups");
            var _active = data.Get("active");
            active = String.IsNullOrWhiteSpace(_active) || _active.ToBool();
            var roles = data.ReadArray("roles").Select(_ => CoreExtensions.ToStr(_)).ToArray();
            removeroles = roles.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
            addroles = roles.Where(_ => !_.StartsWith("-")).ToArray();
            var groups = data.ReadArray("groups").Select(_ => _.ToStr()).ToArray();
            removegroups = groups.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
            addgroups = groups.Where(_ => !_.StartsWith("-")).ToArray();
            tags = data.ReadDict("tags").ToDictionary(_ => _.Key, _ => _.Value.ToStr());
            custom = data.ReadDict("custom");
            mastergroup = data.Get("mastergroup");
        }

        public void Authorize() {
            if (initreq) {
                if(info.Version<=0)throw new Exception("try reset password on unknown login");
                if (String.IsNullOrWhiteSpace(email)) {
                    throw new SecurityException("email required for init req");
                }
                if (String.IsNullOrWhiteSpace(info.Email)) {
                    throw new SecurityException("verification email not provided");
                }
                if (email != info.Email) {
                    throw new SecurityException("no email found");
                }
                if ((info.ResetPasswordExpire - DateTime.Now).TotalMinutes > 7) {
                    throw new SecurityException("too frequent requests, wait for 3 minutes");
                }
                return; // even not authenticated context can request change password in this context
            }
            if (!String.IsNullOrWhiteSpace(pass) && !String.IsNullOrWhiteSpace(reqkey)) {
                if (info.Version <= 0) throw new Exception("try reset password on unknown login");
                info.CheckValidationKey(reqkey);
                return;
            }
            if(!isauth)throw new SecurityException("authentication required");
            bool islocaladmin = false;
            if (login !=currentuser && !isadmin)
            {
                if (!String.IsNullOrWhiteSpace(allowgroup)) {
                    if (!login.EndsWith("@" + info.MasterGroup))
                    {
                        throw new SecurityException("only @ names allowed for non-admin changes in mastergroups");
                    }
                    if (info.Version <= 0) {
                        if (mastergroup != allowgroup) {
                            throw new SecurityException("invalid master group");
                        }
                    }
                    else {
                        if (String.IsNullOrWhiteSpace(mastergroup)) {
                            if (mastergroup != info.MasterGroup) {
                                throw new SecurityException("cannot change mastergroup");
                            }
                        }
                        else {
                            if (info.MasterGroup != allowgroup) {
                                throw new SecurityException("not your user");
                            }
                        }
                    }
                    islocaladmin = true;
                }
                else {
                    throw new SecurityException("only admins allowed to change other user's info");    
                }
                    

            }
            if (!HasDelta()) return; //empty updates allowed
            if (!isadmin) {
                if (!String.IsNullOrWhiteSpace(mastergroup) && mastergroup != allowgroup) {
                    throw new SecurityException("cannot set mastergroup");
                }
                if (Forced) throw new SecurityException("forced mode allowed only for admins");
                if (setadmin) throw new SecurityException("only admins can promote admins");
                if (info.IsGroup || isgroup) throw new SecurityException("only admins can deal with groups");
                if (null != custom && 0 != custom.Count)
                {
                    var j = Experiments.Json.Stringify(custom);
                    if (j.Contains("secure_"))
                    {
                        throw new SecurityException("only admins can deal with secure_ customs");
                    }
                }
                if (tags.Any(_ => _.Key.StartsWith("secure_")))
                {
                    throw new SecurityException("only admins can deal with secure_ tags");
                }
                if (addroles.Any(_ => _.StartsWith("secure_")))
                {
                    throw new SecurityException("only admins can add secure roles");
                }
                if (addgroups.Any(_ => _.StartsWith("secure_")))
                {
                    throw new SecurityException("only admins can add secure groups");
                }
                if (!islocaladmin) {
                    if (info.Version == -1) throw new SecurityException("only admins can register users");
                    if (0 != addroles.Length) throw new SecurityException("only admins can add roles");
                    if (0 != addgroups.Length) throw new SecurityException("only admins can add groups");
                       
                    if (!String.IsNullOrWhiteSpace(email) && info.Email != email) {
                        throw new SecurityException("only admins can change emails");
                    }
                    if (active && !info.IsActive) {
                        throw new SecurityException("only admins can reactivate logins");
                    }
                       
                    if (expire.Year > 1990) {
                        if (expire > info.Expire) {
                            throw new SecurityException("only admins can upgrade expiration");
                        }
                    }
                        
                }

                if (!String.IsNullOrWhiteSpace(pass))
                {
                    if (!String.IsNullOrWhiteSpace(oldpass))
                    {
                        var auth = info.Logon(oldpass);
                        if (auth != LogonAuthenticationResult.Ok)
                        {
                            throw new SecurityException("invalid old pass given");
                        }
                    }
                    else if (!String.IsNullOrWhiteSpace(reqkey))
                    {
                        info.CheckValidationKey(reqkey);
                    }
                    else
                    {
                        throw new SecurityException("only admins can directly setup password");
                    }

                }

            }
        }

        public LoginInfo GetInfo() {
    
            return info;
        }

        private bool? _hasdelta;
        private bool isauth;
        public bool resetrequired;
        private LoginInfo currentinfo;
        private IRoleResolver _roles;
        private string allowgroup;
        private ILoginSourceProvider _logins;
        private IPrincipal _principal;

        private UpdateLoginInfoRequest(IRoleResolver roles, ILoginSourceProvider logins, IPrincipal user) {
            this._roles = _roles;
            this._logins = _logins;
            this._principal = user;
        }

        private UpdateLoginInfoRequest(IScope context) {
            LoadFields(context);
            Initialize();
        }

        private void LoadFields(IScope data) {
            _roles = data.Get<IRoleResolver>("roles");
            _logins = data.Get<ILoginSourceProvider>("logins");
            _principal = data.Get<IPrincipal>("principal");
            login = data.Get("login","");
            Forced = data.Get("forced",false).ToBool();
            name = data.Get("name","");
            email = data.Get("email","");
            pass = data.Get("pass","");
            oldpass = data.Get("oldpass","'");
            expire = data.Get("expire").ToDate();
            initreq = data.Get("initreq").ToBool();
            noadmin = data.Get("noadmin").ToBool();
            setadmin = data.Get("setadmin").ToBool() && !noadmin;
            reqkey = data.Get("reqkey","");
            isgroup = data.Get("isgroup").ToBool() || login.EndsWith("@groups");
            var _active = data.Get("active","");
            active = String.IsNullOrWhiteSpace(_active) || _active.ToBool();
            var roles = data.Get("roles",new String[]{}).Select(_ => _.ToStr()).ToArray();
            removeroles = roles.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
            addroles = roles.Where(_ => !_.StartsWith("-")).ToArray();
            var groups = data.Get("groups", new String[] { }).Select(_ => _.ToStr()).ToArray();
            removegroups = groups.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
            addgroups = groups.Where(_ => !_.StartsWith("-")).ToArray();
            tags = data.Get("tags") as IDictionary<string,string>;
            var c = data.Get<object>("custom");
            if (null != c) {
                custom = c.jsonify() as IDictionary<string, object>;
            }
            mastergroup = data.Get("mastergroup","");
        }

        public bool HasDelta() {
            if (null == _hasdelta) {
                _hasdelta = InternalGetHasDelta();
            }
            return _hasdelta.Value;
        }

        private bool InternalGetHasDelta() {
            if (initreq) {
                return true;
            }
            if (!String.IsNullOrWhiteSpace(pass)) {
                if (info.Logon(pass) != LogonAuthenticationResult.Ok) {
                    return true;
                }
            }
            if (!String.IsNullOrWhiteSpace(name) && name != info.Name) {
                return true;
            }
            if (setadmin && !info.IsAdmin) {
                this.resetrequired = true;
                return true;
            }
            if (noadmin && info.IsAdmin) {
                this.resetrequired = true;
                return true;
            }
            if (!String.IsNullOrWhiteSpace(mastergroup) && mastergroup != info.MasterGroup)
            {
                return true;
            }
            if (!String.IsNullOrWhiteSpace(email) && email != info.Email) {
                return true;
            }
            if (active != info.IsActive) {
                return true;
            }
            if (info.Groups.Any(_ => -1 != Array.IndexOf(removegroups, _))) {
                this.resetrequired = true;
                return true;
            }
            if (info.Roles.Any(_ => -1 != Array.IndexOf(removeroles, _))) {
                this.resetrequired = true;
                return true;
            }
            if (addgroups.Any(_ => !info.Groups.Contains(_))) {
                this.resetrequired = true;
                return true;
            }
            if (addroles.Any(_ => !info.Roles.Contains(_))) {
                this.resetrequired = true;
                return true;
            }
            foreach (var tag in tags) {
                if (tag.Value == "_remove_") {
                    if (info.Tags.ContainsKey(tag.Key)) {
                        return true;
                    }
                }
                else {
                    if (!info.Tags.ContainsKey(tag.Key)) return true;
                    if (info.Tags[tag.Key] != tag.Value) return true;
                }
            }

            if (custom != null && custom.Count != 0) {
                if (info.Custom == null || info.Custom.Count == 0) return true;
                var basis = Experiments.Json.Stringify(info.Custom);
                var restored = Experiments.Json.Parse(basis);
                JsonExtend.Extend(restored,custom);
                var newj = Experiments.Json.Stringify(restored);
                if (basis != newj) {
                    resetrequired = true;
                    return true;
                }
            }

            return false;
        }

        public void Update() {
            // change password is special update operation
                
            if (initreq) {
                info.InitResetPassword();
                return;
            }
            if (!String.IsNullOrWhiteSpace(pass) && !String.IsNullOrWhiteSpace(reqkey)) {
                info.ResetPassword(pass,reqkey);
                return;
            }



                
            if (!String.IsNullOrWhiteSpace(pass)) {
                info.SetPassword(pass);
            }
                 
            if (!String.IsNullOrWhiteSpace(name)) {
                info.Name = name;
            }
            if (!String.IsNullOrWhiteSpace(email)) {
                info.Email = email;
            }
            if (active != info.IsActive) {
                info.RawIsActive = active;
            }
            if (expire.Year > 1990) {
                info.Expire = expire;
            }
            foreach (var removerole in removeroles) {
                info.Roles.Remove(removerole);
            }
            foreach (var addrole in addroles) {
                info.Roles.Remove(addrole);
                info.Roles.Add(addrole);
            }
            foreach (var removegroup in removegroups) {
                info.Groups.Remove(removegroup);
            }
            foreach (var addgroup in addgroups)
            {
                info.Groups.Remove(addgroup);
                info.Groups.Add(addgroup);
            }
            foreach (var tag in tags) {
                if (tag.Value == "_remove_") {
                    info.Tags.Remove(tag.Key);
                }
                else {
                    info.Tags[tag.Key] = tag.Value;
                }
            }
            if (setadmin) {
                info.IsAdmin = true;
            }
            if (noadmin) {
                info.IsAdmin = false;
            }

            if (null != custom && 0 != custom.Count) {
                info.Custom = info.Custom ?? new Dictionary<string, object>();
                JsonExtend.Extend(info.Custom, custom);
            }
        }
    }
}