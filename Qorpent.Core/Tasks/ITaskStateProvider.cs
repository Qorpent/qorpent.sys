namespace Qorpent.Tasks {
	/// <summary>
	///		Провайдер состояний задач
	/// </summary>
	public interface ITaskStateProvider {
		/// <summary>
		///		Разрешает состояние задачи относительно её уникального идентификатора
		/// </summary>
		/// <param name="id">Идентификатор задачи</param>
		/// <returns>Состояние задачи</returns>
		TaskState GetState(long id);
	}
}