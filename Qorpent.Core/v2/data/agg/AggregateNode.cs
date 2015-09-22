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
        private ICollector[] _collectors;
        private IRouter[] _routers;

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

        public IEnumerable<string> CollectCondition(string valname = null) {
            if (null != Parent) {
                foreach (var c in Parent.CollectCondition()) {
                    yield return c;
                }
            }
            if (null!=RouteKey && !string.IsNullOrWhiteSpace(RouteKey.Condition)) {
                yield return this.RouteKey.Condition;
            }
            if (!string.IsNullOrWhiteSpace(valname)) {
                var val = Collectors.First(_ => _.Key == valname);
                if (!string.IsNullOrWhiteSpace(val.Condition)) {
                    yield return val.Condition;
                }
            }
        } 

        public ICollector[] Collectors
        {
            get { return _collectors ?? (null!=Parent?Parent.Collectors:null); }
            set { _collectors = value; }
        }

        public IRouter[] Routs
        {
            get { return _routers ?? (null != Parent ? Parent.Routs : null); }
            set { _routers = value; }
        }
       

public void WriteAsJson(TextWriter output, string mode, ISerializationAnnotator annotator, bool pretty = false, int level = 0) {
            var jw = new JsonWriter(output,pretty:pretty,level:level);
            jw.OpenObject();
            if (0 != Meta.Count) {
                jw.WriteProperty("meta",Meta);
            }
            if (null == Parent && null != Collectors && 0!=Collectors.Length) {
                jw.WriteProperty("collectors", Collectors.OfType<ICollector>().Select(
                    c => new
                    {
                        key = c.Key,
                        name = c.Name ?? c.Key,
                        shortname = c.ShortName ?? c.Name ?? c.Key,
                        group = c.Group,
                        condition = c.Condition
                    }.jsonify()).ToArray());
            }
            if (null == Parent && null != Routs && 0 != Routs.Length) {
                jw.WriteProperty("routs", Routs.Select(
                    c =>
                        new
                        {
                            key = c.Key ?? "nokey",
                            name = c.Name ?? c.Key,
                            shortname = c.ShortName ?? c.Name ?? c.Key,
                            level = c.Level,
                            parent = null == c.Parent ? "" : c.Parent.Key,
                        }.jsonify()).ToArray());
                
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