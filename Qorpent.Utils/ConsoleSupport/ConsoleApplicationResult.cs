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
	}
}