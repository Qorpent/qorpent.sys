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
        /// <summary>
        /// Задает ширину линии (стрелки, узла, кластера...) в точках 
        /// </summary>
        public double Penwidth
        {
            get { return Get(DotConstants.OrientationAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.PenwidthAttribute, str);

            }
        }
        /// <summary>
        /// Коэффициент для shape=polygon задающий при положительном значении верхнюю часть больше, чем нижнюю и при отрицательном - наоборот 
        /// </summary>
        public double Distortion
        {
            get { return Get(DotConstants.DistortionAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.DistortionAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да", то размер узла определяется значениями ширины и высоты - Width and Height 
        /// </summary>
        public bool FixedSize
        {
            get { return Get(DotConstants.FixedSizeAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.FixedSizeAttribute, str);

            }
        }
        /// <summary>
        /// Задает высоту узла. По умолчанию 0.5,  min значение 0.02 
        /// </summary>
        public double Height
        {
            get { return Get(DotConstants.HeightAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.HeightAttribute, str);

            }
        }
        /// <summary>
        /// Задает ширину узла. По умолчанию 0.75,  min значение 0.01 
        /// </summary>
        public double Width
        {
            get { return Get(DotConstants.WidthAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.WidthAttribute, str);

            }
        }
        /// <summary>
        /// Для Shape=polygon задает косые. Положительные значения - верхняя части полигона направо, отрицательные  - налево. По умолчанию 0.0   
        /// </summary>
        public double Skew
        {
            get { return Get(DotConstants.SkewAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.SkewAttribute, str);

            }
        }
        /// <summary>
        /// Задает колличество рамочек вокруг узла той же формы что и форма узла  
        /// </summary>
        public int Peripheries
        {
            get { return Get(DotConstants.PeripheriesAttribute).To<int>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.PeripheriesAttribute, str);

            }
        }
        /// <summary>
        /// Задает количество сторон для полигона 
        /// </summary>
        public double Sides
        {
            get { return Get(DotConstants.SidesAttribute).To<double>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.SidesAttribute, str);

            }
        }
        
         
	}
}