namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// Построитель NODE для DOT
    /// </summary>
    public class DotEdgeBuilder : DotGraphElementBuilderBase<Edge>, IGraphEdgeBuilder
    {
        /// <summary>
        /// Создает билдер, нацеленный на определенное ребро
        /// </summary>
        /// <param name="edge"></param>
        internal DotEdgeBuilder(Edge edge)
            : base(edge)
        {
        }

        /// <summary>
        /// Возвращает код исходного узла
        /// </summary>
        /// <returns></returns>
        public string GetFrom() {
            return Element.From;
        }

        /// <summary>
        /// Возвращает код целевого узла
        /// </summary>
        /// <returns></returns>
        public string GetTo() {
            return Element.To;
        }
    }
}