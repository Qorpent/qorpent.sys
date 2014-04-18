using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent
{
	/// <summary>
	/// 
	/// </summary>
	public class ScheduledTask
	{
		private readonly Action _worker;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="worker"></param>
		public ScheduledTask(Action worker){
			this._worker = worker;
			StartInterval = 1000;
			AfterSuccessInterval = 60000;
			AfterErrorInterval = 60000;
			WaitInterval = 30000;
		}
		/// <summary>
		/// Время ожидания выполнения
		/// </summary>
		public int WaitInterval { get; set; }

		/// <summary>
		/// Время до первого запуска
		/// </summary>
		public int StartInterval { get; set; }
		/// <summary>
		/// Интервал после удачного выполнения
		/// </summary>
		public int AfterSuccessInterval { get; set; }

		/// <summary>
		/// Интервал после неудачного выполнения
		/// </summary>
		public int AfterErrorInterval { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Exception LastError { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnStart;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnBeforeExecute;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnSuccess;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnComplete;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnError;

		/// <summary>
		/// 
		/// </summary>
		public event Action<ScheduledTask> OnStop;

		/// <summary>
		/// Продолжать активность после ошибки
		/// </summary>
		public bool ProceedOnError { get; set; }
		/// <summary>
		/// Если обращение к воркеру производится в момент работы его предыдущего запуска, то повторный вызов не производится
		/// </summary>
		public bool UseCurrentTaskIfItWasNotCompleted { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool WasStoppedAfterError { get; set; }

		private bool _started = false;


		private Timer _timer;
		/// <summary>
		/// 
		/// </summary>
		public void Start(){
			if (_started) return;
			_started = true;
			_timer = new Timer(Execute,true,StartInterval,Timeout.Infinite);	
			if (null != OnStart){
				OnStart.Invoke(this);
			}
		}

		private void Execute(object state){
			if (!_started) return;
			if (null != OnBeforeExecute){
				OnBeforeExecute.Invoke(this);
			}
			var t = ExecuteWorker();
			var normaltime = false;
			try{
				normaltime = t.Wait(WaitInterval);
			}
			catch{
			}

			if (null != OnComplete){
				OnComplete.Invoke(this);
			}

			if (t.IsFaulted || !normaltime){
				var ex = t.Exception ?? new Exception("timeout");
				AfterError(ex);
			}
			else{
				if (null != OnSuccess){
					OnSuccess.Invoke(this);
				}
				_timer = new Timer(Execute, true, AfterSuccessInterval, Timeout.Infinite);
			}
		}

		private void AfterError(Exception ex){
			LastError = ex;
			if (null != OnError){
				OnError.Invoke(this);
			}
			if (ProceedOnError){
				_timer = new Timer(Execute, true, AfterErrorInterval, Timeout.Infinite);
			}
			else{
				WasStoppedAfterError = true;
				Stop();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Stop(){
			if (!_started) return;
			_started = false;
			if (null != OnStop)
			{
				OnStop.Invoke(this);
			}
		}

		private Task _current = null;
		/// <summary>
		/// Запуск на исполнение непосредственно обработчкиа
		/// </summary>
		/// <returns></returns>
		public Task ExecuteWorker(){
			if (null != _current){
				if (!_current.IsCompleted){
					_current.Wait();
					if (UseCurrentTaskIfItWasNotCompleted){
						return _current;
					}
				}
			}
			_current = Task.Run(_worker);
			return _current;
		}

		
	}
}
