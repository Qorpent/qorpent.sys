using System.Collections.Generic;
using System.Text;

namespace Qorpent.Graphs.Dot {
    /// <summary>
    /// Рендер графа на языке Node
    /// </summary>
    public class GraphRender:IGraphConvertible {
        /// <summary>
        /// Символ сдвига уровня при форматировании
        /// </summary>
        public const string LEVEL = "\t";
        /// <summary>
        /// Строка, открывающая блок
        /// </summary>
        public const string OPENBLOCK = " {\r\n";
        /// <summary>
        /// Строка закрывающая блок
        /// </summary>
        public const string CLOSEBLOCK = "}\r\n";
        /// <summary>
        /// Ключевое слово обозначения графа
        /// </summary>
        public const string GRAPHKEYWORD = "digraph ";
        /// <summary>
        /// Ключевой слово обозначения подграфа
        /// </summary>
        public const string SUBGRAPHKEYWORD = "subgraph ";
        /// <summary>
        /// Последовательность закрытия строки
        /// </summary>
        public const string LINECLOSER = ";\r\n";
        /// <summary>
        /// Начало атрибутов для узла/ребра
        /// </summary>
        public const string OPENINLINEATTRIBUTES = " [";
        /// <summary>
        /// Окончание атрибутов для узла/ребра
        /// </summary>
        public const string CLOSEINLINEATTRIBUTES = "]";
        /// <summary>
        /// Символ равенства
        /// </summary>
        public const string EQ = "=";
        /// <summary>
        /// Разделитель атрибутов
        /// </summary>
        public const string ATTRDELIMITER = ";";
        /// <summary>
        /// Символ ребра
        /// </summary>
        public const string EDGE = " -> ";
        private Graph _graph;
        private GraphOptions _parameters;
        private StringBuilder _buffer;
        private int _level;

        private GraphRender(Graph graph, GraphOptions parameters) {
            _graph = graph;
            _parameters = parameters;
        }
        /// <summary>
        /// Создает объект рендера
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IGraphConvertible Create(Graph graph, GraphOptions parameters = null) {
            return new GraphRender(graph, parameters ?? new GraphOptions());
        }

        /// <summary>
        /// Создает объект рендера
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string Render(Graph graph, GraphOptions parameters = null)
        {
            return new GraphRender(graph, parameters ?? new GraphOptions()).GenerateGraphScript(null);
        }
        /// <summary>
        /// Строит скрипт графа на DOT
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateGraphScript(GraphOptions parameters) {
            _buffer = new StringBuilder();
            _level = 0;
            _parameters = parameters ?? _parameters;
            PrepareGraphForRendering();
            OpenGraph();
            WriteGraphBody(_graph);
            CloseGraph();
            return _buffer.ToString();
        }
        /// <summary>
        /// Возвращает построителя для внутреннего графа
        /// </summary>
        /// <returns></returns>
        public IGraphBuilder GetBuilder() {
            
            return _graph.GetBuilder();
        }

        private void WriteGraphBody(SubGraph g) {
            WriteGraphAttributes(g);
            WriteDefaultNode(g);
            WriteDefaultEdge(g);
            foreach (var n in g.Nodes) {
                WriteNode(n);
            }
            foreach (var sg in g.SubGraphs) {
                WriteSubgraph(sg);
            }
            foreach (var edge in g.Edges) {
                WriteEdge(edge);
            }
        }

        private void WriteEdge(Edge edge) {
             WriteLevel();
            _buffer.Append(edge.From);
            _buffer.Append(EDGE);
            _buffer.Append(edge.To);
            WriteInlineAttributes(edge);
            CloseLine();
        }

        private void WriteSubgraph(SubGraph sg) {
            OpenSubgraph(sg);
            WriteGraphBody(sg);
            CloseSubgraph();
        }

        private void OpenSubgraph(SubGraph sg) {
            WriteLevel();
            _buffer.Append(SUBGRAPHKEYWORD);
            _buffer.Append(sg.Code);
            OpenBlock();
        }

        private void CloseSubgraph() {
            CloseBlock();
        }

        private void WriteNode(Node node) {
            WriteLevel();
            _buffer.Append(node.Code);
            WriteInlineAttributes(node);
            CloseLine();
        }

        private void WriteInlineAttributes(GraphElementBase e) {
            if (0 == e.Attributes.Count) return;
            _buffer.Append(OPENINLINEATTRIBUTES);
            foreach (var a in e.Attributes) {
                WriteAttribute(a);
            }
            _buffer.Append(CLOSEINLINEATTRIBUTES);
        }

        private void WriteAttribute(KeyValuePair<string, object> a) {
            _buffer.Append(a.Key);
            _buffer.Append(EQ);
            _buffer.Append(DotLanguageUtils.GetAttributeString(a.Key, a.Value));
            _buffer.Append(ATTRDELIMITER);
        }

        private void CloseLine() {
            _buffer.Append(LINECLOSER);
        }

        private void WriteDefaultEdge(SubGraph graph) {
            if (null == graph.DefaultEdge) return;
            WriteNode(graph.DefaultEdge);
        }

        private void WriteDefaultNode(SubGraph graph)
        {
            if (null == graph.DefaultNode) return;
            WriteNode(graph.DefaultNode);
        }

        private void WriteGraphAttributes(SubGraph graph) {
            foreach (var a in graph.Attributes) {
                WriteLevel();
                _buffer.Append(a.Key);
                _buffer.Append(EQ);
                _buffer.Append(DotLanguageUtils.GetAttributeString(a.Key, a.Value));
                _buffer.Append(LINECLOSER);
            }
        }


        private void CloseBlock() {
            _level--;
            WriteLevel();
            _buffer.Append(CLOSEBLOCK);
           
        }
        private void OpenBlock() {
            _buffer.Append(OPENBLOCK);
            _level++;
        }
        private void WriteLevel() {
            for (var i = 0; i < _level; i++) {
                _buffer.Append(LEVEL);
            }
        }

        private void CloseGraph() {
           CloseBlock();
        }

        private void OpenGraph() {
            _buffer.Append(GRAPHKEYWORD);
            _buffer.Append(_graph.Code);
            OpenBlock();
        }

        private void PrepareGraphForRendering() {
            
            foreach (var p in _parameters.OverrideGraphAttributes) {
                
                if (p.Key.StartsWith("node.")) {
                    ApplyNodeOverride(p);
                }
                else if (p.Key.StartsWith("edge.")) {
                    ApplyEdgeOverride(p);
                }
                else {
                    _graph.OverrideAttribute(p.Key,p.Value);
                }
                
            }
            if (_parameters.Tune) {
                _graph.AutoTune();
            }
        }

        private void ApplyEdgeOverride(KeyValuePair<string, object> p) {
            var edgeattr = p.Key.Split('.');
            if (edgeattr.Length == 2) {
                if (null == _graph.DefaultEdge) {
                    _graph.DefaultEdge = new Node();
                }
                _graph.DefaultEdge.OverrideAttribute(edgeattr[1], p.Value);
            }
            else {
                var targetEdge = _graph.ResolveEdge(edgeattr[1], edgeattr[2]);
                if (null != targetEdge) {
                    targetEdge.OverrideAttribute(edgeattr[3], p.Value);
                }
            }
        }

        private void ApplyNodeOverride(KeyValuePair<string, object> p) {
            var nodeattr = p.Key.Split('.');
            if (nodeattr.Length == 2) {
                if (null == _graph.DefaultNode) {
                    _graph.DefaultNode = new Node();
                }
                _graph.DefaultNode.OverrideAttribute(nodeattr[1], p.Value);
            }
            else {
                var targetNode = _graph.ResolveNode(nodeattr[1]);
                if (null != targetNode) {
                    targetNode.OverrideAttribute(nodeattr[2], p.Value);
                }
            }
        }
    }
}