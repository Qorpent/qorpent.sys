using System;
using System.Diagnostics;

namespace Qorpent.Utils{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleApplicationResult{
		/// <summary>
		/// Признак просроченного процесса
		/// </summary>
		public bool Timeouted { get; set; }
		/// <summary>
		/// Ошибка выполнения
		/// </summary>
		public Exception Exception { get; set; }
		/// <summary>
		/// Статус процесса по выходу
		/// </summary>
		public int State { get; set; }
		/// <summary>
		/// Исходящая строка
		/// </summary>
		public string Output { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Error { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsOK{
			get{
				if (null == Exception && 0 == State) return true;
				if (null == StartInfo) return false;
				if (StartInfo.FileName.Contains("mkdir") || StartInfo.FileName.Contains("netsh")){
					return State > 0;
				}
				return false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public ProcessStartInfo StartInfo { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="res"></param>
		public void Merge(ConsoleApplicationHandler h,ConsoleApplicationResult res){
			Timeouted = Timeouted || res.Timeouted;
			Exception = Exception ?? res.Exception;
			if (0 == State){
				State = res.IsOK?0:res.State;
				Output += "Command: " + h.ExePath + " " + h.Arguments + "\r\n" + res.Output+res.Error;
			}
			
			
			if (0 != State){
				Error += "Command: " + h.ExePath + " " + h.Arguments + "\r\n" + res.Error+res.Output;
			}

		}
	}
}