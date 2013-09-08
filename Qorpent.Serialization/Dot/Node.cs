using System;
using Qorpent.Serialization;

namespace Qorpent.Dot {
	/// <summary>
	/// Узел графа
	/// </summary>
	public class Node : GraphElementBase {
	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public NodeShapeType Shape {
	        get { return Get<NodeShapeType>(DotConstants.ShapeAttribute); }
	        set { Set(DotConstants.ShapeAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public NodeStyleType Style {
	        get { return Get<NodeStyleType>(DotConstants.StyleAttribute); }
	        set { Set(DotConstants.StyleAttribute, value); }
	    }

	    /// <summary>
        /// Целевой подграф
        /// </summary>
       [IgnoreSerialize]
        public string SubgraphCode { get; set; }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Orientation {
	        get { return Get<double>(DotConstants.OrientationAttribute); }
	        set { Set(DotConstants.OrientationAttribute, value); }
	    }


	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Distortion {
	        get { return Get<double>(DotConstants.DistortionAttribute); }
	        set { Set(DotConstants.DistortionAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public bool FixedSize {
	        get { return Get<bool>(DotConstants.FixedSizeAttribute); }
	        set { Set(DotConstants.FixedSizeAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Height {
	        get { return Get<double>(DotConstants.HeightAttribute); }
	        set { Set(DotConstants.HeightAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Width {
	        get { return Get<double>(DotConstants.WidthAttribute); }
	        set { Set(DotConstants.WidthAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Skew {
	        get { return Get<double>(DotConstants.SkewAttribute); }
	        set { Set(DotConstants.SkewAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public int Peripheries {
	        get { return Get<int>(DotConstants.PeripheriesAttribute); }
	        set { Set(DotConstants.PeripheriesAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double Sides {
	        get { return Get<double>(DotConstants.SidesAttribute); }
	        set { Set(DotConstants.SidesAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string XLabe {
	        get { return Get<string>(DotConstants.XLabeAttribute); }
	        set { Set(DotConstants.XLabeAttribute, value); }
	    }

	    /// <summary>
        /// Содерджаший узел граф
        /// </summary>
	   [IgnoreSerialize]
        public SubGraph Parent { get; set; }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public ColorAttribute FillColor {
	        get { return Get<ColorAttribute>(DotConstants.FillColorAttribute); }
	        set { Set(DotConstants.FillColorAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string Tooltip {
	        get { return Get<string>(DotConstants.TooltipAttribute); }
	        set { Set(DotConstants.TooltipAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string Target {
	        get { return Get<string>(DotConstants.TargetAttribute); }
	        set { Set(DotConstants.TargetAttribute, value); }
	    }

	    /// <summary>
	    ///
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