using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;

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
		/// <summary>
		/// Расширение для результирующих файлов
		/// </summary>
		string OutputExtension { get; set; }

		/// <summary>
		/// Корневая директория
		/// </summary>
		string RootDirectory { get; set; }

		/// <summary>
		/// Исходный код
		/// </summary>
		IList<XElement> Sources { get; }

		/// <summary>
		/// Журнал проекта
		/// </summary>
		IUserLog Log { get; set; }

		/// <summary>
		/// Возвращает путь к целевой директории
		/// </summary>
		/// <returns></returns>
		string GetOutputDirectory();

		/// <summary>
		/// Возвращает нормализованный полный путь корневой папки репозитория или решения
		/// </summary>
		/// <returns></returns>
		string GetRootDirectory();
		/// <summary>
		/// Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		string GetOutputExtension();

		/// <summary>
		/// Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		string GetLogDirectory();
	}
}