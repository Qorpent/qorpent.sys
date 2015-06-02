namespace Qorpent.Graphs {
    /// <summary>
    /// Построитель ребер графа
    /// </summary>
    public interface IGraphEdgeBuilder : IGraphElementBuilder
    {
        /// <summary>
        /// Возвращает код исходного узла
        /// </summary>
        /// <returns></returns>
        string GetFrom();
        /// <summary>
        /// Возвращает код целевого узла
        /// </summary>
        /// <returns></returns>
        string GetTo();
    }
}