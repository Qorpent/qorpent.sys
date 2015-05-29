using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using qorpent.v2.security.user;
using qorpent.v2.security.user.storage;
using Qorpent;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Security;

namespace qorpent.v2.security.logon.providers
{

    [ContainerComponent(Lifestyle.Transient,"winlogon.passlogon",ServiceType=typeof(ILogonProvider))]
    public class WinLogon: ILogonProvider,IPasswordLogon
    {
        public const int WINLOGONIDX = 10000;

        public WinLogon() {
            Idx = WINLOGONIDX;
        }

        [Inject]
        public IUserService UserService { get; set; }

        /// <summary>
        /// Execute system logon procedure and return true if proceed
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="logontype"></param>
        /// <returns></returns>
        public bool Logon(string username, string password, int logontype = WinLogonType.Logon32LogonNetwork)
        {
            lock (this)
            {
                IntPtr token = new IntPtr();
                return Logon(username, password, ref token, logontype);
            }
        }

        [DllImport("ADVAPI32.dll", EntryPoint =
            "LogonUserW", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUserW(string lpszUsername, string lpszDomain,
                                             string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        /// <summary>
        /// Execute system logon procedure and return true if proceed
        /// can return system token of logon (windows only)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="token"></param>
        /// <param name="logontype"></param>
        /// <returns></returns>
        public bool Logon(string username, string password, ref IntPtr token, int logontype = WinLogonType.Logon32LogonNetwork)
        {
            lock (this)
            {

                var name = username;
                var domain = ".";
                username = username.Replace("\\", "/");
                if (username.Contains("/"))
                {
                    domain = username.Split('/')[0];
                    name = username.Split('/')[1];
                }
                else if (username.Contains("@"))
                {
                    domain = username.Split('@')[1];
                    name = username.Split('@')[0];
                }

                bool authenticated = LogonUserW(name, domain, password, logontype, 0, ref token);
                return authenticated;

            }
        }

        public IIdentity Logon(string username, string password, IScope scope = null)
        {
            lock (this)
            {
                var token = new IntPtr();
                var auth = Logon(username, password, ref token, 3);
                if (!auth) return null;
                 var native =  new WindowsIdentity(token);
                var result = new Identity {
                    Name = native.Name.ToLowerInvariant(),
                    IsAuthenticated = true,
                    AuthenticationType = "win",
                    Native = native
                };
                if (null != UserService) {
                    result.User = UserService.GetUser(result.Name);
                }
                return result;

            }
        }

        public int Idx { get; set; }
        public object Reset(ResetEventData data) {
            return null;
        }

        public object GetPreResetInfo() {
            return null;
        }
    }
}
