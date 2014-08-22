using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Data.DataDiff;
using Qorpent.Log;

namespace Qorpent.XDiffDataUpdater
{
	/// <summary>
	/// Программа обновления БД из файлов метаданных
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public static int Main(string[] args){
			var log = ConsoleLogWriter.CreateLog("main");
			try{
				var parameters = new DataDiffConsoleParameters();
				parameters.Initialize(args);
				log = parameters.Log;
				var executor = new DataDiffConsololeExecutor();
				executor.Execute(parameters);
				return 0;
			}
			catch (Exception ex){
				log.Fatal(ex.ToString(), ex);
				return -1;
			}
			finally{
				Thread.Sleep(200);
			}
		}
	}
}
