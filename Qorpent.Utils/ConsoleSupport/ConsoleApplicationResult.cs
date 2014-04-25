using System;

namespace Qorpent.Utils{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleApplicationResult{
		/// <summary>
		/// ������� ������������� ��������
		/// </summary>
		public bool Timeouted { get; set; }
		/// <summary>
		/// ������ ����������
		/// </summary>
		public Exception Exception { get; set; }
		/// <summary>
		/// ������ �������� �� ������
		/// </summary>
		public int State { get; set; }
		/// <summary>
		/// ��������� ������
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
			get { return null == Exception && 0 == State && string.IsNullOrWhiteSpace(Error); }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="h"></param>
		/// <param name="res"></param>
		public void Merge(ConsoleApplicationHandler h,ConsoleApplicationResult res){
			Timeouted = Timeouted || res.Timeouted;
			Exception = Exception ?? res.Exception;
			if (0 == State) State = res.State;
			Output += "Command: " + h.ExePath + " " + h.Arguments + "\r\n" + res.Output;
			if (0 != State){
				Error += "Command: " + h.ExePath + " " + h.Arguments + "\r\n" + res.Error;
			}

		}
	}
}