namespace Qorpent.Graphs {
    /// <summary>
    /// ����������� ����� �����
    /// </summary>
    public interface IGraphEdgeBuilder : IGraphElementBuilder
    {
        /// <summary>
        /// ���������� ��� ��������� ����
        /// </summary>
        /// <returns></returns>
        string GetFrom();
        /// <summary>
        /// ���������� ��� �������� ����
        /// </summary>
        /// <returns></returns>
        string GetTo();
    }
}