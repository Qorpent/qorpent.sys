using System;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;
namespace Qorpent.Dot {
	/// <summary>
	/// Ребро графа
	/// </summary>
	public class Edge : GraphElementBase {
	    private string _from;
	    private string _to;

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public Arrow ArrowHead {
	        get { return Get<Arrow>(DotConstants.ArrowHeadAttribute); }
	        set { Set(DotConstants.ArrowHeadAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public Arrow ArrowTail {
	        get { return Get<Arrow>(DotConstants.ArrowTailAttribute); }
	        set { Set(DotConstants.ArrowTailAttribute, value); }
	    }

	    /// <summary>
        /// Тип ребра
        /// </summary>
        public string Type { get; set; }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public bool DecorateLabel {
	        get { return Get<bool>(DotConstants.DecorateLabelAttribute); }
	        set { Set(DotConstants.DecorateLabelAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public bool LabelFloat {
	        get { return Get<bool>(DotConstants.LabelFloatAttribute); }
	        set { Set(DotConstants.LabelFloatAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string Lhead {
	        get { return Get<string>(DotConstants.LheadAttribute); }
	        set { Set(DotConstants.LheadAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string Ltail {
	        get { return Get<string>(DotConstants.LtailAttribute); }
	        set { Set(DotConstants.LtailAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string SameHead {
	        get { return Get<string>(DotConstants.SameHeadAttribute); }
	        set { Set(DotConstants.SameHeadAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public string SameTail {
	        get { return Get<string>(DotConstants.SameTailAttribute); }
	        set { Set(DotConstants.SameTailAttribute, value); }
	    }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public DirType Dir {
	        get { return Get<DirType>(DotConstants.DirAttribute); }
	        set { Set(DotConstants.DirAttribute, value); }
	    }

	    /// <summary>
        /// Задает ширину линии (стрелки, узла, кластера...) в точках 
        /// </summary>
         [IgnoreSerialize]
        public double Penwidth
        {
            get { return Get<double>(DotConstants.OrientationAttribute); }
            set
            {
                Set(DotConstants.PenwidthAttribute, value);
            }
        }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public double ArrowSize {
	        get { return Get<double>(DotConstants.ArrowSizeAttribute); }
	        set { Set(DotConstants.ArrowSizeAttribute, value); }
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
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public int Minlen {
	        get { return Get<int>(DotConstants.MinlenAttribute); }
	        set { Set(DotConstants.MinlenAttribute, value); }
	    }

	    /// <summary>
        /// Код входящего узла
        /// </summary>
	    public string From {
            get { return string.IsNullOrWhiteSpace(_from)?(_from=DotLanguageUtils.NULLCODE):_from; }
            set { _from = DotLanguageUtils.EscapeCode(value); }
        }

	    /// <summary>
        /// Код исходящего узла
        /// </summary>
        public string To {
            get { return string.IsNullOrWhiteSpace(_to) ? (_to = DotLanguageUtils.NULLCODE) : _to; }
            set { _to = DotLanguageUtils.EscapeCode(value); }
	    }

	    /// <summary>
        /// Родительский подграф
        /// </summary>
	    [IgnoreSerialize]
        public SubGraph Parent { get; set; }

	    /// <summary>
	    ///
	    /// </summary>
	    [IgnoreSerialize]
	    public ColorAttribute Color {
	        get { return Get<ColorAttribute>(DotConstants.ColorAttribute); }
	        set { Set(DotConstants.ColorAttribute, value); }
	    }
        /// <summary>
        /// Стиль ребра
        /// </summary>
        [IgnoreSerialize]
        public EdgeStyleType Style {
            get { return Get<EdgeStyleType>(DotConstants.StyleAttribute); }
            set { Set(DotConstants.StyleAttribute, value); }
	    }


	    /// <summary>
		/// Автонастройка
		/// </summary>
		public override void AutoTune()
		{
			base.AutoTune();
			// автоматическое выставление Dir в случае ArrowTail
			if (HasAttribute(DotConstants.ArrowTailAttribute)) {
				if (!HasAttribute(DotConstants.DirAttribute))
				{
					Dir = DirType.Both;
				}
			}
		}

	    /// <summary>
	    /// Создает ребро
	    /// </summary>
	    /// <param name="from"></param>
	    /// <param name="to"></param>
	    /// <param name="data"></param>
	    /// <param name="setup"></param>
	    /// <returns></returns>
	    public static Edge Create(string from, string to, object data=null, Action<Edge> setup=null) {
	        var result = new Edge {From = from, To = to, Data = data};
            if (null != setup) {
                setup(result);
            }
	        return result;
	    }
	}
}