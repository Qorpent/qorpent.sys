using System.Linq;
using System.Threading.Tasks;

namespace Qorpent.Applications {
	/// <summary>
	/// Обертка задачи в модели предусловий
	/// </summary>
	public sealed class TaskWrapper {
		/// <summary>
		/// Запускает задачу
		/// </summary>
		/// <param name="task"></param>
		/// <param name="dependency"> </param>
		public TaskWrapper(Task task, params TaskWrapper[] dependency) {
			_task = task;
			_dependency = dependency;
			
		}

		/// <summary>
		/// Синхронизация
		/// </summary>
		public void Wait() {
			_task.Wait();
		}

		/// <summary>
		/// Запуск задачи
		/// </summary>
		public void Run() {
			Task.Run(() =>
				{
					lock (this)
					{
						if (null != _dependency && 0 != _dependency.Length)
						{
							Task.WaitAll(_dependency.Select(_ => _._task).ToArray());
						}
						_task.Start();
					}
				});
		}

		private Task _task;
		private TaskWrapper[] _dependency;

		/// <summary>
		/// Признак завершенности задачи
		/// </summary>
		public bool IsCompleted {
			get {
				return _task.IsCompleted;
			
			}
		}
		/// <summary>
		/// Признак ошибки задачи
		/// </summary>
		public bool IsFault
		{
			get
			{

				return _task.IsFaulted;
			}
		}

		/// <summary>
		/// Текущий статус задачи
		/// </summary>
		public TaskStatus Status {
			get {
				return _task.Status;
			}
		}
	}
}