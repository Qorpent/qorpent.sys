using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using qorpent.v2.security.authorization;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user.storage.providers {
    [ContainerComponent(Lifestyle.Singleton, "file.usersource", ServiceType = typeof (IUserSource))]
    public class FileUserSource :
        InitializeAbleService,
        IUserSource,
        IUserCacheLease,
        IRoleResolverCacheLease {
        private const string DEFAULTFILEPATH = "@repos@/.app/pwd.bxls";
        private string _file;
        private bool _initialized;
        private readonly IDictionary<string, IUser> _cache = new Dictionary<string, IUser>();

        public FileUserSource() {
            DefaultFilePath = DEFAULTFILEPATH;
            CheckRate = 5000;
            Idx = 200;
            Enabled = true;
        }

        public string DefaultFilePath { get; set; }
        public string ResolvedFilePath { get; set; }
        public bool Enabled { get; set; }
        public DateTime LastFileTime { get; set; }
        public DateTime LastCheck { get; set; }
        public int CheckRate { get; set; }

        public bool Refresh(bool forse = true) {
            CheckCache(forse);
            return true;
        }

        public string ETag {
            get { return "fls"; }
        }

        public DateTime Version {
            get {
                if (string.IsNullOrWhiteSpace(_file)) {
                    return DateTime.MinValue;
                }
                return LastFileTime;
            }
        }

        public IUser GetUser(string username) {
            if (!Enabled) {
                return null;
            }
            lock (this) {
                CheckCache();
                if (_cache.ContainsKey(username.ToLowerInvariant())) {
                    return _cache[username];
                }
                return null;
            }
        }

        public IEnumerable<IUser> SearchUsers(UserSearchQuery query) {
            return _cache.Values.Where(query.IsMatch);
        }



        public int Idx { get; set; }

        public override void Initialize() {
            lock (this) {
                Setup();
                if (File.Exists(ResolvedFilePath)) {
                    _file = ResolvedFilePath;
                    var bsharpresult = BSharpCompiler.CompileFile(_file);
                    foreach (
                        var cls in
                            bsharpresult.ResolveAll("pwd").OrderBy(_ => _.Compiled.Attr("idx").ToInt())) {
                        foreach (var element in cls.Compiled.Elements("usr")) {
                            var record = new User();
                            UserSerializer.ReadXml(record, element);
                            _cache[record.Login] = record;
                        }
                    }
                    LastCheck = DateTime.Now;
                    LastFileTime = File.GetLastWriteTime(_file);
                }
            }
        }

        private void Setup() {
            Enabled = true;
            var file = DefaultFilePath;

            if (null != ConfigProvider) {
                var conf = ConfigProvider.GetConfig();
                if (null != conf) {
                    InitializeFromXml(conf);
                }
            }
            else {
                ResolvedFilePath = EnvironmentInfo.ResolvePath(file);
            }
        }

        public override void InitializeFromXml(XElement e) {
            var file = DefaultFilePath;
            base.InitializeFromXml(e);
            var element = e.Element("logon");
            if (null != element) {
                element = element.Element("file");
            }
            if (null != element) {
                file = element.Attr("path", file);
                Enabled = element.Attr("active", "true").ToBool();
            }
            ResolvedFilePath = EnvironmentInfo.ResolvePath(file);
        }

        private void CheckCache(bool forced = false) {
            if (!_initialized) {
                Initialize();
                _initialized = true;
            }
            if (string.IsNullOrWhiteSpace(_file)) {
                Setup();
                if (File.Exists(ResolvedFilePath)) {
                    _file = ResolvedFilePath;
                }
                else {
                    return;
                }
            }
            if (forced || LastCheck.AddMilliseconds(CheckRate) < DateTime.Now) {
                if (File.GetLastWriteTime(_file) > LastFileTime) {
                    _cache.Clear();
                    Initialize();
                }
                LastCheck = DateTime.Now;
            }
        }

        public override object Reset(ResetEventData data) {
            _cache.Clear();
            Initialize();
            return null;
        }

        public void Clear() {
            _cache.Clear();
            _initialized = false;
            Initialize();
        }
    }
}