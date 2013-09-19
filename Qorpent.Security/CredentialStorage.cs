using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Security
{
    /// <summary>
    /// Защищенное хранилище пар имя-пароль для WEB
    /// </summary>
    [ContainerComponent(ServiceType = typeof(ICredentialStorage),Name = "default.credentialstorage")]
    public class CredentialStorage:ServiceBase, ICredentialStorage {
        [Inject(Name = "nc.hasheddirectory",Required = true)] private IHashedDirectory FileStorage { get; set; }

        /// <summary>
        /// Получить креденции для указанного сервера и приложения
        /// </summary>
        /// <param name="node"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public ICredentials GetCredentials(string node = "127.0.0.1", string app="default") {
#if PARANOID
            if (Debugger.IsAttached) {
                throw new Exception("cannot use under debugger");
            }
#endif
            if (string.IsNullOrWhiteSpace(node)) node = "127.0.0.1";
            if (string.IsNullOrWhiteSpace(app)) app = "default";
            var key = node + ":" + app;
            if (FileStorage.Exists(key)) {
                var record = new StreamReader(FileStorage.Open(key)).ReadToEnd();
                var delindex = record.IndexOf(':');
                var username = record.Substring(0, delindex);
                var password = record.Substring(delindex + 1);
                var ss = new SecureString();
                foreach (var c in password) {
                    ss.AppendChar(c);
                }
                return new NetworkCredential(username,ss);
            }
            return null;
        }

        /// <summary>
        /// Установить креденции для указанного сервера и приложения
        /// </summary>
        /// <param name="node"></param>
        /// <param name="app"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void SetCredentials(string node, string app, string username, string password)
        {
#if PARANOID
            if (Debugger.IsAttached) {
                throw new Exception("cannot use under debugger");
            }
#endif
            if (string.IsNullOrWhiteSpace(node)) node = "127.0.0.1";
            if (string.IsNullOrWhiteSpace(app)) app = "default";
            if(string.IsNullOrWhiteSpace(username))throw new ArgumentNullException("username");
            if(string.IsNullOrWhiteSpace(username))throw new ArgumentNullException("password");
            var key = node + ":" + app;
            var record = username + ":" + password;
            FileStorage.Write(key, record);
        }
        
    }
}
