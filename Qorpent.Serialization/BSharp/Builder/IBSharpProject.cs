using Qorpent.Config;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpProject:IConfig {
		/// <summary>
		/// Целевые проекты при билде
		/// </summary>
		string[] TargetNames { get; set; }
		/// <summary>
		/// Признак полностью загруженного проекта
		/// </summary>
		bool IsFullyQualifiedProject { get; set; }	
		/// <summary>
		/// Флаги по управлению выводом
		/// </summary>
		BSharpBuilderOutputAttributes OutputAttributes { get; set; }
		/// <summary>
		/// Исходящая папка для отладочной информации
		/// </summary>
		string DebugOutputDirectory { get; set; }
		/// <summary>
		/// Исходящая папка для результатов
		/// </summary>
		string MainOutputDirectory { get; set; }
		/// <summary>
		/// Исходящая папка для журнала
		/// </summary>
		string LogOutputDirectory { get; set; }
	}
}