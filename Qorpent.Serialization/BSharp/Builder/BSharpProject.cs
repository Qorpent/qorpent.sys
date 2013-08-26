using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public class BSharpProject :ConfigBase, IBSharpProject {
        /// <summary>
        /// 
        /// </summary>
        public BSharpProject()
        {
            Sources = new List<XElement>();
            Conditions = new Dictionary<string, string>();
            Targets = new BSharpBuilderTargets();
        }

		private const string TARGET_NAMES = "target_names";
		private const string FULLY_QUALIFIED = "fully_qualified";
		private const string OUTPUT_ATTRIBUTES = "output_attrbutes";
		private const string DEBUG_OUTPUT_DIRECTORY = "debug_output_directory";
		private const string MAIN_OUTPUT_DIRECTORY = "main_output_directory";
		private const string LOG_OUTPUT_DIRECTORY = "log_output_directory";
		private const string OUTPUT_EXTENSION = "output_extension";
		private const string ROOT_DIRECTORY = "root_directory";
		private const string LOG = "log";
		private const string CONDITIONS = "conditions";
        private const string SOURCES = "sources";
        private const string TARGETS = "targets";
		
		/// <summary>
		/// Целевые проекты при билде
		/// </summary>
		public string[] TargetNames {
			get { return Get(TARGET_NAMES, new string[] {}); }
			set { Set(TARGET_NAMES, value); }
		}

		/// <summary>
		/// Признак полностью загруженного проекта
		/// </summary>
		public bool IsFullyQualifiedProject {
			get { return Get(FULLY_QUALIFIED, false); }
			set { Set(FULLY_QUALIFIED, value); }
		}

		/// <summary>
		/// Флаги по управлению выводом
		/// </summary>
		public BSharpBuilderOutputAttributes OutputAttributes {
			get { return Get(OUTPUT_ATTRIBUTES, BSharpBuilderOutputAttributes.Default); }
			set { Set(OUTPUT_ATTRIBUTES, value); }
		}

		/// <summary>
		/// Исходящая папка для отладочной информации
		/// </summary>
		public string DebugOutputDirectory {
			get { return Get(DEBUG_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultDebugDirectory); }
			set { Set(DEBUG_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		/// Исходящая папка для результатов
		/// </summary>
		public string MainOutputDirectory {
			get { return Get(MAIN_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultOutputDirectory); }
			set { Set(MAIN_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		/// Исходящая папка для журнала
		/// </summary>
		public string LogOutputDirectory {
			get { return Get(LOG_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultLogDirectory); }
			set { Set(LOG_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		/// Расширение для результирующих файлов
		/// </summary>
		public string OutputExtension {
			get { return Get(OUTPUT_EXTENSION, BSharpBuilderDefaults.DefaultOutputExtension ); }
			set { Set(OUTPUT_EXTENSION, value); }
		}

		/// <summary>
		/// Корневая директория
		/// </summary>
		public string RootDirectory {
			get { return Get(ROOT_DIRECTORY, EnvironmentInfo.RootDirectory); }
			set { Set(ROOT_DIRECTORY, value); }
		}
		/// <summary>
		/// 
		/// </summary>
        public IList<XElement> Sources {
            get { return Get<IList<XElement>>(SOURCES); }
            set { Set(SOURCES, value); }
		}
        /// <summary>
        ///     Цели проекта
        /// </summary>
        public BSharpBuilderTargets Targets {
            get { return Get<BSharpBuilderTargets>(TARGETS); }
            set { Set(TARGETS, value); }
        }
	    IUserLog _log =  new StubUserLog();
		/// <summary>
		/// Журнал проекта
		/// </summary>
		public IUserLog Log {
			get { return Get(LOG, _log); }
			set { Set(LOG, value); }
		}

		/// <summary>
		/// Условия компиляции 
		/// </summary>
		public IDictionary<string, string> Conditions
		{
			get { return Get<IDictionary<string, string>>(CONDITIONS); }
			set { Set(CONDITIONS, value); }
		}


		/// <summary>
		/// Возвращает путь к целевой директории
		/// </summary>
		/// <returns></returns>
		public string GetOutputDirectory() {
			if (!string.IsNullOrWhiteSpace(MainOutputDirectory)) {
				if (Path.IsPathRooted(MainOutputDirectory)) {
					return MainOutputDirectory;
				}
				return Path.Combine(GetRootDirectory(), MainOutputDirectory);
			}
			return Path.Combine(GetRootDirectory(), BSharpBuilderDefaults.DefaultOutputDirectory);
		}

		/// <summary>
		/// Возвращает нормализованный полный путь корневой папки репозитория или решения
		/// </summary>
		/// <returns></returns>
		public string GetRootDirectory() {
			if (string.IsNullOrWhiteSpace(RootDirectory)) return EnvironmentInfo.RootDirectory;
			return Path.GetFullPath(RootDirectory);
		}

		/// <summary>
		/// Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		public string GetOutputExtension() {
			if (string.IsNullOrWhiteSpace(OutputExtension)) return BSharpBuilderDefaults.DefaultOutputExtension;
			return OutputExtension;
		}

		/// <summary>
		/// Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		public string GetLogDirectory()
		{
			if (!string.IsNullOrWhiteSpace(LogOutputDirectory)) {
				if (Path.IsPathRooted(LogOutputDirectory)) {
					return LogOutputDirectory;
				}
				return Path.Combine(GetRootDirectory(), LogOutputDirectory);
			}
			return Path.Combine(GetRootDirectory(), BSharpBuilderDefaults.DefaultOutputDirectory);
		}
	}
}