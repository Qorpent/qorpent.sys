using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Log;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// 
	/// </summary>
	public interface IBSharpProject : IScope {
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
        /// Требование создать пакет компилированного кода в виде перносимого архива
        /// </summary>
        string LibPkgName { get; set; }

		/// <summary>
		/// Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		bool GenerateSrcPkg { get; set; }

        /// <summary>
        /// Требование создать пакет исходников в виде перносимого архива
        /// </summary>
        bool GenerateLibPkg { get; set; }

		/// <summary>
		///Формирует Json файл с массивом рабочих классов
		/// </summary>
		bool GenerateJsonModule { get; set; }
        /// <summary>
        /// Формирует кастомный JSON
        /// </summary>
		bool GenerateJson { get; set; }

	    /// <summary>
	    /// Флаг необходимости генерации графической карты классов
	    /// </summary>
	    bool GenerateGraph { get; set; }

	    /// <summary>
	    /// Расширения проекта (имена классов или библиотек)
	    /// </summary>
	    IList<string> Extensions { get; set; }

        /// <summary>
        /// Исходный класс, на основе которого сделан проект
        /// </summary>
        IBSharpClass SrcClass { get; set; }

        /// <summary>
        /// Расширения компилятора
        /// </summary>
        IList<IBSharpCompilerExtension> CompilerExtensions { get; }

	    /// <summary>
	    /// Название модуля JSON
	    /// </summary>
	    string JsonModuleName { get; set; }

		/// <summary>
		/// Список элементов исходного кода, которые игнорируются в качестве классов, просто пропускаются
		/// </summary>
		string IgnoreElements { get; set; }

		/// <summary>
		/// 
		/// </summary>
		IScope Global { get;  }
		/// <summary>
		/// 
		/// </summary>
		IBSharpCompiler Compiler { get; set; }
		/// <summary>
		/// 
		/// </summary>
		IBSharpContext Context { get; set; }

		/// <summary>
		/// Исходное определение в XML
		/// </summary>
		XElement Definition { get; set; }

	    /// <summary>
	    /// Пространство имен по умолчанию
	    /// </summary>
	    string DefaultNamespace { get; set; }

	    /// <summary>
	    /// Имя модуля для Web-генерации
	    /// </summary>
	    string ModuleName { get; set; }
        /// <summary>
        /// Генерализованный запрет на какую либо Write продукцию - только компиляция и пост-обработка
        /// </summary>
	    bool NoOutput { get; set; }

	    bool DoCompileExtensions { get; set; }
	    string CompileFolder { get; set; }

	    /// <summary>
		/// Возвращает путь к целевой директории
		/// </summary>
		/// <returns></returns>
		string GetOutputDirectory();
        // <summary>
        /// Возвращает путь к целевой директории
        /// </summary>
        /// <returns></returns>
        string GetCompileDirectory();
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

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="target"></param>
	    IBSharpProject SafeOverrideProject(IBSharpProject target);
		/// <summary>
		/// Возвращает все директории с исходными файлами
		/// </summary>
		/// <returns></returns>
		IEnumerable<string> GetSourceDirectories();
	}
}