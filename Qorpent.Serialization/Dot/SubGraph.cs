using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot {
    /// <summary>
    ///     Полный граф DOT
    /// </summary>
    public class SubGraph : GraphElementBase {
        private readonly IDictionary<string, Edge[]> _edgeResolutionCache = new Dictionary<string, Edge[]>();
        private readonly IDictionary<string, Node> _nodeResolutionCache = new Dictionary<string, Node>();
        private Node _defaultNode;
        private Node _defaultEdge;

        /// <summary>
        /// </summary>
        public SubGraph() {
            Nodes = new List<Node>();
            Edges = new List<Edge>();
            SubGraphs = new List<SubGraph>();
        }

        /// <summary>
        /// Добавляет или мержит нод
        /// </summary>
        /// <param name="newNode"></param>
        /// <returns></returns>
        public Node AddNode(Node newNode) {
            var existed = ResolveNode(newNode.Code);
            if (null != existed) {
                existed.Merge(newNode);
                return existed;
            }
            Nodes.Add(newNode);
            newNode.Parent = this;
            return newNode;
        }


        /// <summary>
        /// Добавляет или мержит нод
        /// </summary>
        /// <returns></returns>
        public Edge AddEdge(Edge newEdge, bool merge = true)
        {
            if (merge) {
                var existed = ResolveEdge(newEdge.From, newEdge.To, newEdge.Type, false);
                if (null != existed) {
                    existed.Merge(newEdge);
                    return existed;
                }
            }
            Edges.Add(newEdge);
            newEdge.Parent = this;
            return newEdge;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subgraph"></param>
        public SubGraph AddSubGraph(SubGraph subgraph)
        {
            var existed = ResolveSubgraph(subgraph.Code);

            if (null != existed) {
                existed.Merge(subgraph);
            }
            else {
                existed = new SubGraph {Code = subgraph.Code};
                existed.Merge(subgraph);
            }
            foreach (var n in subgraph.Nodes) {
                AddNode(n);
            }
            foreach (var e in subgraph.Edges) {
                AddEdge(e);
            }
            foreach (var s in subgraph.SubGraphs) {
                existed.AddSubGraph(s);
            }
            return existed;
        }
        /// <summary>
        /// Универсальный метод добавления элементов
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public GraphElementBase AddElement(GraphElementBase element) {
            if (element is Node) {
                return AddNode((Node) element);
            }
            if (element is Edge)
            {
                return AddEdge((Edge)element);
            }
            if (element is SubGraph)
            {
                return AddSubGraph((SubGraph)element);
            }
           throw new Exception("unknown element");
        }

        /// <summary>
        /// Универсальный метод добавления элементов
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public void AddElements(IEnumerable<GraphElementBase> elements)
        {
            foreach (GraphElementBase graphElementBase in elements) {
                AddElement(graphElementBase);
            }
        }


        /// <summary>
        /// Добавляет узел к графу
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static SubGraph operator +(SubGraph graph, Node node) {
            graph.AddNode(node);
            return graph;
        }

        /// <summary>
        /// Добавляет узел к графу
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static SubGraph operator +(SubGraph graph, Edge edge)
        {
            graph.AddEdge(edge);
            return graph;
        }

        /// <summary>
        /// Добавляет узел к графу
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="subgraph"></param>
        /// <returns></returns>
        public static SubGraph operator +(SubGraph graph, SubGraph subgraph) {
            graph.AddSubGraph(subgraph);
            return graph;
        }

       

        /// <summary>
        ///     Подграфы
        /// </summary>
        [SerializeNotNullOnly]
        public IList<SubGraph> SubGraphs { get; private set; }

        /// <summary>
        ///     Узел по умолчанию
        /// </summary>
        [SerializeNotNullOnly]
        public Node DefaultNode {
            get { return _defaultNode; }
            set { 
                _defaultNode = value; 
                if (null != _defaultNode) {
                    _defaultNode.Code = "node";
                }
            }
        }

        /// <summary>
        ///     Ребро по умолванию
        /// </summary>
        [SerializeNotNullOnly]
        public Node DefaultEdge {
            get { return _defaultEdge; }
            set {
                _defaultEdge = value;
                if (null != _defaultEdge) {
                    _defaultEdge.Code = "edge";
                }
            }
        }

        /// <summary>
        ///     Узлы
        /// </summary>
        [SerializeNotNullOnly]
        public IList<Node> Nodes { get; private set; }

        /// <summary>
        ///     Ребра
        /// </summary>
        [SerializeNotNullOnly]
        public IList<Edge> Edges { get; private set; }

        /// <summary>
        ///     Направление графа
        /// </summary>
       [IgnoreSerialize]
        public RankDirType RankDir {
            get { return Get<RankDirType>(DotConstants.RankDirAttribute); }
            set { Set(DotConstants.RankDirAttribute, value); }
        }

        /// <summary>
        ///     Если "Да", то расположение графа центировано, а если "Нет", то не центировано
        /// </summary>
        [IgnoreSerialize]
        public bool Center {
            get { return Get<bool>(DotConstants.CenterAttribute); }
            set { Set(DotConstants.CenterAttribute, value); }
        }

        /// <summary>
        ///     Задает цвет границы подграфов. По умолчанию черный
        /// </summary>
        [IgnoreSerialize]
        public ColorAttribute PenColor {
            get { return Get<ColorAttribute>(DotConstants.PenColorAttribute); }
            set { Set(DotConstants.PenColorAttribute, value); }
        }

        /// <summary>
        ///     Родительский граф
        /// </summary>
        [IgnoreSerialize]
        public SubGraph Parent { get; set; }


        /// <summary>
        ///     Автонастройка
        /// </summary>
        public override void AutoTune() {
            base.AutoTune();
            if (null != DefaultNode) {
                DefaultNode.AutoTune();
            }
            if (null != DefaultEdge) {
                DefaultEdge.AutoTune();
            }
            foreach (var sg in SubGraphs) {
                sg.AutoTune();
            }
            foreach (var n in Nodes) {
                n.AutoTune();
            }
            foreach (var e in Edges) {
                e.AutoTune();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SubGraph ResolveSubgraph(string code) {
            var __code = DotLanguageUtils.GetClusterCode(code);
            if (Code == __code) return this;
            var result = SubGraphs.Select(sg => sg.ResolveSubgraph(__code)).FirstOrDefault(_=>null!=_);
            return result;
        }

        /// <summary>
        ///     Очистка кэшей
        /// </summary>
        public void CleanResolutionCache() {
            if (null != Parent) Parent.CleanResolutionCache();
            _edgeResolutionCache.Clear();
            _nodeResolutionCache.Clear();
        }

        /// <summary>
        ///     Метод резолюции узлов
        /// </summary>
        /// <param name="code"></param>
        /// <param name="cached"></param>
        /// <returns></returns>
        public Node ResolveNode(string code, bool cached = true) {
            
            if (null != Parent) return Parent.ResolveNode(code);
            var __code = DotLanguageUtils.EscapeCode(code);
            if (cached && _nodeResolutionCache.ContainsKey(__code)) {
                return _nodeResolutionCache[__code];
            }
            
            var resolved = EnumerateNodes().FirstOrDefault(_ => _.Code == __code);
            if (cached && null != resolved) {
                _nodeResolutionCache[__code] = resolved;
            }
            return resolved;
        }

        /// <summary>
        ///     Метод резолюции узлов
        /// </summary>
        /// <returns></returns>
        public Edge ResolveEdge(string from, string to, string type = null, bool cached = true) {
            Edge[] edges = ResolveEdges(from,to,cached);
            if (null == edges) return null;
            if (0 == edges.Length) return null;
            if (string.IsNullOrWhiteSpace(type)) {
                return edges[0];
            }
            return edges.FirstOrDefault(_ => _.Type == type);
        }

        /// <summary>
        ///     Метод резолюции узлов
        /// </summary>
        /// <returns></returns>
        public Edge[] ResolveEdges(string from, string to,bool cached = true) {
            if (null != Parent) return Parent.ResolveEdges(from, to);
            var __from = DotLanguageUtils.EscapeCode(from);
            var __to = DotLanguageUtils.EscapeCode(to);
            var key = from + "->" + to;
            if (cached && _edgeResolutionCache.ContainsKey(key)) {
                return _edgeResolutionCache[key];
            }
            var resolved = EnumerateEdges().Where(_ => _.From == __from && _.To == __to).ToArray();
            if (cached) {
                _edgeResolutionCache[key] = resolved;
            }
            return resolved;
        }

        /// <summary>
        ///     Перечислить все узлы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node> EnumerateNodes() {
            foreach (Node n in Nodes) yield return n;
            foreach (SubGraph subGraph in SubGraphs) {
                foreach (Node n in subGraph.EnumerateNodes()) {
                    yield return n;
                }
            }
        }

        /// <summary>
        ///     Перечислить все узлы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Edge> EnumerateEdges() {
            foreach (Edge e in Edges) yield return e;
            foreach (SubGraph subGraph in SubGraphs) {
                foreach (Edge e in subGraph.EnumerateEdges()) {
                    yield return e;
                }
            }
        }

        /// <summary>
        ///     Перечислить все субграфы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SubGraph> EnumerateSubGraphs() {
            foreach (SubGraph n in SubGraphs) yield return n;
            foreach (SubGraph subGraph in SubGraphs) {
                foreach (SubGraph n in subGraph.EnumerateSubGraphs()) {
                    yield return n;
                }
            }
        }

        /// <summary>
        ///     Установить родителя по субграфам
        /// </summary>
        protected void SetParentsForSubgraphs() {
            foreach (SubGraph sg in SubGraphs) {
                sg.Parent = this;
                sg.SetParentsForSubgraphs();
            }
        }

        /// <summary>
        /// Нормализует родителей для узлов и ребер
        /// </summary>
        public void SetParentsForNodesAndEdges() {
            foreach (var n in Nodes) {
                n.Parent = this;
            }
            foreach (var e in Edges) {
                e.Parent = this;
            }
            foreach (var subGraph in SubGraphs) {
                subGraph.SetParentsForNodesAndEdges();
            }
        }
    }
}