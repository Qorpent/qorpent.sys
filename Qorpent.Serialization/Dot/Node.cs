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
                else if (str == "mrecord")
                {
                    str = "Mrecord";
                }                
                Set(DotConstants.ShapeAttribute,str); 
            }
		}
        /// <summary>
        /// Задает угол в градусах для вращения многоугольника формы узла (shape=poligon). Н-р, node [shape=poligon; orientation=30]
        /// </summary>
        public double Orientation
        {
            get { return Get(DotConstants.OrientationAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.OrientationAttribute, str);

            }
        }
	}
}