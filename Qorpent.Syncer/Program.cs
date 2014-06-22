using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO.Syncronization;
using Qorpent.Log;

namespace Qorpent.Syncer
{
	/// <summary>
	/// 
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static int Main(string[] args){
			var log = ConsoleLogWriter.CreateLog("main", LogLevel.Info, "${Message}");
			if (0 == args.Length){
				log.Error("no dir given");
				return -1;
			}
			var src = Environment.CurrentDirectory;
			var trg = args[0];
			if (1 < args.Length){
				src = args[0];
				trg = args[1];
			}
			var syncer = new DirectorySynchronization(src, trg){Log =log};
			try{
				syncer.Synchronize();
			}
			catch (Exception ex){
				log.Error("error "+ex.Message,ex);
				return -2;
			}
			return 0;
		}
	}
}
