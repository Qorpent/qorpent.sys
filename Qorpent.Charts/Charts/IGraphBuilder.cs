namespace Qorpent.Graphs {
    /// <summary>
    /// Построитель подграфов
    /// </summary>
    public interface IGraphBuilder : IGraphSubGraphBuilder, IGraphSource, IGraphConvertible
    {
        /// <summary>
        /// Удаляет узел из графа
        /// </summary>
        /// <param name="nodeBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphNodeBuilder nodeBuilder);
        /// <summary>
        /// Удаляет ребро из графа
        /// </summary>
        /// <param name="edgeBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphEdgeBuilder edgeBuilder);
        /// <summary>
        /// Удаляет 
        /// </summary>
        /// <param name="subGraphBuilder"></param>
        /// <returns></returns>
        IGraphBuilder Remove(IGraphSubGraphBuilder subGraphBuilder);

    }
}