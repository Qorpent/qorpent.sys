using Qorpent.Serialization;

namespace qorpent.v2.data.agg {
    public class RouteKey
    {

        private string _key;
        private string _title;
        private string _sortKey;
        public RouteKey() { }
        public RouteKey(string key, string title = null) {
            this.Key = key;
            this.Name = title ?? key;
         
        }

        public string SortKey
        {
            get { return _sortKey ?? Name ?? Key; }
            set { _sortKey = value; }
        }

        public string Key
        {
            get { return _key ?? (_key = _title.Escape(EscapingType.JsonLiteral)); }
            set { _key = value.Escape(EscapingType.JsonLiteral); }
        }

        public string Name
        {
            get { return _title ?? _key; }
            set { _title = value; }
        }

        public string Comment { get; set; }
    }
}