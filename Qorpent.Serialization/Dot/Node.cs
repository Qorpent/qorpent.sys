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
			set {
                var str = value.ToStr().ToLower();
                if (str == "mcircle")
                {
                    str = "Mcircle";
                }
                else if (str == "mdiamond")
                {
                    str = "Mdiamond";
                }
                else if (str == "msquare")
                {
                    str = "Msquare";
                }
                else if (str == "Mrecord")
                {
                    str = "mrecord";
                }                
                Set(DotConstants.ShapeAttribute,str); 
            }
		}
	}
}