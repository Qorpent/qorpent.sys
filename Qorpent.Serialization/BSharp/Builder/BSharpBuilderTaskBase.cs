namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public abstract class BSharpBuilderTaskBase : IBSharpBuilderTask {
		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public abstract void Execute(IBSharpContext context);

		/// <summary>
		/// Фаза цели
		/// </summary>
		public BSharpBuilderPhase Phase { get; set; }

		/// <summary>
		/// Номер внутри фазы
		/// </summary>
		public int Index { get; set; }
	}
}