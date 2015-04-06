namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// ����������� NODE ��� DOT
    /// </summary>
    public class DotNodeBuilder : DotGraphElementBuilderBase<Node>,IGraphNodeBuilder {
        
        internal DotNodeBuilder(Node node) : base(node) {
        }

        /// <summary>
        /// ��� �������� ��������
        /// </summary>
        public string SubGraphCode {
            get { return Element.SubgraphCode; }
            set { Element.SubgraphCode = value; }
        }
    }
}