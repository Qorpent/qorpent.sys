using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Config;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils{
	/// <summary>
	/// Базовые параметры консольных приложений
	/// </summary>
	public class ConsoleApplicationParameters:ConfigBase{
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
			get{
				return Get("loglevel", LogLevel.Info);
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
		public string WorkingDirectory
		{
			get { return Get("workingdirectory", ""); }
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
		/// Путь к репозиторию
		/// </summary>
		public string RepositoryPath
		{
			get { return Get("repositorypath", ""); }
			set { Set("repositorypath", value); }
		}

		/// <summary>
		/// Путь к репозиторию
		/// </summary>
		public string ManifestPath
		{
			get { return Get("manifestpath", ""); }
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
		/// Отложенный конструктор, логика подготовки 
		/// </summary>
		public virtual void Initialize(params string[] arguments){
			var helper = new ConsoleArgumentHelper();
			helper.Apply(arguments, this);
			Log = ConsoleLogWriter.CreateLog("main", LogLevel.Info, LogFormat);
			if (!string.IsNullOrWhiteSpace(WorkingDirectory)){
				Environment.CurrentDirectory = Path.GetFullPath(WorkingDirectory);
			}
			if (TreatAnonymousAsBSharpProjectReference && !string.IsNullOrWhiteSpace(Arg1)){
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
			if (!string.IsNullOrWhiteSpace(WorkingDirectory))
			{
				Environment.CurrentDirectory = Path.GetFullPath(WorkingDirectory);
				EnvironmentInfo.RootDirectory = Environment.CurrentDirectory;

			}
			if (!string.IsNullOrWhiteSpace(RepositoryPath)){
				EnvironmentInfo.LocalRepositoryDirectory = RepositoryPath;

			}
			if (!string.IsNullOrWhiteSpace(ShadowEvidence)){
				EnvironmentInfo.ShadowEvidence = ShadowEvidence;
			}
			
			if (!string.IsNullOrWhiteSpace(ManifestPath)){
				EnvironmentInfo.ConfigDirectory = ManifestPath;
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
				Debugger.Launch();
			}
			InternalInitialize(arguments);
			
		    InternalCheckValid();


            DebugPrintArguments();
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
        /// Определяет первый не пустой параметр из списка
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
	    public string Resolve(params string[] names) {
            foreach (var name in names) {
                var val = Get(name, "");
                if (!string.IsNullOrWhiteSpace(val)) return val;
            }
            return "";
        }

	    /// <summary>
	    /// Определяет файловый параметр с приведением его к полной нормализованной форме
	    /// </summary>
	    /// <param name="defaultExtension">Расширение по умолчанию</param>
	    /// <param name="names"></param>
	    /// <returns></returns>
	    public string ResolveFileName(string defaultExtension, params string[] names) {
            var result = Resolve(names);
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
			var bsconfigs = Directory.GetFiles(Environment.CurrentDirectory, "*.bsconf");
			if (0 == bsconfigs.Length){
				throw new Exception("No bsconf file found");
			}
			var compiler = WellKnownHelper.Create<IBSharpCompiler>();
			var bxl = WellKnownHelper.Create<IBxlParser>();
			var sources = bsconfigs.Select(_ => bxl.Parse(File.ReadAllText(_), _)).ToArray();
			var context = compiler.Compile(sources);
			var cls = context.Get(Arg1);
			if (null == cls){
				throw new Exception("cannot find config with name "+Arg1);
			}
			cls.Compiled.Apply(this);
			foreach (var x in cls.Compiled.Attributes())
			{
				
				var name = x.Name.LocalName;
				if (!this.options.ContainsKey(name)){
					Set(name, x.Value);
				}
			}
			foreach (var x in cls.Compiled.Elements()){
				var name = x.Name.LocalName;
				if (!options.ContainsKey(name)){
					var val = x.Attr("code");
					if (string.IsNullOrWhiteSpace(val)){
						val = x.Value;
					}
					Set(name, val);
				}
			}
			
		}

		/// <summary>
		/// В случае наличия - первый анонимный атрибут будет восприниматься как ссылка на код проекта B# в текущей директории
		/// </summary>
		public bool TreatAnonymousAsBSharpProjectReference { get; set; }
	}
}