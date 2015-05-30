using System;
using System.Collections.Generic;
using qorpent.v2.security.authorization;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IoC;
using Qorpent.IO;
using Qorpent.IO.Net;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user.storage.providers
{
    [ContainerComponent(Lifestyle.Singleton, "elastic.usersource", ServiceType = typeof(IUserSource))]
    public class ElasticUserSource : 
        ServiceBase, 
        IUserSource, 
        IUserCacheLease, 
        IWriteableUserSource,
        IRoleResolverCacheLease{
        public ElasticUserSource() {
            Idx = 100;
            Urls = new List<string>{"http://127.0.0.1:9200"};
            Index = "app";
            Type = "pwd";
            LogId = "elastic.usersource";
            PingRate = 10000;
            CacheRate = 5000;
            WriteUsersEnabled = true;
            
        }

        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        public IList<string> Urls { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public string LogId { get; set; }

        private const string leasequery = @"{
  
    ""aggs"" : {
        ""_version"" : { ""max"" : { ""field"" : ""_version"" } },
        ""_timestamp"" : { ""max"" : { ""field"" : ""updatetime"" } }
    }
  
}";

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        private bool _initialized = false;
        public bool WriteUsersEnabled { get; set; }
        public void Initialize(bool forced = false) {
            if (_initialized && !forced) return;
            WriteUsersEnabled = true;
            if (null != ConfigProvider) {
                var xml = ConfigProvider.GetConfig();
                if (null != xml) {
                    var element = xml.Element("logon");
                    if (null != element) {
                        element = element.Element("elastic");
                        
                    }
                    if (null != element) {
                        
                        var _urls = element.Attr("urls","");
                        if (!string.IsNullOrWhiteSpace(_urls)) {
                            Urls = _urls.SmartSplit(false, true, ';', ',');
                        }
                        WriteUsersEnabled = element.Attr("active", "true").ToBool();
                        Index = element.Attr("index", Index);
                        Type = element.Attr("type", Type);
                        LogId = element.Attr("logid", LogId);
                        IsDefault = element.Attr("defaultstore").ToBool();
                        PingRate = element.Attr("pingrate", PingRate.ToString()).ToInt();
                        CacheRate = element.Attr("cacherate", CacheRate.ToString()).ToInt();
                    }
                }
            }
            Logg = LoggyManager.Get(LogId);
            if (WriteUsersEnabled) {
                ExecuteCommand("/" + Index, method: "PUT");
            }
            _initialized = true;
        }

        public int Idx { get; set; }

        public IUser GetUser(string login) {
            if (!WriteUsersEnabled) return null;
            lock (this) {
                CheckCache();
                if (InvalidConnection) return null;
                if (_cache.ContainsKey(login)) {
                    return _cache[login];
                }
                var id = UserSerializer.GetId(login);
                var json = ExecuteCommand(GetBaseUrl() + id);
                if (null == json) {
                    return null;
                }
                var j = json.jsonify();
                var found = j.bul("found");
                if (found) {
                    var version = j.num("_version");
                    var src = j.map("_source");
                    var user = UserSerializer.CreateFromJson(src);
                    user.Id = id;
                    user.Version = version;
                    _cache[login] = user;
                }
                else {
                    _cache[login] = null;
                }
                return _cache[login];
            }
        }

        public bool InvalidConnection { get; set; }

        private void CheckCache(bool forced = false) {
            if (!_initialized) {
                Initialize();
            }
            if (!forced && InvalidConnection) {
                if (LastPing.AddMilliseconds(PingRate) > DateTime.Now) {
                    return;
                }
            }
            if (!forced && LastCheck.AddMilliseconds(CacheRate) > DateTime.Now) {
                return;
            }
            var currentETag = ETag;
            var currentVersion = Version;
            var json = ExecuteCommand(GetBaseUrl()+"_search?search_type=count", leasequery);
            if (null == json) {
                ETag = null;
                Version = DateTime.MinValue;
                LastCheck = DateTime.MinValue;
                
            }
            else {
                var j = json.jsonify();
                ETag = j.str("aggregations._version.value");
                Version = j.date("aggregations._timestamp.value_as_string");
            }

            if (ETag != currentETag || Version != currentVersion) {
                _cache.Clear();
            }
        }

        private string GetBaseUrl() {
            return "/" + Index + "/" + Type + "/";
        }

        private string ExecuteCommand(string cmd, string post = null,string method= "") {
            lock (this) { 
            string json;
            for (var i = 0; i < Urls.Count; i++) {
                var url = Urls[i] + cmd;
                try {
                    var cli = new HttpClient{ConnectionTimeout = 100};
                    json = cli.GetString(url, post, _ => {
                        if (!string.IsNullOrWhiteSpace(method)) {
                            _.Method = method;
                        }
                    });
                    InvalidConnection = false;
                    if (i != 0) {
                        var best = Urls[i];
                        Urls.RemoveAt(i);
                        Urls.Insert(0,best);
                    }
                    return json;
                }
                catch (IOException e) {
                    if(i<Urls.Count-1)continue;
                    Logg.Error("Invalid Elastic Search Login "+e.Message, e);
                    InvalidConnection = true;
                    LastError = e;
                    return null;
                }
                finally {
                    LastPing = DateTime.Now;
                }
            }
            return null;
                }
        }

        public Exception LastError { get; set; }

        IDictionary<string,IUser> _cache = new Dictionary<string, IUser>(); 

        public bool Refresh() {
            CheckCache(true);
            return true;
        }



        public string ETag {
            get { return _eTag; }
            set { _eTag = value; }
        }

   
        private string _eTag;
        private DateTime _version;

        public DateTime Version {
            get { return _version; }
            set { _version = value; }
        }

        public int PingRate { get; set; }
        public int CacheRate { get; set; }
        public DateTime LastPing { get; set; }
        public DateTime LastCheck { get; set; }

        public bool IsDefault { get; set; }

        public IUser Store(IUser user) {
            if (!WriteUsersEnabled) {
                throw new Exception("not enabled");
            }
            lock (this) {
                if (InvalidConnection) {
                    throw new Exception("cannot store due to invalid connection");
                }
                user.Id = UserSerializer.GetId(user);
                if (user.CreateTime.Year <= 1900) {
                    user.CreateTime = DateTime.Now.ToUniversalTime();
                }
                user.UpdateTime = DateTime.Now.ToUniversalTime();
                var json = UserSerializer.GetJson(user,"store");
                var url = GetBaseUrl() + user.Id;
                if (user.Version > 0) {
                    url += "?version=" + user.Version;
                }
                var result = ExecuteCommand(url, json);
                if (null == result) {
                    throw new Exception("invalid storage operation",LastError);
                }
                var j = result.jsonify();
                var version = j.num("_version");
                user.Version = version;
                _cache[user.Login] = user;
                return user;
            }
        }

        public void Clear() {
            _cache.Clear();
            
        }
    }
}
