using System.Collections.Generic;
using qorpent.v2.query;
using Qorpent;

namespace bit.cross.accident.storage.storages {
    public interface IObjectSource {
        object Get(string id, IScope scope =null);
        SearchResult Search(object query=null, IScope scope = null);
        IEnumerable<object> SearchAll(object query = null,IScope scope = null);
    }

    public interface IObjectSource<T> : IObjectSource
    {
        T Get(string id, IScope scope = null);
        SearchResult<T> Search(object query = null, IScope scope = null);
        IEnumerable<T> SearchAll(object query = null, IScope scope = null);
    }
}