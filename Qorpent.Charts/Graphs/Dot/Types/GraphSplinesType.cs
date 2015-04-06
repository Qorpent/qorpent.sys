namespace Qorpent.Graphs.Dot.Types {
	/// <summary>
	/// Стили узлов
	/// </summary>
	public enum GraphSplinesType {
		/// <summary>
		/// Отсутствие связей между узлами графа
		/// </summary>
        None,
        /// <summary>
        /// Связи выстраиваются в одну линию и накладываются друг на друга 
		/// </summary>
        Line,
        /// <summary>
        /// Линейные и дугообразные связи, не накладывающиеся друг друга (по умолчанию)
        /// </summary>
        Spline,
        /// <summary>
        /// Линейные, угловые и дугообразные связи, не накладывающиеся друг друга
        /// </summary>
        Polyline,
        /// <summary>
        /// Дугообразные связи
        /// </summary>
        Curved,
        /// <summary>
        /// Линейные и угловые (90 градусов) связи, не накладывающиеся друг друга
        /// </summary>
        Ortho,
        
        
        




	}
}