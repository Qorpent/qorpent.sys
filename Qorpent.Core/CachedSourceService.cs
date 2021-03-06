using System.Collections.Generic;
using System.Linq;
using qorpent.v2.query;
using qorpent.v2.security.authorization;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Model;

namespace Qorpent {
    public class CachedSourceService<TSource, TResult, TLease> : ExtensibleServiceBase<TSource>, IObjectSource<TResult>
        where TLease : ICacheLease
        where TSource : IObjectSource<TResult>
    {
        private ICacheService<TResult, TLease> _cache;

        [Inject]
        public ICacheService<TResult, TLease> Cache
        {
            get { return _cache ?? (_cache = new CacheService<TResult, TLease>()); }
            set { _cache = value; }
        }

        public override object Reset(ResetEventData data)
        {
            Cache.Clear();
            return null;
        }


        public virtual TResult Get(string id, IScope scope = null)
        {
            return Cache.Get(id, _ => {
                return Extensions.Select(__ => __.Get(_)).FirstOrDefault(__ => null != __);
            });
        }

        public SearchResult<TResult> Search(object query = null, IScope scope = null)
        {
            var subqueries = Extensions.Select(_ => _.Search(query, scope)).Where(_ => _ != null && _.Size != 0).ToArray();
            if (subqueries.Length == 0) return new SearchResult<TResult>();
            if (subqueries.Length == 1) return subqueries[0];
            var result = new SearchResult<TResult>();
            var items = new List<TResult>();
            foreach (var subquery in subqueries)
            {
                result.Total += subquery.Total;
                result.Size += subquery.Size;
                foreach (var i in subquery.TypedItems)
                {

                    items.Add(i);
                    if (i is IWithStringId)
                    {
                        var ided = i as IWithStringId;
                        Cache.UpSet(ided.Id, (k, v) => {
                            if (null == v) return i;
                            if (i is IWithIntVersion)
                            {
                                var v1 = v as IWithVersion;
                                var v2 = i as IWithVersion;
                                return v2.Version > v1.Version ? i : v;
                            }
                            return v;
                        });
                    }
                }
            }
            if (typeof (IWithIndex).IsAssignableFrom(typeof (TResult))) {
                result.TypedItems = items.OfType<IWithIndex>().OrderBy(_ => _.Idx).OfType<TResult>().ToArray();
            }
            else {
                result.TypedItems = items.ToArray();
            }
            return result;

        }

        public IEnumerable<TResult> SearchAll(object query = null, IScope scope = null)
        {
            return Extensions.Select(_ => _.SearchAll(query, scope)).SelectMany(_ => _);
        }
    }
}