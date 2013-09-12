using System.Linq;
using Qorpent.Graphs.Dot.Builder;
using Qorpent.Serialization;

namespace Qorpent.Graphs.Dot
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
        /// <summary>
        /// Возвращает билдера, привязанного к данному графу
        /// </summary>
        /// <returns></returns>
        public IGraphBuilder GetBuilder() {
            return new DotGraphBuilder(this);
        }

        /// <summary>
        /// Разделитель по развитию рангов в долях от ранга
        /// </summary>
        [IgnoreSerialize]
        public double RankSep {
            get { return Get<double>(DotConstants.RankSepAttribute); }
            set { Set(DotConstants.RankSepAttribute, value); }
        }

        /// <summary>
        /// Разделитель по развитию рангов в долях от ранга
        /// </summary>
        [IgnoreSerialize]
        public double NodeSep {
            get { return Get<double>(DotConstants.NodeSepAttribute); }
            set { Set(DotConstants.NodeSepAttribute, value); }
        }

        /// <summary>
        /// зазор при оверлапе
        /// </summary>
        [IgnoreSerialize]
        public double Sep {
            get { return Get<double>(DotConstants.SepAttribute); }
            set { Set(DotConstants.SepAttribute, value); }
        }

        /// <summary>
        /// Расстояния между  ребрами
        /// </summary>
        [IgnoreSerialize]
        public double ESep {
            get { return Get<double>(DotConstants.ESepAttribute); }
            set { Set(DotConstants.ESepAttribute, value); }
        }

        /// <summary>
        ///     Если "Да", то позволяет стрелкам идти между подграфами (См. Lhead Ltail )
        /// </summary>
        [IgnoreSerialize]
        public bool Compound {
            get { return Get<bool>(DotConstants.CompoundAttribute); }
            set { Set(DotConstants.CompoundAttribute, value); }
        }

        /// <summary>
        ///     Сконцентрированное расположение узлов и подграфов графа
        /// </summary>
        [IgnoreSerialize]
        public bool Concentrate {
            get { return Get<bool>(DotConstants.ConcentrateAttribute); }
            set { Set(DotConstants.ConcentrateAttribute, value); }
        }

        /// <summary>
        ///     Если "Да", то график отображается в ландшафтном режиме, т.е. переварачивается на 90 градусов
        /// </summary>
        [IgnoreSerialize]
        public bool Landscape {
            get { return Get<bool>(DotConstants.LandscapeAttribute); }
            set { Set(DotConstants.LandscapeAttribute, value); }
        }

        /// <summary>
        ///     Разварачивает граф на альбомный лист. В значении д.б. указана любая английская Л - l*, L* или полностью Landscape Н-р, orientation="Landscape"
        /// </summary>
        [IgnoreSerialize]
        public string Orientation {
            get { return Get<string>(DotConstants.OrientationAttribute); }
            set { Set(DotConstants.OrientationAttribute, value); }
        }

        /// <summary>
        ///     Квантум увеличивает размеры узла графа на указанное значение. Н-р, quantum=0.5
        /// </summary>
        [IgnoreSerialize]
        public double Quantum {
            get { return Get<double>(DotConstants.QuantumAttribute); }
            set { Set(DotConstants.QuantumAttribute, value); }
        }

        /// <summary>
        ///     Если "Да" и есть несколько графов, то ?????
        /// </summary>
        [IgnoreSerialize]
        public bool Remincross {
            get { return Get<bool>(DotConstants.RemincrossAttribute); }
            set { Set(DotConstants.RemincrossAttribute, value); }
        }

        /// <summary>
        ///     Если установить значение 90, то произойдет переориентации графа в альбомную страницу
        /// </summary>
        [IgnoreSerialize]
        public int Rotate {
            get { return Get<int>(DotConstants.RotateAttribute); }
            set { Set(DotConstants.RotateAttribute, value); }
        }

        /// <summary>
        ///     Задает тип разрисовки линий графа (дуги, ломанные и т.п.)
        /// </summary>
        [IgnoreSerialize]
        public string Splines {
            get { return Get<string>(DotConstants.SplinesAttribute); }
            set { Set(DotConstants.SplinesAttribute, value); }
        }

        /// <summary>
        /// Устанавливает настройки для компактного отображения
        /// </summary>
        public void Compactize() {
            FontSize = 9;
            RankSep = 0.3;
            NodeSep = 0.2;
            Sep = 0.2;
            Quantum = 0.2;
            ESep = 0.2;
        }
    }
}