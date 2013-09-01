using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Config;
using Qorpent.Log;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpProject : IConfig {
        /// <summary>
        ///     Цели проекта
        /// </summary>
        BSharpBuilderTargets Targets { get; set; }
		/// <summary>
		/// Признак полностью загруженного проекта
		/// </summary>
		bool IsFullyQualifiedProject { get; set; }
        /// <summary>
        ///     Признак того, что результаты будут писаться на диск
        /// </summary>
        bool WriteCompiled { get; set; }
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
        ///     Расширение для входных файлов
        /// </summary>
        string InputExtensions { get; set; }
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
		/// Условия компиляции 
		/// </summary>
		IDictionary<string, string> Conditions { get; set; }
		/// <summary>
		/// Имя проекта
		/// </summary>
		string ProjectName { get; set; }

		/// <summary>
		/// Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		string SrcPkgName { get; set; }

		/// <summary>
		/// Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		bool GenerateSrcPkg { get; set; }

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