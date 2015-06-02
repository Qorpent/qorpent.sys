namespace Qorpent.Graphs {
    /// <summary>
    /// Построитель узлов графа
    /// </summary>
    public interface IGraphNodeBuilder : IGraphElementBuilder {
        /// <summary>
        /// Код целевого субграфа
        /// </summary>
        string SubGraphCode { get; set; }
    }
}