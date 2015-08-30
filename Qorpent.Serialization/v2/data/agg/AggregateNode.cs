using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.data.agg {
    public class AggregateNode:IJsonSerializable {
      
        private  IDictionary<string, AggregateNode> _children;
        private IDictionary<string, object> _values;

        public AggregateNode Parent { get; set; }

        public IDictionary<string, AggregateNode> Children
        {
            get { return _children??(_children = new Dictionary<string,AggregateNode>()); }
        }

        public IDictionary<string, object> Values
        {
            get { return _values ?? (_values=new Dictionary<string, object>()); }
        }

        protected int IDBASIS = 0;
        private int _id=-1;
        private IDictionary<string, object> _meta;

        public RouteKey RouteKey { get; set; }

        public int Id
        {
            get { return _id == -1 ? (_id = GetRoot().IDBASIS++):_id; }
            set { _id = value; }
        }

        public AggregateNode GetRoot() {
            if (null == Parent) return this;
            return Parent.GetRoot();
        }

        public IDictionary<string, object> Meta
        {
            get { return _meta??(_meta=new Dictionary<string, object>()); }
            set { _meta = value; }
        }

        public int Level
        {
            get { return null == Parent ? 0 : (Parent.Level + 1); }
        }

        public bool IsBucket { get; set; }

        public object GetValue(string name, params string[] path) {
            if (name.Contains("/") || name.Contains("."))
            {
                var p = name.SmartSplit(false, true, '/', '.');
                return GetValue(p[0], p.Skip(1).ToArray());
            }
            if (path != null && 0 != path.Length) {
                var realname = path.Last();
                var node = GetNode(name, path.Take(path.Length - 1).ToArray());
                return node.GetValue(realname);

            }
            if (Values.ContainsKey(name)) {
                return Values[name];
            }
            return null;
        }

        public void SetValue(string name, object value) {
            Values[name] = value;
        }

        public AggregateNode GetNode(string name, params string[] path) {
            if (name.Contains("/") || name.Contains(".")) {
                var p = name.SmartSplit(false, true, '/','.');
                return GetNode(p[0], p.Skip(1).ToArray());
            }
            if (path != null && 0 != path.Length) {
                var current = GetNode(name);
                foreach (var s in path) {
                    current = current.GetNode(s);
                }
                return current;
            }
            if (!Children.ContainsKey(name)) {
                return Children[name] = new AggregateNode {Parent = this};
            }
            return Children[name];
        }


        public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            var jw = new JsonWriter(output,pretty:pretty,level:level);
            jw.OpenObject();
            if (0 != Meta.Count) {
                jw.WriteProperty("meta",Meta);
            }
            jw.WriteProperty("id",Id);
            jw.WriteProperty("isbucket",IsBucket);
            if (null != RouteKey) {
                jw.WriteProperty("key",RouteKey.Key);
                jw.WriteProperty("name",RouteKey.Name);
                jw.WriteProperty("sortkey",RouteKey.SortKey);
                jw.WriteProperty("comment",RouteKey.Comment);
            }
            jw.OpenProperty("values");
            jw.OpenArray();
            foreach (var value in Values) {
                jw.WriteObject(new {key=value.Key,value=value.Value});
            }
            jw.CloseArray();
            jw.CloseProperty();
            

            if (null != _children && 0 != _children.Count) {
                jw.OpenProperty("aggs");
                jw.OpenArray();
                foreach (var node in Children) {
                    jw.WriteObject(node.Value);
                }
                jw.CloseArray();
                jw.CloseProperty();
            }
            jw.CloseObject();
        }
    }
}