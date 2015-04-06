using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Qorpent.Utils.Extensions{
	/// <summary>
	/// 
	/// </summary>
	public static class OsExtensions{
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		private const int SW_SHOWNORMAL = 1;
		private const int SW_SHOWMAXIMIZED = 3;
		private const int SW_RESTORE = 9;

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="process"></param>
		/// <returns></returns>
		public static Process EnsureForeground(this Process process){
			if (IntPtr.Zero == process.MainWindowHandle) return process;
			process.WaitForInputIdle();
			ShowWindow(process.MainWindowHandle, SW_RESTORE);
			ShowWindow(process.MainWindowHandle, SW_SHOWNORMAL);
			SetForegroundWindow(process.MainWindowHandle);
			return process;
		}

	}
}