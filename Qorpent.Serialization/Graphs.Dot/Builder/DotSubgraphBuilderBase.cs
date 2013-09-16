namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// ����������� NODE ��� DOT
    /// </summary>
    public abstract class DotSubgraphBuilderBase<TSubGraph> : DotGraphElementBuilderBase<TSubGraph>, IGraphSubGraphBuilder where TSubGraph:SubGraph
    {
        private readonly IGraphSubGraphBuilder _parent;

        /// <summary>
        /// ������� ������ �������� � ��������� ������ � ������������ ������
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subGraph"></param>
        internal DotSubgraphBuilderBase(IGraphSubGraphBuilder parent, TSubGraph subGraph)
            : base(subGraph) {
            _parent = parent;
        }

        /// <summary>
        /// �������� ������ � ���� �� ���������
        /// </summary>
        /// <returns></returns>
        public IGraphNodeBuilder GetDefaultNode() {
            if (Element.DefaultNode == null) {
                Element.DefaultNode = new Node();
            }
            return new DotNodeBuilder(Element.DefaultNode);
        }

        /// <summary>
        /// �������� ������ � ����� �� ���������
        /// </summary>
        /// <returns></returns>
        public IGraphNodeBuilder GetDefaultEdge() {
            if (Element.DefaultEdge == null)
            {
                Element.DefaultEdge = new Node();
            }
            return new DotNodeBuilder(Element.DefaultEdge);
        }

        /// <summary>
        /// �������/�������� ���� �� ����
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public IGraphNodeBuilder GetNode(string code)
        {
            if (null != _parent) {
                return _parent.GetNode(code);
            }
            var existed = Element.ResolveNode(code);
            if (null == existed)
            {
                existed = new Node { Code = code };
                Element.AddNode(existed);
            }
            return new DotNodeBuilder(existed);
        }


        /// <summary>
        /// �������/�������� �����
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IGraphEdgeBuilder GetEdge(string from, string to)
        {
            if (null != _parent)
            {
                return _parent.GetEdge(from,to);
            }
            var existed = Element.ResolveEdge(from, to);
            if (null == existed)
            {
                existed = new Edge { From = from, To = to };
                Element.AddEdge(existed);
            }
            return new DotEdgeBuilder(existed);
        }

        /// <summary>
        /// �������/�������� �����
        /// </summary>
        /// <returns></returns>
        public IGraphSubGraphBuilder GetSubgraph(string code) {
            var existed = Element.ResolveSubgraph(code);
            if (null == existed) {
                existed = new SubGraph {Code = code};
                Element.AddSubGraph(existed);
            }
            return new DotSubgraphBuilder(this,existed);
        }

    }
}