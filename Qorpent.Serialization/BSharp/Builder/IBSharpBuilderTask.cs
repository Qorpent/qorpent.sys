namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// Задачи билда
	/// </summary>
	public interface IBSharpBuilderTask {
		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		void Execute(IBSharpContext context);
		/// <summary>
		/// Фаза цели
		/// </summary>
		BSharpBuilderPhase Phase { get; }
		/// <summary>
		/// Номер внутри фазы
		/// </summary>
		int Index { get; }

		/// <summary>
		/// Установить целевой проект
		/// </summary>
		/// <param name="project"></param>
		void SetProject(IBSharpProject project);
	}
}