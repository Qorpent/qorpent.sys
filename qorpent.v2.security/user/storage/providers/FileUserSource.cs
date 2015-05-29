using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent;
using Qorpent.BSharp;
using Qorpent.Events;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user.storage.providers {
    [ContainerComponent(Lifestyle.Singleton,"file.usersource",ServiceType=typeof(IUserSource))]
    [ContainerComponent(Lifestyle.Singleton,"file.usercachelease",ServiceType=typeof(IUserCacheLease))]
    public class FileUserSource : ServiceBase, IUserSource,IUserCacheLease
    {
        private const string DEFAULTFILEPATH = "@repos@/.app/pwd.bxls";

        [Inject]
        public IHostConfigProvider ConfigProvider { get; set; }


        public FileUserSource() {
            DefaultFilePath = DEFAULTFILEPATH;
            CheckRate = 5000;
            Idx = 200;
        }
        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        private void Initialize() {
            lock (this) {
                var file = ResolvePathToFile();
                if (File.Exists(file)) {
                    _file = file;
                    var bsharpresult = BSharpCompiler.CompileFile(_file);
                    foreach (
                        var cls in
                            bsharpresult.ResolveAll("pwd").OrderBy(_ => CoreExtensions.Attr(_.Compiled, "idx").ToInt())) {
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

        public string DefaultFilePath { get; set; }

        private string ResolvePathToFile() {
            var file = DefaultFilePath;
            if (null != ConfigProvider) {
                var conf = ConfigProvider.GetConfig();
                if (null != conf.Definition) {
                    file = conf.Definition.Attr("pwdfile", file);
                }
            }
            file = EnvironmentInfo.ResolvePath(file);
            return file;
        }

        IDictionary<string, IUser> _cache = new Dictionary<string, IUser>();
        private string _file;

        public IUser GetUser(string username)
        {
            lock (this) {
                CheckCache();
                if (_cache.ContainsKey(username.ToLowerInvariant())) return _cache[username];
                return null;
            }
        }

        private bool _initialized = false;
        private void CheckCache(bool forced = false) {
            if (!_initialized) {
                Initialize();
                _initialized = true;
            }
            if (string.IsNullOrWhiteSpace(_file)) {
                var f = ResolvePathToFile();
                if (File.Exists(f)) {
                    _file = f;
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

        public DateTime LastFileTime { get; set; }
        public DateTime LastCheck { get; set; }
        public int CheckRate { get; set; }

        public int Idx { get; set; }
        public bool Refresh() {
            CheckCache(true);
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
    }
}