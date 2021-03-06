﻿using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Builder{
	/// <summary>
	/// </summary>
	public class BSharpProject : Scope, IBSharpProject{
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
		private const string INPUT_EXTENSION = "input_extension";
		private const string WRITE_COMPILED = "write_compiled";
		private const string PROJECT_NAME = "project_name";
		private const string GENERATE_SRC_PKG = "generate_src_pkg";
		private const string GENERATE_LIB_PKG = "generate_lib_pkg";
		private const string GENERATE_JSON_MODULE = "generate_json_module";
		private const string SRC_PKG_NAME = "src_pkg_name";
		private const string LIB_PKG_NAME = "lib_pkg_name";
		private const string GENERATE_GRAPH = "generate_graph";
		private const string EXTENSIONS = "extensions";
		private const string SRCCLASS = "srcclass";
		private const string JSON_MODULE_NAME = "json_module_name";
		private const string IGNORE_ELEMENTS = "ignore_elements";
        private const string DEFAULTNAMESPCE = "defaultnamespace";
        private const string MODULENAME = "modulename";
        private const string NOOUTPUT = "nooutput";
        private const string DOCOMPILEEXTENSIONS = "docompileextensions";
        private const string COMPILEFOLDER = "compileextensionsdir";
        private const string GENERATEJSON = "generatejson";
        private const string EXECUTEXSLTTASKS = "executexslttasks";

		private static readonly string[] overrideAttributes = new[]{
			OUTPUT_ATTRIBUTES,
			MAIN_OUTPUT_DIRECTORY,
			ROOT_DIRECTORY,
			GENERATE_SRC_PKG,
			GENERATE_LIB_PKG,
			GENERATE_JSON_MODULE,
			SRC_PKG_NAME,
			LIB_PKG_NAME,
			GENERATE_GRAPH,
			JSON_MODULE_NAME,
			CONDITIONS
		};

		private readonly IList<IBSharpCompilerExtension> _compilerExtensions = new List<IBSharpCompilerExtension>();
		private readonly IUserLog _log = new StubUserLog();
		private IScope _global;

		/// <summary>
		/// </summary>
		public BSharpProject(){
			Sources = new List<XElement>();
			Conditions = new Dictionary<string, string>();
			Targets = new BSharpBuilderTargets();
			Extensions = new List<string>();
		}

		/// <summary>
		///     Целевые проекты при билде
		/// </summary>
		public string[] TargetNames{
			get { return Get(TARGET_NAMES, new string[]{}); }
			set { Set(TARGET_NAMES, value); }
		}

		/// <summary>
		/// </summary>
		/// <param name="target"></param>
		public IBSharpProject SafeOverrideProject(IBSharpProject target){
			foreach (string overrideAttribute in overrideAttributes){
				if (ContainsKey(overrideAttribute) && this[overrideAttribute].ToBool()){
					target.Set(overrideAttribute, this[overrideAttribute]);
				}
			}
			return target;
		}

		/// <summary>
		/// Возвращает все директории с исходными файлами
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetSourceDirectories(){
		    if (null == Definition && !Definition.Attr("ExplicitSources").ToBool()) {
		        yield return GetRootDirectory();
		    }
		    if (null != SourceDirectories){
				foreach (var dir in SourceDirectories){
					if (Path.IsPathRooted(dir)){
						yield return dir;

					}
					else{
					    if (dir.Contains("@"))
					    {
					        yield return EnvironmentInfo.ResolvePath(dir).NormalizePath();
					    }
					    else
					    {
					        yield return
					            EnvironmentInfo.ResolvePath(Path.GetFullPath(Path.Combine(GetRootDirectory(), dir))).NormalizePath();
					    }
					}
				}
			}
		}

		/// <summary>
		///     Расширения компилятора
		/// </summary>
		public IList<IBSharpCompilerExtension> CompilerExtensions{
			get { return _compilerExtensions; }
		}

		/// <summary>
		///     Признак полностью загруженного проекта
		/// </summary>
		public bool IsFullyQualifiedProject{
			get { return Get(FULLY_QUALIFIED, false); }
			set { Set(FULLY_QUALIFIED, value); }
		}

		/// <summary>
		/// </summary>
		public bool WriteCompiled{
			get { return Get(WRITE_COMPILED, true); }
			set { Set(WRITE_COMPILED, value); }
		}

		/// <summary>
		///     Флаги по управлению выводом
		/// </summary>
		public BSharpBuilderOutputAttributes OutputAttributes{
			get { return Get(OUTPUT_ATTRIBUTES, BSharpBuilderOutputAttributes.Default); }
			set { Set(OUTPUT_ATTRIBUTES, value); }
		}

		/// <summary>
		///     Исходящая папка для отладочной информации
		/// </summary>
		public string DebugOutputDirectory{
			get { return Get(DEBUG_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultDebugDirectory); }
			set { Set(DEBUG_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		///     Исходящая папка для отладочной информации
		/// </summary>
		public IList<string> SourceDirectories
		{
			get { return Get<IList<string>>("sourcedirectories"); }
			set { Set("sourcedirectories", value); }
		}

		/// <summary>
		///     Исходящая папка для результатов
		/// </summary>
		public string MainOutputDirectory{
			get { return Get(MAIN_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultOutputDirectory); }
			set { Set(MAIN_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		///     Исходящая папка для журнала
		/// </summary>
		public string LogOutputDirectory{
			get { return Get(LOG_OUTPUT_DIRECTORY, BSharpBuilderDefaults.DefaultLogDirectory); }
			set { Set(LOG_OUTPUT_DIRECTORY, value); }
		}

		/// <summary>
		///     Расширение для результирующих файлов
		/// </summary>
		public string OutputExtension{
			get { return Get(OUTPUT_EXTENSION, BSharpBuilderDefaults.DefaultOutputExtension); }
			set { Set(OUTPUT_EXTENSION, value); }
		}

		/// <summary>
		///     Расширение для входных файлов
		/// </summary>
		public string InputExtensions{
			get { return Get(INPUT_EXTENSION, BSharpBuilderDefaults.DefaultInputExtension); }
			set { Set(INPUT_EXTENSION, value); }
		}

		/// <summary>
		///     Корневая директория
		/// </summary>
		public string RootDirectory{
			get { return Get(ROOT_DIRECTORY, EnvironmentInfo.RootDirectory); }
			set { Set(ROOT_DIRECTORY, value); }
		}

		/// <summary>
		/// </summary>
		public IList<XElement> Sources{
			get { return Get<IList<XElement>>(SOURCES); }
			set { Set(SOURCES, value); }
		}

		/// <summary>
		///     Цели проекта
		/// </summary>
		public BSharpBuilderTargets Targets{
			get { return Get<BSharpBuilderTargets>(TARGETS); }
			set { Set(TARGETS, value); }
		}


		/// <summary>
		///     Журнал проекта
		/// </summary>
		public IUserLog Log{
			get { return Get(LOG, _log); }
			set { Set(LOG, value); }
		}

		/// <summary>
		///     Условия компиляции
		/// </summary>
		public IDictionary<string, string> Conditions{
			get { return Get<IDictionary<string, string>>(CONDITIONS); }
			set { Set(CONDITIONS, value); }
		}

		/// <summary>
		///     Корневая директория
		/// </summary>
		public string ProjectName{
			get { return Get(PROJECT_NAME, ""); }
			set { Set(PROJECT_NAME, value); }
		}

		/// <summary>
		///     Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		public bool GenerateSrcPkg{
			get { return Get(GENERATE_SRC_PKG, false); }
			set { Set(GENERATE_SRC_PKG, value); }
		}

		/// <summary>
		///     Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		public bool GenerateLibPkg{
			get { return Get(GENERATE_LIB_PKG, false); }
			set { Set(GENERATE_LIB_PKG, value); }
		}

		/// <summary>
		///     Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		public bool GenerateJsonModule{
			get { return Get(GENERATE_JSON_MODULE, false); }
			set { Set(GENERATE_JSON_MODULE, value); }
		}

		/// <summary>
		///     Требование создать пакет исходников в виде перносимого архива
		/// </summary>
		public string SrcPkgName{
			get { return Get(SRC_PKG_NAME, "src.bssrc"); }
			set { Set(SRC_PKG_NAME, value); }
		}

		/// <summary>
		///     Имя компилированного пакета
		/// </summary>
		public string LibPkgName{
			get { return Get(LIB_PKG_NAME, "compiled.bslib"); }
			set { Set(LIB_PKG_NAME, value); }
		}

		/// <summary>
		///     Флаг необходимости генерации графической карты классов
		/// </summary>
		public bool GenerateGraph{
			get { return Get(GENERATE_GRAPH, false); }
			set { Set(GENERATE_GRAPH, value); }
		}

		/// <summary>
		///     Расширения проекта (имена классов или библиотек)
		/// </summary>
		public IList<string> Extensions{
			get { return Get<IList<string>>(EXTENSIONS); }
			set { Set(EXTENSIONS, value); }
		}

		/// <summary>
		///     Исходный класс, на основе которого сделан проект
		/// </summary>
		public IBSharpClass SrcClass{
			get { return Get<IBSharpClass>(SRCCLASS); }
			set { Set(SRCCLASS, value); }
		}

		/// <summary>
		///     Название модуля JSON
		/// </summary>
		public string JsonModuleName{
			get { return Get<string>(JSON_MODULE_NAME); }
			set { Set(JSON_MODULE_NAME, value); }
		}

		/// <summary>
		///     Список элементов исходного кода, которые игнорируются в качестве классов, просто пропускаются
		/// </summary>
		public string IgnoreElements{
			get { return Get<string>(IGNORE_ELEMENTS); }
			set { Set(IGNORE_ELEMENTS, value); }
		}

		/// <summary>
		/// </summary>
		public IScope Global{
			get { return _global ?? (_global = new Scope{UseInheritance = false}); }
		}

		/// <summary>
		/// </summary>
		public IBSharpCompiler Compiler { get; set; }

		/// <summary>
		/// </summary>
		public IBSharpContext Context { get; set; }
		/// <summary>
		/// Исходное определение в XML
		/// </summary>
		public XElement Definition { get; set; }

	    /// <summary>
	    /// Пространство имен по умолчанию
	    /// </summary>
	    public string DefaultNamespace {
            get { return Get<string>(DEFAULTNAMESPCE); }
            set { Set(DEFAULTNAMESPCE, value); }
	    }

        /// <summary>
        /// Имя модуля для Web-генерации
        /// </summary>
        public string ModuleName
        {
            get { return Get<string>(MODULENAME); }
            set { Set(MODULENAME, value); }
        }

	    public bool NoOutput {
            get { return Get<bool>(NOOUTPUT); }
            set { Set(NOOUTPUT, value); }
	    }

	    public bool DoCompileExtensions
        {
            get { return Get<bool>(DOCOMPILEEXTENSIONS); }
            set { Set(DOCOMPILEEXTENSIONS, value); }
        }
        public string CompileFolder
        {
            get { return Get<string>(COMPILEFOLDER); }
            set { Set(COMPILEFOLDER, value); }
        }

	    public bool GenerateJson
        {
            get { return Get<bool>(GENERATEJSON); }
            set { Set(GENERATEJSON, value); }
        }

        public bool ExecuteXsltTasks
        {
            get { return Get<bool>(EXECUTEXSLTTASKS); }
            set { Set(EXECUTEXSLTTASKS, value); }
        }

        /// <summary>
        ///     Возвращает путь к целевой директории
        /// </summary>
        /// <returns></returns>
        public string GetOutputDirectory(){
			if (!string.IsNullOrWhiteSpace(MainOutputDirectory)){
				if (Path.IsPathRooted(MainOutputDirectory)){
					return MainOutputDirectory;
				}
				return Path.Combine(GetRootDirectory(), MainOutputDirectory);
			}
			return Path.Combine(GetRootDirectory(), BSharpBuilderDefaults.DefaultOutputDirectory);
		}

	    public string GetCompileDirectory() {
            if (!string.IsNullOrWhiteSpace(CompileFolder))
            {
                if (Path.IsPathRooted(CompileFolder))
                {
                    return CompileFolder;
                }
                return Path.Combine(GetRootDirectory(), CompileFolder);
            }
            return Path.Combine(GetRootDirectory(), BSharpBuilderDefaults.DefaultCompileDirectory);
        }

	    /// <summary>
		///     Возвращает нормализованный полный путь корневой папки репозитория или решения
		/// </summary>
		/// <returns></returns>
		public string GetRootDirectory(){
			if (string.IsNullOrWhiteSpace(RootDirectory)) return EnvironmentInfo.RootDirectory;
			return Path.GetFullPath(RootDirectory);
		}

		/// <summary>
		///     Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		public string GetOutputExtension(){
			if (string.IsNullOrWhiteSpace(OutputExtension)) return BSharpBuilderDefaults.DefaultOutputExtension;
			return OutputExtension;
		}


		/// <summary>
		///     Возвращает исходящее расширение
		/// </summary>
		/// <returns></returns>
		public string GetLogDirectory(){
			return Path.Combine(GetRootDirectory(), GetOutputDirectory()); // drop logs to the output directory 
		}
	}
}