using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Utils;

namespace Qorpent.Data.DataDiff
{
	/// <summary>
	/// 
	/// </summary>
	public class DataDiffConsoleParameters : ConsoleApplicationParameters
	{
		/// <summary>
		/// 
		/// </summary>
		public DataDiffConsoleParameters(){
			this.TreatAnonymousAsBSharpProjectReference = true;
			RepositoryPath = "auto";
		}
		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string RepositoryPath { get; set; }
		/// <summary>
		/// Путь к папке внутри репозитория
		/// </summary>
		public string ProjectDirectory { get; set; }
		/// <summary>
		/// Имя проекта в папке (при компиляции через bsproj)
		/// </summary>
		public string ProjectName { get; set; }
		/// <summary>
		/// Прототип (прототипы) для отбора на синхронизацию
		/// </summary>
		public string Prototype { get; set; }
		/// <summary>
		/// Строка соединения
		/// </summary>
		public string Connection{ get; set; }
	}
}
