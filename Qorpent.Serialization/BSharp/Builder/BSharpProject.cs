using Qorpent.Config;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public class BSharpProject :ConfigBase, IBSharpProject {
		private const string TARGET_NAMES = "target_names";
		private const string FULLY_QUALIFIED = "fully_qualified";
		private const string OUTPUT_ATTRIBUTES = "output_attrbutes";
		private const string DEBUG_OUTPUT_DIRECTORY = "debug_output_directory";
		private const string MAIN_OUTPUT_DIRECTORY = "main_output_directory";
		private const string LOG_OUTPUT_DIRECTORY = "log_output_directory";

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
			get { return Get(DEBUG_OUTPUT_DIRECTORY, ".debug"); }
			set { Set(DEBUG_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		/// Исходящая папка для результатов
		/// </summary>
		public string MainOutputDirectory {
			get { return Get(MAIN_OUTPUT_DIRECTORY, ".output"); }
			set { Set(MAIN_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		/// Исходящая папка для журнала
		/// </summary>
		public string LogOutputDirectory {
			get { return Get(LOG_OUTPUT_DIRECTORY, ".log"); }
			set { Set(LOG_OUTPUT_DIRECTORY, value); }
		}
	}
}