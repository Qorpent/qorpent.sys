using System;

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
			get { return null == Exception && 0 == State && string.IsNullOrWhiteSpace(Error); }
		}
	}
}