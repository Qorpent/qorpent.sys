using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Experiments;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace Qorpent {
    public class CacheService<TItem, TLeaseType> : ExtensibleServiceBase<TLeaseType>, ICacheService<TItem, TLeaseType> where TLeaseType : ICacheLease {
        protected IDictionary<string, TItem> InternalCache { get; private set; } 
        
        public CacheService() {
            InternalCache = new Dictionary<string, TItem>();
            RefreshRate = 10000;
        }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Refresh(true);
        }

        public bool Refresh(bool forse) {
            if (DateTime.Now.AddMilliseconds(-RefreshRate) > LastRefresh) {
                Logg.Debug(new { cache = "begin refresh" }.stringify());
                LastRefresh = DateTime.Now;
                lock (this) {
                    var currentEtag = ETag;
                    var currentVersion = Version;
                    foreach (var extension in Extensions) {
                        extension.Refresh(forse);
                    }
                    if (currentEtag == ETag && currentVersion == Version) return false;
                    Logg.Trace(new{cache="refreshed"}.stringify());
                    Clear();
                    
                    return true;
                }
            }
            return false;

        }

        public string ETag {
            get {
                if (Extensions.Count == 0) return "undefined";
                return string.Join("/", Extensions.Select(_ => _.ETag)).GetMd5();
            }
        }

        public DateTime Version {
            get {
                if (Extensions.Count == 0) return DateTime.MinValue;
                return Extensions.Max(_ => _.Version);
            }
        }

        public TItem Get(string key, Func<string, TItem> retriever) {
            lock (this) {
                if (InternalCache.ContainsKey(key)) {
                    Refresh(false);
                    if (InternalCache.ContainsKey(key)) {
                        return InternalCache[key];
                    }
                }
                var obj = retriever(key);
                InternalCache[key] = obj;
				
                return obj;
            }
        }

        public object Clear() {
            lock (this) {
                var _size = InternalCache.Count;
                InternalCache.Clear();
                LastRefresh = DateTime.MinValue;
                return _size;
            }
        }

        public int RefreshRate { get; set; }
        public bool Exists(string key) {
            return InternalCache.ContainsKey(key);

        }

        public TItem UpSet(string key, Func<string,TItem, TItem> setup = null) {
            if (Exists(key)) {
                var existed = InternalCache[key];
                if (null == setup) return existed;
                var updated = setup(key, existed);
                if (!Equals(existed,updated)) {
                    InternalCache[key] = updated;
                }
                return updated;
            }
            if (null == setup) return default(TItem);
            return Get(key, _ => setup(_, default(TItem)));
        }

        public DateTime LastRefresh { get; set; }
    }
}