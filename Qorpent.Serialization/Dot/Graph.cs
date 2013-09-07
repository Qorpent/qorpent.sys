using System.Linq;
using Qorpent.Serialization;

namespace Qorpent.Dot
{
    /// <summary>
    /// Полный граф
    /// </summary>
    public class Graph : SubGraph, IGraphConvertible
    {
        /// <summary>
        /// Тут автоматическое наведение порядка по графу
        /// </summary>
        public override void AutoTune() {
            CorrectClusterCodes();
            CheckIfCompound();
            SetParentsForSubgraphs();
            MoveNodesToSubgraphs();
            SetParentsForNodesAndEdges();
            base.AutoTune();
        }

        private void CorrectClusterCodes() {
           foreach (var c in EnumerateSubGraphs()) {
               c.Code = DotLanguageUtils.GetClusterCode(c.Code);
           }
            foreach (var n in EnumerateNodes().Where(_=>!string.IsNullOrWhiteSpace(_.SubgraphCode))) {
                n.SubgraphCode = DotLanguageUtils.GetClusterCode(n.SubgraphCode);
            }
        }

        private void CheckIfCompound() {
            if (EnumerateEdges().Any(e =>
                          e.HasAttribute(DotConstants.LheadAttribute)
                          || e.HasAttribute(DotConstants.LtailAttribute))) {
                if (!HasAttribute(DotConstants.CompoundAttribute)) {
                    Compound = true;
                }
            }
        }
        /// <summary>
        /// Добавляет узел к графу
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        public static Graph operator +(Graph graph, Edge edge)
        {
            graph.AddEdge(edge);
            return graph;
        }

        /// <summary>
        /// Добавляет узел к графу
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Graph operator +(Graph graph, Node node)
        {
            graph.AddNode(node);
            return graph;
        }
        
        /// <summary>
        /// Акцессор по нодам
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Node this[string code] {
            get { return ResolveNode(code); }
        }
        /// <summary>
        /// Акцессор по узлам
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Edge this[string from,string to,string type=null] {
            get { return ResolveEdge(from, to, type); }
        }
        

        /// <summary>
        /// Перемещает узлы по подграфам
        /// </summary>
        public void MoveNodesToSubgraphs()
        {
            foreach (var n in Nodes.ToArray())
            {
                if (!string.IsNullOrWhiteSpace(n.SubgraphCode))
                {
                    var sg = ResolveSubgraph(n.SubgraphCode);
                    if (null == sg) {
                        sg = new SubGraph {Code = n.SubgraphCode, Parent = this};
                        SubGraphs.Add(sg);
                    }
                    sg.Nodes.Add(n);
                    n.Parent = sg;
                    Nodes.Remove(n);
                }
            }
        }

        /// <summary>
        /// Возвращает скрипт графа на целевом языке
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string GenerateGraphScript(GraphOptions parameters) {
            var render = GraphRender.Create(this,parameters);
            return render.GenerateGraphScript(parameters);
        }
    }
}