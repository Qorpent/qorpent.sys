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
        CrossClassLink
	}
}