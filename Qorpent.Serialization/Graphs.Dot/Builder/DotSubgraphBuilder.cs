namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// Построитель субграфов
    /// </summary>
    public class DotSubgraphBuilder : DotSubgraphBuilderBase<SubGraph>  {
        /// <summary>
        /// Создает стандартный построитель субграфов
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="subGraph"></param>
        public DotSubgraphBuilder(IGraphSubGraphBuilder parent, SubGraph subGraph) : base(parent, subGraph) {
        }
    }
}