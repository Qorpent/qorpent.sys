using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading;
using Qorpent.Events;
using Qorpent.IO.Http;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Security {
    /// <summary>
    /// Обновляет информацию о пользователе
    /// </summary>
    public class UpdateLoginInfoHandler:IRequestHandler {
        private IHostServer _server;
        private IRoleResolver _roles;
        private ILoginSourceProvider _logins;

        public UpdateLoginInfoHandler(IHostServer server) {
            _server = server;
            _roles = _server.Container.Get<IRoleResolver>();
            _logins = _server.Container.Get<ILoginSourceProvider>();
        }

        class UpdateRequest {
            private string login;
            private bool isadmin;
            private string currentuser;
            public bool forced;
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

            public UpdateRequest(WebContext context, RequestParameters data, IRoleResolver _roles, ILoginSourceProvider _logins) {
                login = data.Get("login");
                isauth = context.User.Identity.IsAuthenticated;
                currentuser = context.User.Identity.Name;
                if (string.IsNullOrWhiteSpace(login))
                {
                    login = currentuser;
                }
                isadmin = _roles.IsInRole(currentuser, "ADMIN");
                forced = data.Get("forced").ToBool();
                info = _logins.Get(login);
                if (null == info) {
                    info = new LoginInfo {Login = login};
                }
                else {
                    info = info.Clone();
                }
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
                active = string.IsNullOrWhiteSpace(_active) || _active.ToBool();
                var roles = data.Get("roles").SmartSplit(false, true, ',', '|', ' ');
                removeroles = roles.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
                addroles = roles.Where(_ => !_.StartsWith("-")).ToArray();
                var groups = data.Get("groups").SmartSplit(false, true, ',', '|', ' ');
                removegroups = groups.Where(_ => _.StartsWith("-")).Select(_ => _.Substring(1)).ToArray();
                addgroups = groups.Where(_ => !_.StartsWith("-")).ToArray();
                tags = TagHelper.Parse(data.Get("tags"));
                
            }

            public void Authorize() {
                if (initreq) {
                    if(info.Version<=0)throw new Exception("try reset password on unknown login");
                    if (string.IsNullOrWhiteSpace(email)) {
                        throw new SecurityException("email required for init req");
                    }
                    if (string.IsNullOrWhiteSpace(info.Email)) {
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
                if (!string.IsNullOrWhiteSpace(pass) && !string.IsNullOrWhiteSpace(reqkey)) {
                    if (info.Version <= 0) throw new Exception("try reset password on unknown login");
                    info.CheckValidationKey(reqkey);
                    return;
                }
                if(!isauth)throw new SecurityException("authentication required");
                if (login !=currentuser && !isadmin)
                {
                    
                        throw new SecurityException("only admins allowed to change other user's info");
                    
                }
                if (!HasDelta()) return; //empty updates allowed
                if (!isadmin) {
                    if(info.Version==-1)throw new SecurityException("only admins can register users");
                    if(forced)throw new SecurityException("forced mode allowed only for admins");
                    if(setadmin)throw new SecurityException("only admins can promote admins");
                    if(info.IsGroup || isgroup)throw new SecurityException("only admins can deal with groups");
                    if(0!=addroles.Length)throw new SecurityException("only admins can add roles");
                    if(0!=addgroups.Length)throw new SecurityException("only admins can add groups");
                    if (!string.IsNullOrWhiteSpace(email) && info.Email != email) {
                        throw new SecurityException("only admins can change emails");
                    }
                    if (active && !info.IsActive) {
                        throw new SecurityException("only admins can reactivate logins");
                    }
                    if (tags.Any(_ => _.Key.StartsWith("secure_"))) {
                        throw new SecurityException("only admins can deal with secure_ tags");
                    }
                    if (expire.Year > 1990) {
                        if (expire > info.Expire) {
                            throw new SecurityException("only admins can upgrade expiration");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(pass)) {
                        if (!string.IsNullOrWhiteSpace(oldpass)) {
                            var auth = info.Logon(oldpass);
                            if (auth != LogonAuthenticationResult.Ok) {
                                throw new SecurityException("invalid old pass given");
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(reqkey)) {
                            info.CheckValidationKey(reqkey);
                        }
                        else {
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
            public bool rolesupdated;

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
                if (!string.IsNullOrWhiteSpace(pass)) {
                    if (info.Logon(pass) != LogonAuthenticationResult.Ok) {
                        return true;
                    }
                }
                if (!string.IsNullOrWhiteSpace(name) && name != info.Name) {
                    return true;
                }
                if (setadmin && !info.IsAdmin) {
                    this.rolesupdated = true;
                    return true;
                }
                if (noadmin && info.IsAdmin) {
                    this.rolesupdated = true;
                    return true;
                }
                if (!string.IsNullOrWhiteSpace(email) && email != info.Email) {
                    return true;
                }
                if (active != info.IsActive) {
                    return true;
                }
                if (info.Groups.Any(_ => -1 != Array.IndexOf(removegroups, _))) {
                    this.rolesupdated = true;
                    return true;
                }
                if (info.Roles.Any(_ => -1 != Array.IndexOf(removeroles, _))) {
                    this.rolesupdated = true;
                    return true;
                }
                if (addgroups.Any(_ => !info.Groups.Contains(_))) {
                    this.rolesupdated = true;
                    return true;
                }
                if (addroles.Any(_ => !info.Roles.Contains(_))) {
                    this.rolesupdated = true;
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
                return false;
            }

            public void Update() {
                // change password is special update operation
                
                if (initreq) {
                    info.InitResetPassword();
                    return;
                }
                if (!string.IsNullOrWhiteSpace(pass) && !string.IsNullOrWhiteSpace(reqkey)) {
                    info.ResetPassword(pass,reqkey);
                    return;
                }



                
                if (!string.IsNullOrWhiteSpace(pass)) {
                    info.SetPassword(pass);
                }
                 
                if (!string.IsNullOrWhiteSpace(name)) {
                    info.Name = name;
                }
                if (!string.IsNullOrWhiteSpace(email)) {
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
                    
            }
        }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var data = RequestParameters.Create(context);
            var request = new UpdateRequest(context,data,_roles,_logins);
            request.Authorize();
            if (!request.HasDelta()) {
                context.Finish("\"no updates required\"",status:201);
            };
            request.Update();
            if (request.rolesupdated) {
                ((DefaultAuthenticationProvider) _server.Auth).Reset(new ResetEventData(true));
            }
            var info = request.GetInfo();
            _logins.Save(info,request.forced);
            context.Finish("\"updated\"");
        }

        
    }
}