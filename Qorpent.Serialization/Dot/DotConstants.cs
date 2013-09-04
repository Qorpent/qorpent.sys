namespace Qorpent.Dot {
	/// <summary>
	/// Набор констант дота
	/// </summary>
	public static class DotConstants {
		/// <summary>
		/// Атрибут заголовка
		/// </summary>
		public const string LabelAttribute = "label";
		/// <summary>
		/// Атрибут направления разрисовки графа
		/// </summary>
		public const string RankDirAttribute = "rankdir";
		/// <summary>
		/// Атрибут формы узла
		/// </summary>
		public const string ShapeAttribute = "shape";
        /// <summary>
        /// Форма конца стрелки
        /// </summary>
        public const string ArrowHeadAttribute = "arrowhead";
        /// <summary>
        /// Форма начала стрелки 
        /// </summary>
        public const string ArrowTailAttribute = "arrowtail";
        /// <summary>
        /// Если "Да", то присоединение заголовка к стрелке происходит через подчеркивание
        /// </summary>
        public const string DecorateLabelAttribute = "decorate";
        /// <summary>
        /// Если "Да", то заголовок (если длинный) будет пересекать другие стрелки, если "Нет", то заголовок не пересекает их - стрелки выгнуться
        /// </summary>
        public const string LabelFloatAttribute = "labelfloat";
        /// <summary>
        /// Стрелка (голова) направляется от узла одного подграфа к другому подграфу (а не к другому узлу другого подграфа). Для этого также у подграфов д.б. compound=true и в значение Lhead нужно подставить название подграфа
        /// </summary>        
        public const string LheadAttribute = "lhead";
        /// <summary>
        /// Стрелка (хвост) направляется от подграфа к узлу другого подграфа. Для этого также у подграфов д.б. compound=true и в значение Ltail нужно подставить название подграфа
        /// </summary>        
        public const string LtailAttribute = "ltail";
        /// <summary>
        /// Стрелки (голова) направляется к одной точке подграфа (или узла), а не разным. Н-р, если A -> B  и C -> B   и в значение SameHead к ним нужно подставить название подграфа где узел B
        /// </summary>        
        public const string SameHeadAttribute = "samehead";
        /// <summary>
        /// Стрелки (хвост) отходят от одной точки подграфа (или узла), а не от разных. Н-р, если A -> B  и A -> C   и в значение SameTail к ним нужно подставить название подграфа где узел A
        /// </summary>        
        public const string SameTailAttribute = "sametail";
        /// <summary>
        /// Если "Да", то расположение графа центировано, а если "Нет", то не центировано
        /// </summary>        
        public const string CenterAttribute = "center";
        /// <summary>
        /// Если "Да", то позволяет стрелкам идти между подграфами (См. Lhead Ltail )
        /// </summary>        
        public const string CompoundAttribute = "compound";
        /// <summary>
        /// Определеяет на каких концах ребра должны быть стрелки (или не быть). Фактически же стиль стрелки можно задать с помощью ArrowTail и ArrowHead.
        /// </summary>        
        public const string DirAttribute = "dir";
	}

}