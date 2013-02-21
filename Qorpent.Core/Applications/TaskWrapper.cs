using System;
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
			SelfWait = -1;
			DependencyWait = -1;

		}

		/// <summary>
		/// Синхронизация
		/// </summary>
		public void Wait() {
			if (null != _dependency && 0 != _dependency.Length)
			{
				Task.WaitAll(_dependency.Select(_ => _._task).ToArray(),DependencyWait);
			}
			_task.Wait(SelfWait);
		}


		/// <summary>
		/// Ожидание предшественников
		/// </summary>
		public int DependencyWait { get; set; }
		/// <summary>
		/// Ожидание собственной задачи
		/// </summary>
		public int SelfWait { get; set; }

		/// <summary>
		/// Доступ к ошибке
		/// </summary>
		public Exception Error {
			get { return _task.Exception; }
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