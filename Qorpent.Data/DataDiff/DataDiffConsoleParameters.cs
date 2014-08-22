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
			ProjectName = "bs-default";
			ProjectDirectory = "auto";
			Prototype = "data";
			OutputDirectory = ".sqldiff";
			Branch = "master";
			FullUpdate = false;
			ApplyToDatabase = true;
		}
		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string RepositoryDirectory
		{
			get { return Get("repositorypath", ""); }
			set { Set("repositorypath", value); }
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
		public override void Initialize(params string[] arguments)
		{
			base.Initialize(arguments);
			var projdesc = new BSharpProjectDescriptor();
			projdesc.AutoSetup();
			if (RepositoryDirectory == "auto"){
				RepositoryDirectory = projdesc.RepositoryDirectory;
				if (ProjectDirectory == "auto"){
					ProjectDirectory = projdesc.ProjectDirectory;
				}
			}
		}

	}
}
