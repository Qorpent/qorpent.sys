using System.Text;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot {
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

        public const string GRAPHKEYWORD = "digraph ";
        public const string SUBGRAPHOPEN = "subgraph ";
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
        public static IGraphConvertible Create(Graph graph, GraphOptions parameters) {
            return new GraphRender(graph, parameters);
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
            throw new System.NotImplementedException();
        }

        private void WriteSubgraph(SubGraph sg) {
            OpenSubgraph(sg);
            WriteGraphBody(sg);
            CloseSubgraph();
        }

        private void OpenSubgraph(SubGraph sg) {
            WriteLevel();
            _buffer.Append(SUBGRAPHOPEN);
            _buffer.Append(sg.Code);
            _level++;
        }

        private void CloseSubgraph() {
            CloseBlock();
        }

        private void WriteNode(Node node) {
            throw new System.NotImplementedException();
        }

        private void WriteDefaultEdge(SubGraph graph)
        {
            throw new System.NotImplementedException();
        }

        private void WriteDefaultNode(SubGraph graph)
        {
            throw new System.NotImplementedException();
        }

        private void WriteGraphAttributes(SubGraph graph) {
            throw new System.NotImplementedException();
        }


        private void CloseBlock() {
            WriteLevel();
            _buffer.Append(CLOSEBLOCK);
            _level--;
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
                var newval = p.Value;
                if (_graph.Attributes.ContainsKey(p.Key)) {
                    var val = _graph.Attributes[p.Key];
                    var type = null == val ? typeof (string) : val.GetType();
                    newval = val.ToTargetType(type);
                }
                _graph.Attributes[p.Key] = newval;
            }
            if (_parameters.Tune) {
                _graph.AutoTune();
            }
        }
    }
}