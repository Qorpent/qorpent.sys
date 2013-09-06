using System.Linq;

namespace Qorpent.Dot
{
    /// <summary>
    /// Полный граф
    /// </summary>
    public class Graph : SubGraph
    {
        /// <summary>
        /// Тут автоматическое наведение порядка по графу
        /// </summary>
        public override void AutoTune()
        {
            CheckIfCompound();
            SetParentsForSubgraphs();
            MoveNodesToSubgraphs();
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

        private void MoveNodesToSubgraphs()
        {
            foreach (var n in Nodes.ToArray())
            {
                if (!string.IsNullOrWhiteSpace(n.SubgraphCode))
                {
                    var sg = ResolveSubgraph(n.SubgraphCode);
                    sg.Nodes.Add(n);
                    n.Subgraph = sg;
                    Nodes.Remove(n);
                }
            }
        }
    }
}