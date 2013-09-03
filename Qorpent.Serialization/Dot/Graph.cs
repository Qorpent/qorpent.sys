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
	}
}
