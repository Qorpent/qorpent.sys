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
			ProjectName = "bs-default";
			ProjectDirectory = ".";
			Prototype = "data";
			OutputDirectory = ".sqldiff";
			Branch = "master";
			FullUpdate = false;
			ApplyToDatabase = true;
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
		/// Директория для SQL скриптов
		/// </summary>
		public string OutputDirectory { get; set; }
		/// <summary>
		/// Строка соединения
		/// </summary>
		public string Connection{ get; set; }
		/// <summary>
		/// Бранч в репозитории
		/// </summary>
		public string Branch { get; set; }

		/// <summary>
		/// Полное обновление
		/// </summary>
		public bool FullUpdate { get; set; }

		/// <summary>
		/// Регистрировать в таблице метафайлов SQL
		/// </summary>
		public bool RegisterInMetaTable { get; set; }

		/// <summary>
		/// Применить дельту к базе
		/// </summary>
		public bool ApplyToDatabase { get; set; }

	}
}
