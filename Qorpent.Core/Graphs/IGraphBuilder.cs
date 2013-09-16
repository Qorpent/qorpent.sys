namespace Qorpent.Graphs {
    /// <summary>
    /// ����������� ���������
    /// </summary>
    public interface IGraphBuilder : IGraphSubGraphBuilder, IGraphSource, IGraphConvertible
    {
        /// <summary>
        /// ������� ���� �� �����
        /// </summary>
        /// <param name="nodeBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphNodeBuilder nodeBuilder);
        /// <summary>
        /// ������� ����� �� �����
        /// </summary>
        /// <param name="edgeBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphEdgeBuilder edgeBuilder);
        /// <summary>
        /// ������� 
        /// </summary>
        /// <param name="subGraphBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphSubGraphBuilder subGraphBuilder);

    }
}