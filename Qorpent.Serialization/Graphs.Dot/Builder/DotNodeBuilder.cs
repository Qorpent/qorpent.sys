namespace Qorpent.Graphs.Dot.Builder {
    /// <summary>
    /// Построитель NODE для DOT
    /// </summary>
    public class DotNodeBuilder : DotGraphElementBuilderBase<Node>,IGraphNodeBuilder {
        
        internal DotNodeBuilder(Node node) : base(node) {
        }

        /// <summary>
        /// Код целевого субграфа
        /// </summary>
        public string SubGraphCode {
            get { return Element.SubgraphCode; }
            set { Element.SubgraphCode = value; }
        }
    }
}