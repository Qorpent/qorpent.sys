namespace Qorpent.Graphs {
    /// <summary>
    /// ����������� ����� �����
    /// </summary>
    public interface IGraphNodeBuilder : IGraphElementBuilder {
        /// <summary>
        /// ��� �������� ��������
        /// </summary>
        string SubGraphCode { get; set; }
    }
}