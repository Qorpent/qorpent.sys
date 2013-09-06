namespace Qorpent.Dot {
	/// <summary>
	/// Стили стрелок (ребер)
	/// </summary>
	public enum EdgeStyleType {
		/// <summary>
		/// Сплошная линия стрелки
		/// </summary>
        Solid,
        /// <summary>
        /// Пунктирная линия стрелки
		/// </summary>
        Dashed,
        /// <summary>
        /// Точечная линия стрелки 
        /// </summary>
        Dotted,
        /// <summary>
        /// Сплошная жирная линия стрелки
        /// </summary>
        Bold,
        /// <summary>
        /// Задает жирную линию конца, сужающуюся к началу - к стрелке. Задать ширину необходимо через penwidth=число и лучше arrowtail=none
        /// </summary>
        Tapered,
        

	}
}