using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils{
	/// <summary>
	/// Базовые параметры консольных приложений
	/// </summary>
	public class ConsoleApplicationParameters:Scope{
		/// <summary>
		///		Исходный массив аргументов
		/// </summary>
		public string[] SourceArgs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ShadowByDefault { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public ConsoleApplicationParameters(){
			LogFormat = "${Time} ${Message}";
		}
		/// <summary>
		/// Первый анонимный атрибут
		/// </summary>
		public string Arg1 {
			get { return Get("arg1", ""); }
			set { Set("arg1", value); }
		}
        /// <summary>
        /// Второй анонимный атрибут
        /// </summary>
        public string Arg2
        {
            get { return Get("arg2", ""); }
            set { Set("arg2", value); }
        }
		/// <summary>
		/// Запуск режима отладки
		/// </summary>
		public bool Debug{
			get { return Get("debug", false); }
			set { Set("debug", value); }
		}
		/// <summary>
		/// Уровень журнала
		/// </summary>
		public LogLevel LogLevel{
			get {
			    var val = this.ResolveBest("loglevel", "~ll").ToStr();
			    if (string.IsNullOrWhiteSpace(val)) return LogLevel.Info;
			    return val.To<LogLevel>();
			}
			set { Set("loglevel", value); }
		}
		/// <summary>
		/// Формат журнала
		/// </summary>
		public string LogFormat
		{
			get { return Get("logformat", ""); }
			set { Set("logformat", value); }
		}
		/// <summary>
		/// Показать справку
		/// </summary>
		public bool Help
		{
			get { return Get("help", false); }
			set { Set("help", value); }
		}
		/// <summary>
		/// Журнал
		/// </summary>
		public IUserLog Log { get; set; }
		/// <summary>
		/// Параметр изменения текущей директории
		/// </summary>
		public string WorkingDirectory {
            get { return this.ResolveBestString("workingdirectory", "~wd", "workingdir"); }
			set { Set("workingdirectory", value); }
		}
		/// <summary>
		/// Происхождение из другой директории (для BIN)
		/// </summary>
		public string ShadowEvidence{
			get { return Get("shadowevidence", ""); }
			set { Set("shadowevidence", value); }
		}
        /// <summary>
        /// Происхождение из другой директории (для BIN)
        /// </summary>
        public string ConfigExtension
        {
            get { return Get("configextension", "bsconf"); }
            set { Set("configextension", value); }
        }


		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string RepositoryPath
		{
            get { return this.ResolveBestString("repositorypath", "~rp"); }
			set { Set("repositorypath", value); }
		}

		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string ManifestPath
		{
            get { return this.ResolveBestString("manifestpath", "mp"); }
			set { Set("manifestpath", value); }
		}
		/// <summary>
		/// Признак явного отключения теневого запуска
		/// </summary>
		public bool NoShadow{
			get { return Get("noshadow", false); }
			set {Set("noshadow",value);}
		}
		/// <summary>
		/// Признак явного включения теневого запуска
		/// </summary>
		public bool Shadow
		{
			get { return Get("shadow", false); }
			set { Set("shadow", value); }
		}
        /// <summary>
        /// Проверка что это либо запуск без тени либо сама тень
        /// </summary>
	    public bool IsReal {
            get {
                if (NoShadow) return true;
                if (ShadowByDefault && string.IsNullOrWhiteSpace(ShadowEvidence)) return false;
                return true;
            }
	    }

		/// <summary>
		/// Отложенный конструктор, логика подготовки 
		/// </summary>
		public virtual ConsoleApplicationParameters Initialize(params string[] arguments) {
			SourceArgs = arguments;
			var helper = new ConsoleArgumentHelper();
			helper.Apply(arguments, this);
		    if (!string.IsNullOrWhiteSpace(ShadowEvidence)) {
		        EnvironmentInfo.ShadowEvidence = ShadowEvidence;
		    }
			Log = ConsoleLogWriter.CreateLog("main", LogLevel.Info, LogFormat);
			if (!string.IsNullOrWhiteSpace(WorkingDirectory)) {
				var workingDirectory = Path.GetFullPath(WorkingDirectory);
				if (!Directory.Exists(workingDirectory)) {
					Directory.CreateDirectory(workingDirectory);
				}
				Environment.CurrentDirectory = workingDirectory;
				EnvironmentInfo.RootDirectory = Environment.CurrentDirectory;

			}
			if (TreatAnonymousAsBSharpProjectReference && !string.IsNullOrWhiteSpace(Arg1) && IsReal){
				try{
					LoadFromBSharp(); // загружаем параметры из B#
					LogFormat = LogFormat.Replace("@{", "${");
					helper.Apply(arguments, this); //а потом перегружаем из аргументов (чтобы консоль перекрывала)

				}
				catch (Exception ex){
					Log.Fatal(ex.Message,ex,this);
					throw;
				}
			}
			if (!string.IsNullOrWhiteSpace(RepositoryPath)){
				EnvironmentInfo.LocalRepositoryDirectory = RepositoryPath;

			}
			if (!string.IsNullOrWhiteSpace(ShadowEvidence)){
				EnvironmentInfo.ShadowEvidence = ShadowEvidence;
			}
			
			if (!string.IsNullOrWhiteSpace(ManifestPath)){
				if (File.Exists(ManifestPath)) {
					EnvironmentInfo.ManifestPath = ManifestPath;
				} else {
					EnvironmentInfo.ConfigDirectory = ManifestPath;
				}
			}
			if (string.IsNullOrWhiteSpace(LogFormat)){
				LogFormat = "${Time} ${Message}";
			}
			if (LogLevel == LogLevel.None){
				LogLevel = LogLevel.Info;
			}
			Log.Level = LogLevel;
			if (Debug){
				Log.Debug("debugger launched");
			}
			InternalInitialize(arguments);
			
		    InternalCheckValid();


            DebugPrintArguments();
		    return this;
		}
        /// <summary>
        /// выводит отладочную информацию о параметрах
        /// </summary>
	    protected virtual void DebugPrintArguments() {
	        Log.Debug("Resolved call parameters:");
	        foreach (var p in this) {
	            Log.Debug(string.Format("{0,-20} : {1}", p.Key, p.Value));
	        }
	    }

	    /// <summary>
        /// Метод для проверки валидности параметров вызова
        /// </summary>
	    protected virtual void InternalCheckValid() {
	    }

	    /// <summary>
	    /// Вспомогательный метод проверки наличия файла
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="value"></param>
	    protected bool CheckFile(string name, string value) {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(value))
            {
                Log.Error(name+" not set up");
                isValid = false;
            }
            else
            {
                if (!File.Exists(value))
                {
                    Log.Error(name+"file '"+value+"'not exists");
                    isValid = false;
                }
            }
            return isValid;
        }

	    /// <summary>
	    /// Вспомогательный метод для батчевой проверки валидности параметров в модели Assert
	    /// </summary>
	    /// <param name="message"></param>
	    /// <param name="checks"></param>
	    protected void Assert(string message,params bool[] checks) {
            if (!checks.All(_ => _)) {
                throw new Exception(message);
            }
	    }


	    /// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		protected virtual void InternalInitialize(string[] args){
			
		}
        

	    /// <summary>
	    /// Определяет файловый параметр с приведением его к полной нормализованной форме
	    /// </summary>
	    /// <param name="defaultExtension">Расширение по умолчанию</param>
	    /// <param name="names"></param>
	    /// <returns></returns>
	    public string ResolveFileName(string defaultExtension, params string[] names) {
            var result = this.ResolveBestString(names);
            if (string.IsNullOrWhiteSpace(result)) return result;
            result = EnvironmentInfo.ResolvePath(result);
            if (!string.IsNullOrWhiteSpace(defaultExtension)) {
                if (Path.GetExtension(result) != defaultExtension) {
                    if (!File.Exists(result) && File.Exists(result+defaultExtension)) {
                        result += defaultExtension;
                    }
                }
            }
            return result;
        }

		/// <summary>
		/// Производит загрузку из проекта B#
		/// </summary>
		protected virtual void LoadFromBSharp(){
			var bsconfigs = Directory.GetFiles(GetBSharpRoot(), "*."+ConfigExtension);
			if (0 == bsconfigs.Length){
				throw new Exception("No "+ConfigExtension+" file found");
			}
#if BRIDGE
		    var compiler = new BSharpCompiler();
		    var bxl = new BxlParser();
#else
			var compiler = WellKnownHelper.Create<IBSharpCompiler>();
			var bxl = new BxlParser();
#endif
			var sources = bsconfigs.Select(_ => bxl.Parse(File.ReadAllText(_), _)).ToArray();
			var context = compiler.Compile(sources,(IBSharpContext)null);
		    this.BSharpContext = context;
			var cls = context.Get(Arg1);
			if (null == cls){
				throw new Exception("cannot find config with name "+Arg1);
			}
		    this.Definition = cls.Compiled;
			cls.Compiled.Apply(this);
			foreach (var x in cls.Compiled.Attributes())
			{
				
				var name = x.Name.LocalName;
				if (!ContainsKey(name)){
					Set(name, x.Value);
				}
			}
			foreach (var x in cls.Compiled.Elements()){
				var name = x.Name.LocalName;
				if (!ContainsKey(name)){
					var val = x.Attr("code");
					if (string.IsNullOrWhiteSpace(val)){
						val = x.Value;
					}
					Set(name, val);
				}
			}
			
		}
        /// <summary>
        /// Контекст B# при загрузке
        /// </summary>
        public IBSharpContext BSharpContext
        {
            get { return Get("bsharpcontext",(IBSharpContext)null); }
            set
            {
                Set("bsharpcontext", value);
            }
        }

	    /// <summary>
	    /// Определение на B#
	    /// </summary>
	    public XElement Definition {
	        get { return this.Ensure("defintion", new XElement("stub")); }
	        set {
                Set("defintion", value);
	        }
	    }

	    /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
	    protected virtual string GetBSharpRoot() {
	        return Environment.CurrentDirectory;
	    }

	    /// <summary>
		/// В случае наличия - первый анонимный атрибут будет восприниматься как ссылка на код проекта B# в текущей директории
		/// </summary>
		public bool TreatAnonymousAsBSharpProjectReference { get; set; }

	    /// <summary>
	    /// Суффикс теневой копии
	    /// </summary>
	    public string ShadowSuffix {
            get { return Get("shadowsuffix", ""); }
            set
            {
                Set("shadowsuffix", value);
            }
	    }
        /// <summary>
        /// True- при наличии уже ранее запущенной копии, новую не запускает
        /// </summary>
        public bool EnsureShadow
        {
            get { return Get("ensureshadow", false); }
            set
            {
                Set("ensureshadow", value);
            }
        }
	}
}