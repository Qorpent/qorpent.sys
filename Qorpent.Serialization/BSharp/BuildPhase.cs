namespace Qorpent.BSharp {
	/// <summary>
	/// Большая фаза компиляции
	/// </summary>
	public enum BuildPhase {
		/// <summary>
		/// Компиляция
		/// </summary>
		Compile,
		/// <summary>
		/// Линковка на уровне отдельного класа
		/// </summary>
	    AutonomeLink,
        /// <summary>
        /// Линковка между классами
        /// </summary>
        CrossClassLink,
		/// <summary>
		/// Вычисление определений
		/// </summary>
		Evaluate,
		/// <summary>
		/// Применение патчей
		/// </summary>
		ApplyPatch,
		
	}
}