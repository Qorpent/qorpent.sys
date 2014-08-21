using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Data.DataDiff;

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
			
			try{
				var parameters = new DataDiffConsoleParameters();
				parameters.Initialize(args);
				return 0;
			}
			catch (Exception ex){
				Console.WriteLine(ex);
				return -1;
			}
		}
	}
}
