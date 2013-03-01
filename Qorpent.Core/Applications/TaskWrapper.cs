using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Qorpent.Applications {
	/// <summary>
	/// ������� ������ � ������ �����������
	/// </summary>
	public sealed class TaskWrapper {
		/// <summary>
		/// ��������� ������
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
		/// �������������
		/// </summary>
		public void Wait() {
			if (null != _dependency && 0 != _dependency.Length)
			{
				Task.WaitAll(_dependency.Select(_ => _._task).ToArray(),DependencyWait);
			}
			_task.Wait(SelfWait);
		}


		/// <summary>
		/// �������� ����������������
		/// </summary>
		public int DependencyWait { get; set; }
		/// <summary>
		/// �������� ����������� ������
		/// </summary>
		public int SelfWait { get; set; }

		/// <summary>
		/// ����� ����������
		/// </summary>
		public TimeSpan ExecuteTime { get; set; }

		/// <summary>
		/// ������ � ������
		/// </summary>
		public Exception Error {
			get { return _task.Exception; }
		}


		/// <summary>
		/// ������ ������
		/// </summary>
		public void Run() {
			_sw = Stopwatch.StartNew();
			Task.Run(() =>
				{
					lock (this) {
						
						
						if (null != _dependency && 0 != _dependency.Length)
						{
							Task.WaitAll(_dependency.Select(_ => _._task).ToArray());
						}
						_task.ContinueWith(t =>
						{
							_sw.Stop();
							ExecuteTime = _sw.Elapsed;
						});
						if(!_task.IsCompleted||!_task.IsFaulted) {
							_task.Start();
						}
					}
				});
		}

		private Task _task;
		private TaskWrapper[] _dependency;
		private Stopwatch _sw;

		/// <summary>
		/// ������� ������������� ������
		/// </summary>
		public bool IsCompleted {
			get {
				return _task.IsCompleted;
			
			}
		}
		/// <summary>
		/// ������� ������ ������
		/// </summary>
		public bool IsFault
		{
			get
			{

				return _task.IsFaulted;
			}
		}

		/// <summary>
		/// ������� ������ ������
		/// </summary>
		public TaskStatus Status {
			get {
				return _task.Status;
			}
		}
	}
}