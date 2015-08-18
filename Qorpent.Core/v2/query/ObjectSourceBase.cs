using System.Collections.Generic;
using Qorpent;
using Qorpent.v2.query;

namespace qorpent.v2.query {
    public abstract class ObjectSourceBase<T> : InitializeAbleService, IObjectSource<T>,IObjectSource where T : class {

        public abstract T Get(string id, IScope scope = null);
        SearchResult IObjectSource.Search(object query, IScope scope) {
            return Search(query, scope);
        }

        IEnumerable<object> IObjectSource.SearchAll(object query, IScope scope) {
            return SearchAll(query, scope);
        }

        object IObjectSource.Get(string id, IScope scope) {
            return Get(id, scope);
        }

        public abstract SearchResult<T> Search(object query = null, IScope scope = null);

        public virtual IEnumerable<T> SearchAll(object query = null, IScope scope = null) {
            return Search(query, scope).Items;
        }
    }
}