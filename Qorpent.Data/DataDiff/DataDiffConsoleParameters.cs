using System;
using System.IO;
using Qorpent.BSharp;
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
			TreatAnonymousAsBSharpProjectReference = true;
			RepositoryDirectory = "auto";
			ProjectName = "";
			ProjectDirectory = "auto";
			Prototype = "data";
			OutputDirectory = ".sqldiff";
			Branch = "master";
			FullUpdate = false;
			ApplyToDatabase = true;
			Server = "(local)";
		}
		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string CheckoutDirectory
		{
			get { return Get("checkoutdirectory", ""); }
			set { Set("checkoutdirectory", value); }
		}
		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string RepositoryDirectory
		{
			get { return Get("repositorydirectory", ""); }
			set { Set("repositorydirectory", value); }
		}
		/// <summary>
		/// Путь к папке внутри репозитория
		/// </summary>
		public string ProjectDirectory
		{
			get { return Get("projectdirectory", ""); }
			set { Set("projectdirectory", value); }
		}
		/// <summary>
		/// Имя проекта в папке (при компиляции через bsproj)
		/// </summary>
		public string ProjectName
		{
			get { return Get("projectname", ""); }
			set { Set("projectname", value); }
		}
		/// <summary>
		/// Прототип (прототипы) для отбора на синхронизацию
		/// </summary>
		public string Prototype
		{
			get { return Get("prototype", ""); }
			set { Set("prototype", value); }
		}
		/// <summary>
		/// Директория для SQL скриптов
		/// </summary>
		public string OutputDirectory
		{
			get { return Get("outputdirectory", ""); }
			set { Set("outputdirectory", value); }
		}

		/// <summary>
		/// SQL Server
		/// </summary>
		public string Server
		{
			get { return Get("server", ""); }
			set { Set("server", value); }
		}
		/// <summary>
		/// SQL Database Name
		/// </summary>
		public string Database
		{
			get { return Get("database", ""); }
			set { Set("database", value); }
		}
		/// <summary>
		/// Строка соединения
		/// </summary>
		public string Connection
		{
			get { return Get("connection", ""); }
			set { Set("connection", value); }
		}
		/// <summary>
		/// Бранч в репозитории
		/// </summary>
		public string Branch
		{
			get { return Get("branch", ""); }
			set { Set("branch", value); }
		}

		/// <summary>
		/// Полное обновление
		/// </summary>
		public bool FullUpdate
		{
			get { return Get("fullupdate", false); }
			set { Set("fullupdate", value); }
		}

		/// <summary>
		/// Регистрировать в таблице метафайлов SQL
		/// </summary>
		public bool RegisterInMetaTable
		{
			get { return Get("registerinmetatable", false); }
			set { Set("registerinmetatable", value); }
		}

		/// <summary>
		/// Применить дельту к базе
		/// </summary>
		public bool ApplyToDatabase
		{
			get { return Get("applytodatabase", false); }
			set { Set("applytodatabase", value); }
		}

		/// <summary>
		/// Отложенный конструктор, логика подготовки 
		/// </summary>
		protected override void InternalInitialize( string[] arguments)
		{
			base.InternalInitialize(arguments);
			var projdesc = new BSharpProjectDescriptor();
			projdesc.AutoSetup();
			if (RepositoryDirectory == "auto"){
				RepositoryDirectory = projdesc.RepositoryDirectory;
				if (ProjectDirectory == "auto"){
					ProjectDirectory = projdesc.ProjectDirectory;
				}
			}
			if (string.IsNullOrWhiteSpace(CheckoutDirectory)){
				CheckoutDirectory = Path.Combine(Path.GetTempPath(), ".xdiffupd", DateTime.Now.ToString("yyyyMMddHHmmss"));
				
			}
			if (string.IsNullOrWhiteSpace(Connection)){
				if (string.IsNullOrWhiteSpace(Database)){
					Database = "temp";
				}
				Connection = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True;Application Name=xdbu",
				                           Server, Database);
			}
		}

	}
}
