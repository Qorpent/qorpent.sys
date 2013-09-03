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
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}