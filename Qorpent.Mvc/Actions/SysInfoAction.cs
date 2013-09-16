using System;
using System.Net.NetworkInformation;

namespace Qorpent.Mvc.Actions {
	/// <summary>
	/// 
	/// </summary>
	[Action("_sys.info",Role ="ADMIN", Arm="admin")]
	public class SysInfoAction :ActionBase {
		/// <summary>
		/// 	processing of execution - main method of action
		/// </summary>
		/// <returns> </returns>
		protected override object MainProcess() {
			var process = System.Diagnostics.Process.GetCurrentProcess();
			return new
				{
					domain = GetLocalDomain(),
					machineName = Environment.MachineName,
					approot = EnvironmentInfo.RootDirectory,
					pid=process.Id,
					maxworkingset = process.MaxWorkingSet,
					starttime = process.StartTime,
					totalprocesstime = process.TotalProcessorTime,
				};
		}

		/// <summary>
		/// Вычисляет домен локальной машины
		/// </summary>
		/// <returns></returns>
		public static string GetLocalDomain()
		{
			var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
			if (!String.IsNullOrWhiteSpace(ipProperties.DomainName)) {
				return ipProperties.DomainName;
			}
			return "local";
		}
	}
}