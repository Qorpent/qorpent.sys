using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text.RegularExpressions;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent.Experiments;
using Qorpent.IoC;

namespace qorpent.v2.security.management {
    [ContainerComponent(Lifestyle.Singleton, "updateuser.checjer", ServiceType = typeof(IUpdateUserChecker))]
    public class UpdateUserChecker : IUpdateUserChecker {
        [Inject]
        public IRoleResolverService Roles { get; set; }
        [Inject]
        public IUserService Users { get; set; }

        public UpdateResult ValidateUpdate(IIdentity actor, UserUpdateInfo update, IUser target) {
            var validateUpdate = CheckCommons(actor,update, target);
            if (validateUpdate != null) return validateUpdate;
            if (null == target || 0==target.Version) {
                return CheckCreation(actor, update);
            }
            else {
                if (actor.Name == update.Login) {
                    return CheckSelfUpdate(actor,update, target);
                }
                return CheckOtherUpdate(actor, update, target);
            }
        }

        private UpdateResult CheckSelfUpdate(IIdentity actor,UserUpdateInfo update, IUser target) {
          
            if (update.ChangedDomain(target)) {
                return new UpdateResult {IsError = true, ErrorMessage = "cannot change domain"};
            }
            if (Roles.IsInRole(actor, "SECURE_" + target.Domain + "_ADMIN")) {

                if (update.ChangedActive(target) && !target.Active)
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot reactivate yourself" };
                }
                if (update.ChangedLogable(target) && !target.Logable)
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot reactivate logability" };
                }

                return new UpdateResult {Ok = true};
            }
            
            if (update.ChangedEmail(target))
            {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot change email" };
            }
            if (update.ChangedRoles(target)) {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot change roles" };
            }
            if (update.ChangedGroups(target))
            {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot change groups" };
            }
            if (update.ChangedExpire(target)) {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot change expire" };
            }
            return new UpdateResult { Ok = true };
        }

        private UpdateResult CheckCommons(IIdentity actor, UserUpdateInfo update, IUser target) {
            if (null == Roles) {
                throw new Exception("cannot work without roles");
            }
            if (null == Users)
            {
                throw new Exception("cannot work without users");
            }
            var id = actor as Identity;
            var u = target as User;
            if (null != u) {
                if (null != u.UserSource) {
                    var ws = u.UserSource as IWriteableUserSource;
                    if (null == ws || !ws.WriteUsersEnabled) {
                        return new UpdateResult {IsError = true, ErrorMessage = "no storage"};
                    }
                }
            }
            if (null == id) {
                return new UpdateResult {IsError = true, ErrorMessage = "no actor"};
            }
            if (!id.IsAuthenticated) {
                return new UpdateResult {IsError = true, ErrorMessage = "not auth"};
            }

            if (id.IsAdmin) {
                return new UpdateResult {Ok = true};
            }

            if (null != update.IsAdmin && update.IsAdmin.Value) {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot set admin" };
            }

            if (update.ChangedRoles(target)) {
                if (update.Roles.Any(_ => _.ToUpper().Contains("SECURE_"))) {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot manage secure roles" };
                }
            }
            if (update.ChangedGroups(target))
            {
                if (update.Groups.Any(_ => _.ToUpper().Contains("SECURE_")))
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot manage secure groups" };
                }
            }

            if (update.ChangedCustom(target))
            {
                if (update.Custom.stringify().ToUpper().Contains("SECURE_"))
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot manage secure customs" };
                }
            }

            if (null != update.IsGroup)
            {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot manage groups" };
            }

            if (!string.IsNullOrWhiteSpace(update.Email) && !Regex.IsMatch(update.Email, @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$"))
            {
                return new UpdateResult { IsError = true, ErrorMessage = "mailformed email" };
            }

            return null;
        }

        private UpdateResult CheckOtherUpdate(IIdentity actor, UserUpdateInfo update, IUser target) {
            if (update.ChangedDomain(target))
            {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot change domain" };
            }
            if (Roles.IsInRole(actor, "SECURE_" + target.Domain + "_ADMIN"))
            {

                if (update.ChangedActive(target) && !target.Active)
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot reactivate yourself" };
                }
                if (update.ChangedLogable(target) && !target.Logable)
                {
                    return new UpdateResult { IsError = true, ErrorMessage = "cannot reactivate logability" };
                }

                return new UpdateResult { Ok = true };
            }
            return new UpdateResult {
                IsError = true,
                ErrorMessage = "only sys and domain admins can change other's profile"
            };
        }

        private UpdateResult CheckCreation(IIdentity actor, UserUpdateInfo update) {
            var id = actor as Identity;
            var usr = id.User ?? Users.GetUser(actor.Name);
            if (null == usr)
                return new UpdateResult {IsError = true, ErrorMessage = "cannot check not registered actor"};
            if (string.IsNullOrWhiteSpace(usr.Domain)) {
                return new UpdateResult { IsError = true, ErrorMessage = "only admins can create becide domain" };
            }
            if (!Roles.IsInRole(actor, "secure_" + usr.Domain + "_admin")) {
                return new UpdateResult { IsError = true, ErrorMessage = "not domain administrator" };
            }
            if (update.Domain != usr.Domain) {
                return new UpdateResult { IsError = true, ErrorMessage = "only in-domain creation allowed" };
            }
            if (!string.IsNullOrWhiteSpace(update.Password)) {
                return new UpdateResult { IsError = true, ErrorMessage = "cannot set password for new user" };
            }
            if (string.IsNullOrWhiteSpace(update.Email)) {
                return new UpdateResult { IsError = true, ErrorMessage = "email required for new users" };
            }
            
            return new UpdateResult {Ok = true};
        }
    }
}