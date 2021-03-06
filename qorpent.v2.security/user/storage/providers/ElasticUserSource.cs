﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using qorpent.v2.security.authorization;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Net;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.user.storage.providers {
    [ContainerComponent(Lifestyle.Singleton, "elastic.usersource", ServiceType = typeof (IUserSource))]
    public class ElasticUserSource :
        InitializeAbleService,
        IUserSource,
        IUserCacheLease,
        IWriteableUserSource,
        IRoleResolverCacheLease {
        private const string leasequery = @"{
  
    ""aggs"" : {
        ""_version"" : { ""max"" : { ""field"" : ""_version"" } },
        ""_timestamp"" : { ""max"" : { ""field"" : ""updatetime"" } }
    }
  
}";
        private readonly IDictionary<string, IUser> _cache = new Dictionary<string, IUser>();
        private  bool _initialized = false;

        public ElasticUserSource() {
            EsClient = new ElasticSearchClient();
            Idx = 100;
            EsClient.Urls = new List<string> {"http://127.0.0.1:9200"};
            Index = "app";
            Type = "pwd";
            LogId = "elastic.usersource";
            PingRate = 10000;
            CacheRate = 5000;
            WriteUsersEnabled = true;
        }

        public ElasticSearchClient EsClient { get; private set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public string LogId { get; set; }
        public int PingRate { get; set; }
        public int CacheRate { get; set; }
        public DateTime LastCheck { get; set; }

        public bool Refresh(bool forse) {
            CheckCache(forse);
            return true;
        }

        public string ETag { get; set; }
        public DateTime Version { get; set; }
        public int Idx { get; set; }

        public IUser GetUser(string login) {
            if (!WriteUsersEnabled) {
                return null;
            }
            lock (this) {
                CheckCache();
                if (EsClient.InvalidConnection) {
                    return null;
                }
                if (_cache.ContainsKey(login)) {
                    return _cache[login];
                }
	            string json;
	            if (login.StartsWith("!certid:")) {
					var post = new {query = new {match = new {publickey = login.Replace("!certid:", "")}}}.stringify();
					json = EsClient.ExecuteCommand(GetBaseUrl() + "_search", post);
	            } else {
					var id = UserSerializer.GetId(login);
					json = EsClient.ExecuteCommand(GetBaseUrl() + id);		            
	            }
	            if (null == json) {
                    return null;
                }
                var j = json.jsonify();
                var found = j.bul("found") || j.num("*.total") > 0;
                if (found) {
	                var src = j.map("*._source");
                    var version = j.resolvenum("_version","__version");
                    var user = UserSerializer.CreateFromJson(src);
	                user.Id = j.resolvestr("_id","__id");
                    user.Version = version;
                    _cache[login] = user;
                }
                else {
                    _cache[login] = null;
                }
                return _cache[login];
            }
        }

        public IEnumerable<IUser> SearchUsers(UserSearchQuery query) {
            var q = string.IsNullOrWhiteSpace(query.Query)? "login:*" : query.Query;
            if (!string.IsNullOrWhiteSpace(query.Login)) {
                q += " AND login:" + query.Login;
            }
            if (!string.IsNullOrWhiteSpace(query.Name)) {
                q += " AND name:" + query.Name;
            }
            if (!query.Groups) {
                q += " AND NOT isgroup:true";
            }
            if (!query.Users) {
                q += " AND isgroup:true";
            }
            if (!string.IsNullOrWhiteSpace(query.Domain)) {
                q += " AND domain:" + query.Domain;
            }
            var esquery = new {
                query = new {query_string = new {query = q}}
            };
            var json = EsClient.ExecuteCommand(GetBaseUrl() + "_search", esquery.stringify());
            if (null == json)
            {
                yield break;
            }
            var j = json.jsonify();
            var hits = j.arr("hits.hits");
            foreach (var hit in hits) {
                var src = hit.map("*._source");
                var version = hit.resolvenum("_version", "__version");
                var user = UserSerializer.CreateFromJson(src);
                user.Id = hit.resolvestr("_id", "__id");
                user.Version = version;
                _cache[user.Login] = user;
                yield return user;
            }
        }

        public bool WriteUsersEnabled { get; set; }
        public bool IsDefault { get; set; }

        public IUser Store(IUser user) {
            if (!WriteUsersEnabled) {

                throw new Exception("not enabled");
            }
            lock (this) {

                user.Id = UserSerializer.GetId(user);
                if (user.CreateTime.Year <= 1900) {
                    user.CreateTime = DateTime.Now.ToUniversalTime();
                }
                user.UpdateTime = DateTime.Now.ToUniversalTime();
                var json = UserSerializer.GetJson(user, "store");
                var url = GetBaseUrl() + user.Id + "?refresh=true";

                var result = EsClient.ExecuteCommand(url, json);
                if (null == result) {
                    throw new Exception("invalid storage operation", EsClient.LastError);
                }
                var j = result.jsonify();
                var version = j.num("_version");
                user.Version = version;
                _cache[user.Login] = user;
                return user;
            }
        }

        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);
            var element = e.Element("logon");
            if (null != element) {
                element = element.Element("elastic");
            }
            if (null != element) {
                var _ref = element.Attr("ref");
                if (!string.IsNullOrWhiteSpace(_ref)) {
                    var r = e.Element("elastic");
                    if (null == r) {
                        throw new Exception("invalid ref");
                    }
                    r = r.Element(_ref);
                    if (null == r) {
                        throw new Exception("invalid ref");
                    }
                    ReadSingleElement(r);
                }
                ReadSingleElement(element);
            }
        }

        private void ReadSingleElement(XElement element) {
            var _urls = element.Attr("urls", "");
            if (!string.IsNullOrWhiteSpace(_urls)) {
                EsClient.Urls = _urls.SmartSplit(false, true, ';', ',');
            }
            WriteUsersEnabled = element.Attr("active", "true").ToBool();

            Index = element.Attr("index", Index);
            Type = element.Attr("type", Type);
            LogId = element.Attr("logid", LogId);
            IsDefault = element.Attr("defaultstore").ToBool();
            PingRate = element.Attr("pingrate", PingRate.ToString()).ToInt();
            CacheRate = element.Attr("cacherate", CacheRate.ToString()).ToInt();
        }

        private void CheckCache(bool forced = false) {
            if (!_initialized) {
                Initialize();
                _initialized = true;
            }
            if (!forced && EsClient.InvalidConnection) {
                if (EsClient.LastPing.AddMilliseconds(PingRate) > DateTime.Now) {
                    return;
                }
            }
            if (!forced && LastCheck.AddMilliseconds(CacheRate) > DateTime.Now) {
                return;
            }
            var currentETag = ETag;
            var currentVersion = Version;
            var url = GetBaseUrl() + "_search?search_type=count";
            var query = leasequery;
            var json = EsClient.ExecuteCommand(url, query);
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
            if (ETag != currentETag || Version != currentVersion ) {
                _cache.Clear();
            }
        }

        private string GetBaseUrl() {
            return "/" + Index + "/" + Type + "/";
        }

        public void Clear() {
            _cache.Clear();
        }
    }
}