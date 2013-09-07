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
        public override void AutoTune()
        {
            CheckIfCompound();
            SetParentsForSubgraphs();
            MoveNodesToSubgraphs();
            SetParentsForNodesAndEdges();
            base.AutoTune();
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
        /// Перемещает узлы по подграфам
        /// </summary>
        public void MoveNodesToSubgraphs()
        {
            foreach (var n in Nodes.ToArray())
            {
                if (!string.IsNullOrWhiteSpace(n.SubgraphCode))
                {
                    var sg = ResolveSubgraph(n.SubgraphCode);
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