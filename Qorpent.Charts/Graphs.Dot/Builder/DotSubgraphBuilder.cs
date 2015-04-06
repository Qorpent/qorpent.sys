namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// ����������� ���������
    /// </summary>
    public class DotSubgraphBuilder : DotSubgraphBuilderBase<SubGraph>  {
        /// <summary>
        /// ������� ����������� ����������� ���������
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subGraph"></param>
        public DotSubgraphBuilder(IGraphSubGraphBuilder parent, SubGraph subGraph) : base(parent, subGraph) {
        }
    }
}