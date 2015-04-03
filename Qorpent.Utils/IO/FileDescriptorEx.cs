using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.Git;

namespace Qorpent.Utils.IO
{
    /// <summary>
    /// Описывает файловый или условно файловый ресурс с дополнительными возможностями
    /// </summary>
    public class FileDescriptorEx : IVersionedDescriptor {
        private GitCommitInfo _commitInfo;
        private DateTime _version;
        private string _hash;
        private bool _isGitBased;
        private XElement _header;


        /// <summary>
        /// Признак использования Git в качестве источника версий
        /// </summary>
        public bool IsGitBased {
            get {
                CheckoutVersions();
                return _isGitBased;
            }
            set { _isGitBased = value; }
        }

        /// <summary>
        /// Хэщ версии
        /// </summary>
        public string Hash {
            get {
                CheckoutVersions();
                return _hash;
            }
            set { _hash = value; }
        }

        /// <summary>
        /// Время формирования версии
        /// </summary>
        public DateTime Version {
            get {
                CheckoutVersions();
                return _version;
            }
            set { _version = value; }
        }

        /// <summary>
        /// Логическое или локальное имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Полное имя файла
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Информация о связанном коммите
        /// </summary>
        public GitCommitInfo CommitInfo {
            get {
                CheckoutVersions();
                return _commitInfo;
            }
            set { _commitInfo = value; }
        }

        /// <summary>
        /// Расширенные атрибуты текстового файла
        /// </summary>
        public XElement Header {
            get {
                if (null == _header) {
                    ReadAttributes();
                }
                return _header;
            }
            set { _header = value; }
        }

        public void Refresh() {
            if (!string.IsNullOrWhiteSpace(FullName)) {
                Header = null;
                Hash = null;
                Version = DateTime.MinValue;
            }
        }

        private void ReadAttributes() {
            lock (this) {
                if (IsAssembly) {
                    Header = ReadAssemblyHeader();
                }
                else {
                    Header = FileSystemHelper.ReadXmlHeader(FullName);
                }
                if (null == _header) {
                    _header = new XElement("stub");
                }
            }
        }

        private XElement ReadAssemblyHeader() {
            using (var a = new AssemblyUsage(FullName)) {
                var assembly = a.Assembly;              
                var result = new XElement("assembly");
               
                foreach (var attribute in assembly.GetCustomAttributes()) {
                    var properties = attribute.GetType().GetProperties().Where(_ => _.Name != "TypeId").ToArray();
                    if (properties.Length == 1) {
                        result.SetAttributeValue(attribute.GetType().Name, properties[0].GetValue(attribute));
                    }
                    else {
                     
                    var e = new XElement(attribute.GetType().Name);
                    foreach (var info in properties) {
                        var val = info.GetValue(attribute).ToStr();
                        e.SetAttributeValue(info.Name,val);

                    }
                    result.Add(e);   
                    }
                }

                return result;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool AllowNotExisted { get; set; }

        private void CheckoutVersions() {
            if (!string.IsNullOrWhiteSpace(_hash)) {
                return;
            }
            if (string.IsNullOrWhiteSpace(FullName)) {
                throw new Exception("FullName not setup");
            }
            if (!File.Exists(FullName)) {
                if (AllowNotExisted) {
                    Version = DateTime.MinValue;
                    Hash = "INIT";
                    return;
                }
                throw new Exception("file not exists "+FullName);
            }
            lock (this) {
               
                if (! CheckGitVersion()) {
                    if (IsAssembly) {
                        CheckAssemblyVersion();
                    }
                    else {
                        CheckTextFileVersion();
                    }
                    
                }
                
            }
        }
        [Serializable]
        class AssemblyUsage : IDisposable {
            [Serializable]
            class Mapper {
                private Dictionary<string, string> map;
                private AppDomain domain;

                public Mapper(AppDomain appdomain) {
                    this.domain = appdomain;
                    this.map = new Dictionary<string, string>();
                    foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        map[a.GetName().Name.Split(',')[0].ToLowerInvariant()] = a.Location;
                    }
                    appdomain.AssemblyResolve += Resolve;
                    appdomain.UnhandledException+=appdomain_UnhandledException;
                }

                private void appdomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
                    Console.WriteLine(e.ExceptionObject);
                }

                public Assembly Resolve(object s, ResolveEventArgs a) {
                    var localname = a.Name.Split(',')[0].ToLowerInvariant();
                    if (map.ContainsKey(localname))
                    {
                        return domain.Load(File.ReadAllBytes(map[localname]));
                    }

                    return null;
                }
            }
            public AssemblyUsage(string fullname) {
                Evidence e = new Evidence();
                e.AddHostEvidence(new Zone(SecurityZone.Trusted));
                var ads = new AppDomainSetup { ApplicationBase = Path.GetDirectoryName(fullname) };
                this.Sandbox = AppDomain.CreateDomain("test", e, ads, new PermissionSet(PermissionState.Unrestricted));
                new Mapper(Sandbox);


                try {
                    var assembly = Sandbox.Load(Path.GetFileNameWithoutExtension(fullname));
                    this.Assembly = assembly;
                }
                catch (Exception ex) {
                    this.Assembly = Assembly.Load(File.ReadAllBytes(fullname));
                }
               

            }

            

            private AppDomain Sandbox { get; set; }

            public Assembly Assembly { get; private set; }

            public void Dispose() {
                Assembly = null;
                AppDomain.Unload(Sandbox);

            }
        }

        private void CheckAssemblyVersion() {
            using (var a = new AssemblyUsage(FullName)) {
                var assembly = a.Assembly;
                Version = File.GetLastWriteTimeUtc(FullName);
                Hash = assembly.GetName().Version.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAssembly {
            get {
                if (string.IsNullOrWhiteSpace(FullName)) return false;
                return Path.GetExtension(FullName).ToLowerInvariant() == ".dll";
            }
        }

        private void CheckTextFileVersion() {
            using (var s = File.Open(FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var md5 = MD5.Create();
                var hash = md5.ComputeHash(s);
                Hash = Convert.ToBase64String(hash);
                Version = File.GetLastWriteTimeUtc(FullName);
            }
        }

        protected bool CheckGitVersion() {
            var gitCommit = GitHelper.GetLastCommit(FullName);
            if (null != gitCommit) {
                IsGitBased = true;
                CommitInfo = gitCommit;
                Hash = gitCommit.Hash;
                Version = gitCommit.GlobalRevisionTime;
                return true;
            }
            return false;
        }
    }
}
