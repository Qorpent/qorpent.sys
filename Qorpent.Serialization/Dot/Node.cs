using Qorpent.Utils.Extensions;

namespace Qorpent.Dot {
	/// <summary>
	/// Узел графа
	/// </summary>
	public class Node : GraphElementBase {
		/// <summary>
		/// Форма
		/// </summary>
		public NodeShapeType Shape {
			get { return Get(DotConstants.ShapeAttribute).To<NodeShapeType>(); }
			set { Set(DotConstants.ShapeAttribute,value.ToStr()); }
		}
	}
}