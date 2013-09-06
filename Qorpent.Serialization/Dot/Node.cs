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
			get { return Get<NodeShapeType>(DotConstants.ShapeAttribute); }
			set {
                /* //TODO move case
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
                 */
                Set(DotConstants.ShapeAttribute,value); 
            }
		}
        /// <summary>
        /// Целевой подграф
        /// </summary>
        public string SubgraphCode { get; set; }
        /// <summary>
        /// Задает угол в градусах для вращения многоугольника формы узла (shape=poligon). Н-р, node [shape=poligon; orientation=30]
        /// </summary>
        public double Orientation
        {
            get { return Get<double>(DotConstants.OrientationAttribute); }
            set
            {
                Set(DotConstants.OrientationAttribute, value);
            }
        }
        /// <summary>
        /// Задает ширину линии (стрелки, узла, кластера...) в точках 
        /// </summary>
        public double Penwidth
        {
            get { return Get<double>(DotConstants.OrientationAttribute); }
            set
            {
                Set(DotConstants.PenwidthAttribute, value);
            }
        }
        /// <summary>
        /// Коэффициент для shape=polygon задающий при положительном значении верхнюю часть больше, чем нижнюю и при отрицательном - наоборот 
        /// </summary>
        public double Distortion
        {
            get { return Get<double>(DotConstants.DistortionAttribute); }
            set
            {
                Set(DotConstants.DistortionAttribute, value);
            }
        }
        /// <summary>
        /// Если "Да", то размер узла определяется значениями ширины и высоты - Width and Height 
        /// </summary>
        public bool FixedSize
        {
            get { return Get<bool>(DotConstants.FixedSizeAttribute); }
            set
            {
                Set(DotConstants.FixedSizeAttribute, value);
            }
        }
        /// <summary>
        /// Задает высоту узла. По умолчанию 0.5,  min значение 0.02 
        /// </summary>
        public double Height
        {
            get { return Get<double>(DotConstants.HeightAttribute); }
            set
            {
                Set(DotConstants.HeightAttribute, value);
            }
        }
        /// <summary>
        /// Задает ширину узла. По умолчанию 0.75,  min значение 0.01 
        /// </summary>
        public double Width
        {
            get { return Get<double>(DotConstants.WidthAttribute); }
            set
            {
                Set(DotConstants.WidthAttribute, value);
            }
        }
        /// <summary>
        /// Для Shape=polygon задает косые. Положительные значения - верхняя части полигона направо, отрицательные  - налево. По умолчанию 0.0   
        /// </summary>
        public double Skew
        {
            get { return Get<double>(DotConstants.SkewAttribute); }
            set
            {
                Set(DotConstants.SkewAttribute, value);
            }
        }
        /// <summary>
        /// Задает колличество рамочек вокруг узла той же формы что и форма узла  
        /// </summary>
        public int Peripheries
        {
            get { return Get<int>(DotConstants.PeripheriesAttribute); }
            set
            {
                Set(DotConstants.PeripheriesAttribute, value);
            }
        }
        /// <summary>
        /// Задает количество сторон для полигона 
        /// </summary>
        public double Sides
        {
            get { return Get<double>(DotConstants.SidesAttribute); }
            set
            {
                Set(DotConstants.SidesAttribute, value);
            }
        }
        /// <summary>
        /// Внешняя метка для узла или края. Для узлов, названия будут помещаться вне узла, но рядом с ним и для ребра - вблизи центра ребра. 
        /// </summary>
        public string XLabe
        {
            get { return Get<string>(DotConstants.XLabeAttribute); }
            set
            {
                Set(DotConstants.XLabeAttribute, value);
            }
        }
        /// <summary>
        /// Содерджаший узел граф
        /// </summary>
	    public SubGraph Subgraph { get; set; }
	}
}