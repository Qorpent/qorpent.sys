using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Graphs.Dot.Builder
{
    /// <summary>
    /// Построитель графов
    /// </summary>
    [ContainerComponent(Name="default.graph.builder",ServiceType = typeof(IGraphBuilder),Lifestyle=Lifestyle.Transient)]
    public class DotGraphBuilder : DotSubgraphBuilderBase<Graph>,IGraphBuilder
    {
        /// <summary>
        /// Создает построитель графика DOT
        /// </summary>
        public DotGraphBuilder() : base(null,new Graph()) {}
        /// <summary>
        /// Создает построитель в прицеле на конкретный граф
        /// </summary>
        /// <param name="graph"></param>
        internal DotGraphBuilder(Graph graph) : base(null, graph) { }
        /// <summary>
        /// Возвращает граф, приводимый к скрипту
        /// </summary>
        /// <returns></returns>
        public IGraphConvertible BuildGraph(GraphOptions options) {
            Element.AutoTune();
            return Element;
        }
        /// <summary>
        /// Выполнение пост-обработки SVG с графом
        /// </summary>
        /// <param name="currentSvg"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public XElement PostprocessGraphSvg(XElement currentSvg, GraphOptions options) {
            return currentSvg;
        }

        /// <summary>
        /// Удаляет узел из графа
        /// </summary>
        /// <param name="nodeBuilder"></param>
        /// <returns></returns>
        public IGraphBuilder Remove(IGraphNodeBuilder nodeBuilder) {
            var node = (Node) nodeBuilder.GetNative();
            var parent = node.Parent;
            if (null != parent) {
                parent.Nodes.Remove(node);
            }
            return this;
        }

        /// <summary>
        /// Удаляет ребро из графа
        /// </summary>
        /// <param name="edgeBuilder"></param>
        /// <returns></returns>
        public IGraphBuilder Remove(IGraphEdgeBuilder edgeBuilder) {
            var edge = (Edge)edgeBuilder.GetNative();
            var parent = edge.Parent;
            if (null != parent)
            {
                parent.Edges.Remove(edge);
            }
            return this;
        }

        /// <summary>
        /// Удаляет 
        /// </summary>
        /// <param name="subGraphBuilder"></param>
        /// <returns></returns>
        public IGraphBuilder Remove(IGraphSubGraphBuilder subGraphBuilder) {
            var subgraph = (SubGraph)subGraphBuilder.GetNative();
            var parent = subgraph.Parent;
            if (null != parent)
            {
                parent.SubGraphs.Remove(subgraph);
            }
            return this;
        }
        /// <summary>
        /// Формирует скрипт DOT
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateGraphScript(GraphOptions parameters = null) {
            return GraphRender.Create((Graph)BuildGraph(parameters), parameters).GenerateGraphScript();
        }

        /// <summary>
        /// Возвращает ссылку на самого себя в качестве билдера
        /// </summary>
        /// <returns></returns>
        public IGraphBuilder GetBuilder() {
            return this;
        }
    }
}
