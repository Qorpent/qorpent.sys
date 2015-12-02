using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Principal;
using qorpent.Security;
using qorpent.v2.security.authorization;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace qorpent.v2.security.management {
    /// <summary>
    /// 
    /// </summary>
    [ContainerComponent(Lifestyle.Singleton,"accident.clients",ServiceType=typeof(IClientService))]
    public class ClientService :ServiceBase, IClientService {
        /// <summary>
        /// 
        /// </summary>
        public ClientService() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="roles"></param>
        /// <param name="updater"></param>
        /// <param name="manager"></param>
        public ClientService(IUserService users , IRoleResolverService roles = null, IUpdateUserProcessor updater=null, IPasswordManager manager = null) {
            Users = users;
            Roles = roles ?? (new RoleResolverService(users, new RoleResolver {Users = users}));
            Updater = updater ?? (new UpdateUserProcessor {Users = users, Roles = Roles});
            PasswordManager = manager ?? new PasswordManager();
        }
        [Inject]
        public IRoleResolverService Roles { get; set; }

        [Inject]
        public IUpdateUserProcessor Updater { get; set; }
        [Inject]
        public IUserService Users { get; set; }
        [Inject]
        public IPasswordManager PasswordManager { get; set; }

        public ClientResult Init(string userName, ClientRecord record) {
            return Init(new Identity( Users.GetUser(userName)), record);
        }
        public ClientResult Init(IIdentity caller, ClientRecord record) {
            var result = new ClientResult {OK = true};
            try {
                InternalInit(caller, record, result);
            }
            catch(Exception e) {
                result.OK = false;
                result.Error = e;
            }
            return result;
        }

        public ClientResult ToWork(IIdentity caller, string clientSysName) {
            var result = new ClientResult {OK = true};
            try {
                CheckCaller(caller);
                InternalToWork( clientSysName,result);
                if (null != result.Group) {
                    InternalSetExpire( clientSysName, DateTime.Today.AddDays(1).Add(SecurityConst.LEASE_USER), result);
                }
            }
            catch (Exception e) {
                result.OK = false;
                result.Error = e;
            }
            return result;
        }

        private void InternalToWork(string clientSysName, ClientResult result) {
            var group = Users.GetUser(clientSysName + "@groups");
            if (null == group) {
                throw new Exception("no group found");
            }
            if (!group.Roles.Contains(SecurityConst.ROLE_DEMO_ACCESS)) {
                return; //already work
            }
            group.Roles.Remove(SecurityConst.ROLE_DEMO_ACCESS);
            group.Roles.Add("writer"); //HACK: it's unlift-related role
            result.Group = group;
            Users.Store(group);
            
        }

        public ClientResult ToDemo(IIdentity caller, string clientSysName) {
            var result = new ClientResult { OK = true };
            try
            {
                CheckCaller(caller);
                InternalToDemo(clientSysName, result);
                if (null != result.Group)
                {
                    InternalSetExpire(clientSysName, DateTime.Today.AddDays(1).Add(SecurityConst.LEASE_DEMO),result);
                }
            }
            catch (Exception e)
            {
                result.OK = false;
                result.Error = e;

            }
            return result;
        }

        private void InternalToDemo( string clientSysName, ClientResult result) {
            var group = Users.GetUser(clientSysName + "@groups");
            if (null == group)
            {
                throw new Exception("no group found");
            }
            if (group.Roles.Contains(SecurityConst.ROLE_DEMO_ACCESS))
            {
                return; //already demo
            }
            group.Roles.Add(SecurityConst.ROLE_DEMO_ACCESS);
            group.Roles.Remove("writer"); //HACK: it's unlift-related role
            result.Group = group;
            Users.Store(group);
        }

        public ClientResult SetExpire(IIdentity caller, string clientSysName, DateTime newExpire) {
            var result = new ClientResult { OK = true };
            try
            {
                CheckCaller(caller);
                InternalSetExpire(clientSysName,newExpire,result);

            }
            catch (Exception e)
            {
                result.OK = false;
                result.Error = e;
            }
            return result;
        }

        private void InternalSetExpire( string clientSysName, DateTime date, ClientResult result) {
            result.Group = result.Group ?? Users.GetUser(clientSysName + "@groups");
            if (null == result.Group)
            {
                throw new Exception("no group found");
            }
            result.Group.Expire = date;
            Users.Store(result.Group);
            var users = Users.SearchUsers(new UserSearchQuery {Domain = clientSysName}).ToArray();
            foreach (var user in users) {
                if (user.Active) {
                    user.Expire = result.Group.Expire;
                    Users.Store(user);
                }
            }
        }

        private void InternalInit(IIdentity caller, ClientRecord record, ClientResult result) {
            CheckCaller(caller);
            if (string.IsNullOrWhiteSpace(record.Name))
            {
                throw new ArgumentException("no client name supplied", nameof(record.Name));
            }
            if (string.IsNullOrWhiteSpace(record.SysName))
            {
                record.SysName = Escaper.OrganizationSysName(record.Name);
            }
            if (string.IsNullOrWhiteSpace(record.SysName) ||
                record.SysName != Escaper.OrganizationSysName(record.SysName)) {
                throw new ArgumentException("invalid sysname "+record.SysName,nameof(record.SysName));
            }
            

            var groupLogin = record.SysName + "@groups";
            var existed = Users.GetUser(groupLogin);
            if (null != existed) {
                throw new SecurityException("group already exists");
            }
            var group = new User {
                Active = true,
                Login = groupLogin,
                IsGroup = true,
                Name = record.Name,
                Email = record.UserEmail,
                Roles = new[] {SecurityConst.ROLE_DEMO_ACCESS},
                Expire = DateTime.Today.AddDays(1).Add(SecurityConst.LEASE_DEMO),
                Custom = new Dictionary<string,object>{ {"contact" , record.Phone}}
            };
            Users.Store(group);

            var userLogin = "user000@" + record.SysName;
            existed = Users.GetUser(userLogin);
            if (null != existed) {
                throw new SecurityException("user already exists");
            }
            var name = string.IsNullOrWhiteSpace(record.UserName) ? record.Name : record.UserName;

            var user = new User {
                Login = userLogin,
                Name = name,
                Logable = true,
                Domain = record.SysName,
                Groups = new[] {record.SysName},
                Active = true,
                Expire = group.Expire,
                Roles = new[] { SecurityConst.ROLE_DOMAIN_ADMIN }
            };
            var pass = string.IsNullOrWhiteSpace(record.Password) ? PasswordManager.Generate() : record.Password;
            if (!PasswordManager.GetPolicy(pass).Ok) {
                throw new SecurityException("password not match policy");
            }
            
            PasswordManager.SetPassword(user,pass,true);
            Users.Store(user);

            result.GeneratedSysName = record.SysName;
            result.GeneratedPassword = pass;
            result.Group = group;
            result.User = user;
        }

        private void CheckCaller(IIdentity caller) {
            if (!Roles.IsInRole(caller, SecurityConst.ROLE_SECURITY_ADMIN)) {
                throw new SecurityException("not allowed for caller");
            }
        }
    }
}