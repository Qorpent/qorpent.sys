namespace Qorpent.Graphs {
    /// <summary>
    /// Построитель подграфов
    /// </summary>
    public interface IGraphSubGraphBuilder : IGraphElementBuilder {
        /// <summary>
        /// Получить доступ к узлу по умолчанию
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetDefaultNode();
        /// <summary>
        /// Получить доступ к ребру по умолчанию
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetDefaultEdge();
        /// <summary>
        /// Создать/получить узел
        /// </summary>
        /// <returns></returns>
        IGraphNodeBuilder GetNode(string code);
        /// <summary>
        /// Создать/получить ребро
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IGraphEdgeBuilder GetEdge(string from, string to);
    }
}