using System;
using Qorpent.Graphs.Dot.Types;
using Qorpent.Serialization;

namespace Qorpent.Graphs.Dot {
	/// <summary>
	/// Узел графа
	/// </summary>
	public class Node : GraphElementBase {
	    /// <summary>
        /// Атрибут формы узла
	    /// </summary>
	    [IgnoreSerialize]
	    public NodeShapeType Shape {
	        get { return Get<NodeShapeType>(DotConstants.ShapeAttribute); }
	        set { Set(DotConstants.ShapeAttribute, value); }
	    }
        /// <summary>
        ///     Уже посчитан
        /// </summary>
        public bool Counted { get; set; }
        /// <summary>
        ///     Вес узла
        /// </summary>
        public int Weight { get; set; }

		/// <summary>
		/// Прямой входящий вес (только непосредственные связи)
		/// </summary>
		public int DirectInputWeight { get; set; }
	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public NodeStyleType Style {
	        get { return Get<NodeStyleType>(DotConstants.StyleAttribute); }
	        set { Set(DotConstants.StyleAttribute, value); }
	    }
		/// <summary>
		/// Задает ширину линии (стрелки, узла, кластера...) в точках 
		/// </summary>
		[IgnoreSerialize]
		public double Penwidth
		{
			get { return Get<double>(DotConstants.PenwidthAttribute); }
			set
			{
				Set(DotConstants.PenwidthAttribute, value);
			}
		}

	    /// <summary>
        /// Целевой подграф
        /// </summary>
       [IgnoreSerialize]
        public string SubgraphCode { get; set; }

	    /// <summary>
       /// Для graph и node
	    /// </summary>
	    [IgnoreSerialize]
	    public double Orientation {
	        get { return Get<double>(DotConstants.OrientationAttribute); }
	        set { Set(DotConstants.OrientationAttribute, value); }
	    }


	    /// <summary>
        /// Коэффициент для shape=polygon задающий при положительном значении верхнюю часть больше, чем нижнюю и при отрицательном - наоборот
	    /// </summary>
	    [IgnoreSerialize]
	    public double Distortion {
	        get { return Get<double>(DotConstants.DistortionAttribute); }
	        set { Set(DotConstants.DistortionAttribute, value); }
	    }

	    /// <summary>
        /// Если "Да", то размер узла определяется значениями ширины и высоты - Width and Height
	    /// </summary>
	    [IgnoreSerialize]
	    public bool FixedSize {
	        get { return Get<bool>(DotConstants.FixedSizeAttribute); }
	        set { Set(DotConstants.FixedSizeAttribute, value); }
	    }

	    /// <summary>
        /// Задает высоту узла. По умолчанию 0.5,  min значение 0.02
	    /// </summary>
	    [IgnoreSerialize]
	    public double Height {
	        get { return Get<double>(DotConstants.HeightAttribute); }
	        set { Set(DotConstants.HeightAttribute, value); }
	    }

	    /// <summary>
        /// Задает ширину узла. По умолчанию 0.75,  min значение 0.01
	    /// </summary>
	    [IgnoreSerialize]
	    public double Width {
	        get { return Get<double>(DotConstants.WidthAttribute); }
	        set { Set(DotConstants.WidthAttribute, value); }
	    }

	    /// <summary>
        /// Для Shape=polygon задает косые. Положительные значения - верхняя части полигона направо, отрицательные  - налево. По умолчанию 0.0
	    /// </summary>
	    [IgnoreSerialize]
	    public double Skew {
	        get { return Get<double>(DotConstants.SkewAttribute); }
	        set { Set(DotConstants.SkewAttribute, value); }
	    }

	    /// <summary>
        /// Задает колличество рамочек вокруг узла той же формы что и форма узла
	    /// </summary>
	    [IgnoreSerialize]
	    public int Peripheries {
	        get { return Get<int>(DotConstants.PeripheriesAttribute); }
	        set { Set(DotConstants.PeripheriesAttribute, value); }
	    }

	    /// <summary>
        /// Задает количество сторон для полигона
	    /// </summary>
	    [IgnoreSerialize]
	    public double Sides {
	        get { return Get<double>(DotConstants.SidesAttribute); }
	        set { Set(DotConstants.SidesAttribute, value); }
	    }

	    /// <summary>
        /// Внешняя метка для узла или края. Для узлов, названия будут помещаться вне узла, но рядом с ним и для ребра - вблизи центра ребра.
	    /// </summary>
	    [IgnoreSerialize]
	    public string XLabe {
	        get { return Get<string>(DotConstants.XLabelAttribute); }
	        set { Set(DotConstants.XLabelAttribute, value); }
	    }

	    /// <summary>
        /// Содерджаший узел граф
        /// </summary>
	   [IgnoreSerialize]
        public SubGraph Parent { get; set; }

	    /// <summary>
       /// Цвет заливки
	    /// </summary>
	    [IgnoreSerialize]
	    public ColorAttribute FillColor {
	        get { return Get<ColorAttribute>(DotConstants.FillColorAttribute); }
	        set { Set(DotConstants.FillColorAttribute, value); }
	    }

	    /// <summary>
        /// Подсказка
	    /// </summary>
	    [IgnoreSerialize]
	    public string Tooltip {
	        get { return Get<string>(DotConstants.TooltipAttribute); }
	        set { Set(DotConstants.TooltipAttribute, value); }
	    }

	    /// <summary>
        /// Целевой фрейм
	    /// </summary>
	    [IgnoreSerialize]
	    public string Target {
	        get { return Get<string>(DotConstants.TargetAttribute); }
	        set { Set(DotConstants.TargetAttribute, value); }
	    }

	    /// <summary>
        /// Ссылка
	    /// </summary>
	    [IgnoreSerialize]
	    public string Href {
	        get { return Get<string>(DotConstants.HrefAttribute); }
	        set { Set(DotConstants.HrefAttribute, value); }
	    }
        /// <summary>
        /// Цвет линии
        /// </summary>
        [IgnoreSerialize]
	    public ColorAttribute Color {
            get { return Get<ColorAttribute>(DotConstants.ColorAttribute); }
            set { Set(DotConstants.ColorAttribute, value); }
	    }
		/// <summary>
		/// Прямой исходящий вес
		/// </summary>
		public int DirectOutputWeight { get; set; }

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