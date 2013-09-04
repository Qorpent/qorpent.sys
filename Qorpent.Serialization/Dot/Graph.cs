using System.Collections.Generic;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dot
{
	/// <summary>
	/// Полный граф DOT
	/// </summary>
	public class Graph : GraphElementBase
	{
		/// <summary>
		/// 
		/// </summary>
		public Graph() {
			Nodes = new List<Node>();
			Edges = new List<Edge>();
		}
		/// <summary>
		/// Узлы
		/// </summary>
		public IList<Node> Nodes { get; private set; }
		/// <summary>
		/// Ребра
		/// </summary>
		public IList<Edge> Edges { get; private set; }	

		/// <summary>
		/// Направление графа
		/// </summary>
		public RankDirType RankDir {
			get { return Get(DotConstants.RankDirAttribute).To<RankDirType>(); }
			set { Set(DotConstants.RankDirAttribute, value.ToStr()); }
		}

        /// <summary>
        /// Если "Да", то расположение графа центировано, а если "Нет", то не центировано
        /// </summary>
        public bool Center
        {
            get { return Get(DotConstants.CenterAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.CenterAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да", то позволяет стрелкам идти между подграфами (См. Lhead Ltail )
        /// </summary>
        public bool Compound
        {
            get { return Get(DotConstants.CompoundAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.CompoundAttribute, str);

            }
        }
    
	}
}
