using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Experiments;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace Qorpent {
    public abstract class CacheServiceBase<TItem, TLeaseType> : ExtensibleServiceBase<TLeaseType>, ICacheService<TItem, TLeaseType> where TLeaseType : ICacheLease {
        protected IDictionary<string, TItem> InternalCache { get; private set; } 
        
        public CacheServiceBase() {
            InternalCache = new Dictionary<string, TItem>();
            RefreshRate = 10000;
        }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Refresh();
        }

        public bool Refresh() {
            if (DateTime.Now.AddMilliseconds(-RefreshRate) > LastRefresh) {
                Logg.Debug(new { cache = "begin refresh" }.stringify());
                LastRefresh = DateTime.Now;
                lock (this) {
                    var currentEtag = ETag;
                    var currentVersion = Version;
                    foreach (var extension in Extensions) {
                        extension.Refresh();
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
                    Refresh();
                    if (InternalCache.ContainsKey(key)) {
                        return InternalCache[key];
                    }
                }
                var obj = retriever(key);
                InternalCache[key] = obj;
				
                return obj;
            }
        }

        public void Clear() {
            lock (this) {
                InternalCache.Clear();
                LastRefresh = DateTime.MinValue;
            }
        }

        public int RefreshRate { get; set; }
        public DateTime LastRefresh { get; set; }
    }
}