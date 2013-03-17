#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/TaskWrapper.cs
#endregion
using System;
using System.Diagnostics;
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
		/// Время выполнения
		/// </summary>
		public TimeSpan ExecuteTime { get; set; }

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