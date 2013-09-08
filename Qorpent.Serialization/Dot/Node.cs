using System;
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
                
                Set(DotConstants.ShapeAttribute,value); 
            }
		}

        /// <summary>
        /// Стиль
        /// </summary>
	    public NodeStyleType Style {
	        get { return Get<NodeStyleType>(DotConstants.StyleAttribute); }
            set { Set(DotConstants.StyleAttribute,value);}
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
	    public SubGraph Parent { get; set; }
        /// <summary>
        /// Цвет заливки
        /// </summary>
        public ColorAttribute FillColor
        {
            get { return Get<ColorAttribute>(DotConstants.FillColorAttribute); }
	        set { Set(DotConstants.FillColorAttribute,value);}
	    }
        /// <summary>
        /// Всплывающа подсказка
        /// </summary>
	    public string Tooltip {
            get { return Get<string>(DotConstants.TooltipAttribute); }
            set { Set(DotConstants.TooltipAttribute, value); }
	    }
        /// <summary>
        /// Целевой фрейм
        /// </summary>
	    public string Target {
            get { return Get<string>(DotConstants.TargetAttribute); }
            set { Set(DotConstants.TargetAttribute, value); }
	    }
        /// <summary>
        /// Целевой фрейм
        /// </summary>
        public string Href
        {
            get { return Get<string>(DotConstants.HrefAttribute); }
            set { Set(DotConstants.HrefAttribute, value); }
        }

	    /// <summary>
	    /// Создает типовой узел
	    /// </summary>
	    /// <param name="code"></param>
	    /// <param name="label"></param>
	    /// <param name="data"></param>
	    /// <returns></returns>
	    public static Node Create(string code,string label=null, object data=null) {
	        return new Node {Code = code, Label = label ?? code,Data = data};
	    }

        /// <summary>
        /// Создает типовой узел
        /// </summary>
        /// <returns></returns>
        public static Node Create(string code,Action<Node> setup)
        {
            var result = new Node{Code=code};
            if (null != setup) {
                setup(result);
            }
            return result;
        }
	}
}