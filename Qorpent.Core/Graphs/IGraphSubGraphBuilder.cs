namespace Qorpent.Graphs {
    /// <summary>
    /// ����������� ���������
    /// </summary>
    public interface IGraphSubGraphBuilder : IGraphElementBuilder {
        /// <summary>
        /// �������� ������ � ���� �� ���������
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetDefaultNode();
        /// <summary>
        /// �������� ������ � ����� �� ���������
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetDefaultEdge();
        /// <summary>
        /// �������/�������� ����
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetNode(string code);
        /// <summary>
        /// �������/�������� �����
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IGraphEdgeBuilder GetEdge(string from, string to);
    }
}