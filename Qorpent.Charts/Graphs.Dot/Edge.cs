﻿using System;
using Qorpent.Graphs.Dot.Types;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Graphs.Dot {
	/// <summary>
	/// Ребро графа
	/// </summary>
	public class Edge : GraphElementBase {
	    private string _from;
	    private string _to;

	    /// <summary>
        /// Форма начала стрелки
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
        ///  Если "Да", то присоединение заголовка к стрелке происходит через подчеркивание
	    /// </summary>
	    [IgnoreSerialize]
	    public bool DecorateLabel {
	        get { return Get<bool>(DotConstants.DecorateLabelAttribute); }
	        set { Set(DotConstants.DecorateLabelAttribute, value); }
	    }

	    /// <summary>
        /// Если "Да", то заголовок (если длинный) будет пересекать другие стрелки, если "Нет", то заголовок не пересекает их - стрелки выгнуться
	    /// </summary>
	    [IgnoreSerialize]
	    public bool LabelFloat {
	        get { return Get<bool>(DotConstants.LabelFloatAttribute); }
	        set { Set(DotConstants.LabelFloatAttribute, value); }
	    }

	    /// <summary>
        /// Стрелка (голова) направляется от узла одного подграфа к другому подграфу (а не к другому узлу другого подграфа). Для этого также у подграфов д.б. compound=true и в значение Lhead нужно подставить название подграфа
	    /// </summary>
	    [IgnoreSerialize]
	    public string Lhead {
	        get { return Get<string>(DotConstants.LheadAttribute); }
	        set { Set(DotConstants.LheadAttribute, value); }
	    }

	    /// <summary>
        ///  Стрелка (хвост) направляется от подграфа к узлу другого подграфа. Для этого также у подграфов д.б. compound=true и в значение Ltail нужно подставить название подграфа
	    /// </summary>
	    [IgnoreSerialize]
	    public string Ltail {
	        get { return Get<string>(DotConstants.LtailAttribute); }
	        set { Set(DotConstants.LtailAttribute, value); }
	    }

	    /// <summary>
        /// Стрелки (голова) направляется к одной точке подграфа (или узла), а не разным. Н-р, если A -> B  и C -> B   и в значение SameHead к ним нужно подставить название подграфа где узел B
	    /// </summary>
	    [IgnoreSerialize]
	    public string SameHead {
	        get { return Get<string>(DotConstants.SameHeadAttribute); }
	        set { Set(DotConstants.SameHeadAttribute, value); }
	    }

	    /// <summary>
        /// Стрелки (хвост) отходят от одной точки подграфа (или узла), а не от разных. Н-р, если A -> B  и A -> C   и в значение SameTail к ним нужно подставить название подграфа где узел A
	    /// </summary>
	    [IgnoreSerialize]
	    public string SameTail {
	        get { return Get<string>(DotConstants.SameTailAttribute); }
	        set { Set(DotConstants.SameTailAttribute, value); }
	    }

        /// <summary>
        /// Определеяет на каких концах ребра должны быть стрелки (или не быть). Фактически же стиль стрелки можно задать с помощью ArrowTail и ArrowHead.
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
            get { return Get<double>(DotConstants.PenwidthAttribute); }
            set
            {
                Set(DotConstants.PenwidthAttribute, value);
            }
        }

	    /// <summary>
        /// Задает размер наконечника (головы) стрелки. По умолчанию 1
	    /// </summary>
	    [IgnoreSerialize]
	    public double ArrowSize {
	        get { return Get<double>(DotConstants.ArrowSizeAttribute); }
	        set { Set(DotConstants.ArrowSizeAttribute, value); }
	    }

	    /// <summary>
        /// Внешняя метка для узла или края. Для узлов, названия будут помещаться вне узла, но рядом с ним и для ребра - вблизи центра ребра.
	    /// </summary>
	    [IgnoreSerialize]
	    public string XLabel {
	        get { return Get<string>(DotConstants.XLabelAttribute); }
	        set { Set(DotConstants.XLabelAttribute, value); }
	    }

        [IgnoreSerialize]
        public double Weight
        {
            get { return Get<double>(DotConstants.WeightAttribute); }
            set { Set(DotConstants.WeightAttribute, value); }
        }

        /// <summary>
        /// Минимальная длина ребра в рангах (разница между головой и хвостом). По умолчанию 1, минимальная 0
        /// </summary>
        [IgnoreSerialize]
	    public int Minlen {
	        get { return Get<int>(DotConstants.MinlenAttribute); }
	        set { Set(DotConstants.MinlenAttribute, value); }
	    }

	    /// <summary>
        /// Код входящего узла
        /// </summary>
        public string From
        {
            get { return string.IsNullOrWhiteSpace(_from) ? (_from = DotLanguageUtils.NULLCODE) : _from; }
            set { _from = DotLanguageUtils.EscapeCode(value); }
        }

        /// <summary>
        /// Код исходящего узла
        /// </summary>
        public string To
        {
            get { return string.IsNullOrWhiteSpace(_to) ? (_to = DotLanguageUtils.NULLCODE) : _to; }
            set { _to = DotLanguageUtils.EscapeCode(value); }
        }

        /// <summary>
        /// Родительский подграф
        /// </summary>
        [IgnoreSerialize]
        public SubGraph Parent { get; set; }

        /// <summary>
        /// Цвет линии
        /// </summary>
        [IgnoreSerialize]
        public ColorAttribute Color
        {
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
            if (HasAttribute(DotConstants.ArrowTailAttribute))
            {
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
        public static Edge Create(string from, string to, object data = null, Action<Edge> setup = null)
        {
            var result = new Edge { From = from, To = to, Data = data };
            if (null != setup)
            {
                setup(result);
            }
            return result;
        }
    }
}