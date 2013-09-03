using Qorpent.Utils.Extensions;
namespace Qorpent.Dot {
	/// <summary>
	/// Ребро графа
	/// </summary>
	public class Edge : GraphElementBase {
        /// <summary>
        /// Форма конца стрелки
        /// </summary>
        public ArrowType ArrowHead
        {
            get { return Get(DotConstants.ArrowHeadAttribute).To<ArrowType>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.ArrowHeadAttribute, str);
            }
        }
        /// <summary>
        /// Форма начала стрелки
        /// </summary>
        public ArrowType ArrowTail
        {
            get { return Get(DotConstants.ArrowTailAttribute).To<ArrowType>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.ArrowTailAttribute, str);

            }
        }

        /// <summary>
        /// Если "Да", то присоединение заголовка к стрелке происходит через подчеркивание
        /// </summary>
        public bool DecorateLabel
        {
            get { return Get(DotConstants.DecorateLabelAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.DecorateLabelAttribute, str);

            }
        }
        /// <summary>
        /// Если "Да", то заголовок (если длинный) будет пересекать другие стрелки, если "Нет", то заголовок не пересекает их - стрелки выгнуться 
        /// </summary>
        public bool LabelFloat
        {
            get { return Get(DotConstants.LabelFloatAttribute).To<bool>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.LabelFloatAttribute, str);

            }
        }
        /// <summary>
        /// Стрелка (голова) направляется от узла одного подграфа к другому подграфу (а не к другому узлу другого подграфа). Для этого также у подграфов д.б. compound=true и в значение Lhead нужно подставить название подграфа
        /// </summary>
        public string Lhead
        {
            get { return Get(DotConstants.LheadAttribute).To<string>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.LheadAttribute, str);

            }
        }
        /// <summary>
        /// Стрелка (хвост) направляется от подграфа к узлу другого подграфа. Для этого также у подграфов д.б. compound=true и в значение Ltail нужно подставить название подграфа
        /// </summary>
        public string Ltail
        {
            get { return Get(DotConstants.LtailAttribute).To<string>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.LtailAttribute, str);

            }
        }
        /// <summary>
        /// Стрелки (голова) направляется к одной точке подграфа (или узла), а не разным. Н-р, если A -> B  и C -> B   и в значение SameHead к ним нужно подставить название подграфа где узел B
        /// </summary>
        public string SameHead
        {
            get { return Get(DotConstants.SameHeadAttribute).To<string>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.SameHeadAttribute, str);

            }
        }
        /// <summary>
        /// Стрелки (хвост) отходят от одной точки подграфа (или узла), а не от разных. Н-р, если A -> B  и A -> C   и в значение SameTail к ним нужно подставить название подграфа где узел A
        /// </summary>
        public string SameTail
        {
            get { return Get(DotConstants.SameTailAttribute).To<string>(); }
            set
            {
                var str = value.ToStr().ToLower();
                Set(DotConstants.SameTailAttribute, str);

            }
        }
    
    
    
    
    
    
    
    
    
    
    
    }
}