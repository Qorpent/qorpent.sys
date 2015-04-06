namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// ����������� NODE ��� DOT
    /// </summary>
    public class DotEdgeBuilder : DotGraphElementBuilderBase<Edge>, IGraphEdgeBuilder
    {
        /// <summary>
        /// ������� ������, ���������� �� ������������ �����
        /// </summary>
        /// <param name="edge"></param>
        internal DotEdgeBuilder(Edge edge)
            : base(edge)
        {
        }

        /// <summary>
        /// ���������� ��� ��������� ����
        /// </summary>
        /// <returns></returns>
        public string GetFrom() {
            return Element.From;
        }

        /// <summary>
        /// ���������� ��� �������� ����
        /// </summary>
        /// <returns></returns>
        public string GetTo() {
            return Element.To;
        }
    }
}