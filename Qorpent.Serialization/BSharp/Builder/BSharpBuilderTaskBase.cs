namespace Qorpent.BSharp.Builder{
	/// <summary>
	/// </summary>
	public abstract class BSharpBuilderTaskBase : IBSharpBuilderTask{
		/// <summary>
		///     Связанный целевой проект
		/// </summary>
		protected IBSharpProject Project;

		/// <summary>
		///     Установить целевой проект
		/// </summary>
		/// <param name="project"></param>
		public virtual void SetProject(IBSharpProject project){
			Project = project;
		}


		/// <summary>
		///     Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public abstract void Execute(IBSharpContext context);

		/// <summary>
		///     Фаза цели
		/// </summary>
		public BSharpBuilderPhase Phase { get; set; }

		/// <summary>
		///     Номер внутри фазы
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		///     Признак задачи, которая может быть асинхронной
		/// </summary>
		public bool Async { get; set; }
	}
}