using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Xml.Linq;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.logon.providers {
    [ContainerComponent(Lifestyle.Transient, "winlogon.passlogon", ServiceType = typeof (ILogonProvider))]
    public class WinLogon : InitializeAbleService, ILogonProvider, IPasswordLogon {
        public const int WINLOGONIDX = 10000;

        public WinLogon() {
            Idx = WINLOGONIDX;
        }

        [Inject]
        public IUserService UserService { get; set; }

        public bool GoodDog { get; set; }
        public int Idx { get; set; }

        public object Reset(ResetEventData data) {
            return null;
        }

        public object GetPreResetInfo() {
            return null;
        }

        public IIdentity Logon(string username, string password, IScope scope = null) {
            lock (this) {
                var token = new IntPtr();
                var auth = Logon(username, password, ref token, 3);
                if (!auth) {
                    return null;
                }
                var native = new WindowsIdentity(token);
                var isadmin = false;
                if (GoodDog) {
                    var currentUser = Environment.UserName;
                    if (currentUser.Equals(username, StringComparison.InvariantCultureIgnoreCase)) {
                        isadmin = true;
                    }
                }
                var result = new Identity {
                    Name = native.Name.ToLowerInvariant(),
                    IsAuthenticated = true,
                    AuthenticationType = "win",
                    Native = native,
                    IsAdmin = isadmin
                };
                if (null != UserService) {
                    result.User = UserService.GetUser(result.Name);
                }
                return result;
            }
        }

        /// <summary>
        ///     Execute system logon procedure and return true if proceed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="logontype"></param>
        /// <returns></returns>
        public bool Logon(string username, string password, int logontype = WinLogonType.Logon32LogonNetwork) {
            lock (this) {
                var token = new IntPtr();
                return Logon(username, password, ref token, logontype);
            }
        }

        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);
            if (null != e) {
                e = e.Element("logon");
            }
            if (null != e) {
                GoodDog = e.Attr("gooddog").ToBool();
            }
        }

        [DllImport("ADVAPI32.dll", EntryPoint =
            "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUserW(string lpszUsername, string lpszDomain,
            string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        ///     Execute system logon procedure and return true if proceed
        ///     can return system token of logon (windows only)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="token"></param>
        /// <param name="logontype"></param>
        /// <returns></returns>
        public bool Logon(string username, string password, ref IntPtr token,
            int logontype = WinLogonType.Logon32LogonNetwork) {
            lock (this) {
                var name = username;
                var domain = ".";
                username = username.Replace("\\", "/");
                if (username.Contains("/")) {
                    domain = username.Split('/')[0];
                    name = username.Split('/')[1];
                }
                else if (username.Contains("@")) {
                    domain = username.Split('@')[1];
                    name = username.Split('@')[0];
                }

                var authenticated = LogonUserW(name, domain, password, logontype, 0, ref token);
                return authenticated;
            }
        }
    }
}