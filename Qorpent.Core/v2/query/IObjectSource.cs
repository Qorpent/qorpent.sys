using System.Collections.Generic;
using Qorpent;

namespace qorpent.v2.query {
    public interface IObjectSource {
        object Get(string id, IScope scope =null);
        SearchResult Search(object query=null, IScope scope = null);
        IEnumerable<object> SearchAll(object query = null,IScope scope = null);
    }

    public interface IObjectSource<T>
    {
        T Get(string id, IScope scope = null);
        SearchResult<T> Search(object query = null, IScope scope = null);
        IEnumerable<T> SearchAll(object query = null, IScope scope = null);
    }
}