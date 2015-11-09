using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.IO;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
    /// <summary>
    ///     Базовые параметры консольных приложений
    /// </summary>
    public class ConsoleApplicationParameters : Scope {
        /// <summary>
        /// </summary>
        public ConsoleApplicationParameters() {
            LogFormat = "${Time} ${Message}";
        }

        /// <summary>
        ///     Исходный массив аргументов
        /// </summary>
        public string[] SourceArgs { get; set; }

        /// <summary>
        /// </summary>
        public bool ShadowByDefault { get; set; }

        /// <summary>
        ///     Первый анонимный атрибут
        /// </summary>
        public string Arg1
        {
            get { return Get("arg1", ""); }
            set { Set("arg1", value); }
        }

        /// <summary>
        ///     Второй анонимный атрибут
        /// </summary>
        public string Arg2
        {
            get { return Get("arg2", ""); }
            set { Set("arg2", value); }
        }

        /// <summary>
        ///     Запуск режима отладки
        /// </summary>
        public bool Debug
        {
            get { return Get("debug", false); }
            set { Set("debug", value); }
        }

        /// <summary>
        ///     Уровень журнала
        /// </summary>
        public LogLevel LogLevel
        {
            get
            {
                var val = this.ResolveBest("loglevel", "~ll").ToStr();
                if (string.IsNullOrWhiteSpace(val)) {
                    return LogLevel.Info;
                }
                return val.To<LogLevel>();
            }
            set { Set("loglevel", value); }
        }

        /// <summary>
        ///     Формат журнала
        /// </summary>
        public string LogFormat
        {
            get { return Get("logformat", ""); }
            set { Set("logformat", value); }
        }

        /// <summary>
        ///     Показать справку
        /// </summary>
        public bool Help
        {
            get { return Get("help", false); }
            set { Set("help", value); }
        }

        /// <summary>
        ///     Журнал
        /// </summary>
        public IUserLog Log { get; set; }

        /// <summary>
        ///     Параметр изменения текущей директории
        /// </summary>
        public string WorkingDirectory
        {
            get { return this.ResolveBestString("workingdirectory", "~wd", "workingdir"); }
            set { Set("workingdirectory", value); }
        }

        /// <summary>
        ///     Происхождение из другой директории (для BIN)
        /// </summary>
        public string ShadowEvidence
        {
            get { return Get("shadowevidence", ""); }
            set { Set("shadowevidence", value); }
        }

        /// <summary>
        ///     Происхождение из другой директории (для BIN)
        /// </summary>
        public string ConfigExtension
        {
            get { return Get("configextension", "bsconf"); }
            set { Set("configextension", value); }
        }


        /// <summary>
        ///     Путь к репозиторию
        /// </summary>
        public string RepositoryPath
        {
            get { return this.ResolveBestString("repositorypath", "~rp"); }
            set { Set("repositorypath", value); }
        }

        /// <summary>
        ///     Путь к репозиторию
        /// </summary>
        public string ManifestPath
        {
            get { return this.ResolveBestString("manifestpath", "mp"); }
            set { Set("manifestpath", value); }
        }

        /// <summary>
        ///     Признак явного отключения теневого запуска
        /// </summary>
        public bool NoShadow
        {
            get { return Get("noshadow", false); }
            set { Set("noshadow", value); }
        }

        /// <summary>
        ///     Признак явного включения теневого запуска
        /// </summary>
        public bool Shadow
        {
            get { return Get("shadow", false); }
            set { Set("shadow", value); }
        }

        /// <summary>
        ///     Проверка что это либо запуск без тени либо сама тень
        /// </summary>
        public bool IsReal
        {
            get
            {
                if (NoShadow) {
                    return true;
                }
                if (ShadowByDefault && string.IsNullOrWhiteSpace(ShadowEvidence)) {
                    return false;
                }
                return true;
            }
        }

        private ConfigurationOptions _configurationOptions;
        protected ConfigurationOptions ConfigurationOptions
        {
            get
            {
                if(null!=_configurationOptions)return _configurationOptions;
                
                if (Arg1.Contains(".")) { //it's path
                    var path = Arg1;
                    if (path.Contains("@")) {
                        path = EnvironmentInfo.ResolvePath(path);
                    }
                    if (!File.Exists(path)) {
                        throw new Exception("cannot find config file "+path);
                    }
                    var fullpath = Path.GetFullPath(path);
                    var name = Path.GetFileNameWithoutExtension(path);
                    var fileset = new FileSet(Path.GetDirectoryName(fullpath),Path.GetFileName(fullpath));
                    return _configurationOptions = new ConfigurationOptions {
                        Name = name,
                        FileSet = fileset
                    };
                }
                return _configurationOptions= new ConfigurationOptions {
                    Name = Arg1,
                    FileSet = ConfigSet ?? new FileSet(GetBSharpRoot(), "*." + ConfigExtension)
                };
            }
        }

        /// <summary>
        ///     Контекст B# при загрузке
        /// </summary>
        public IBSharpContext BSharpContext
        {
            get { return Get("bsharpcontext", (IBSharpContext) null); }
            set { Set("bsharpcontext", value); }
        }

        /// <summary>
        ///     Определение на B#
        /// </summary>
        public XElement Definition
        {
            get { return this.Ensure("defintion", new XElement("stub")); }
            set { Set("defintion", value); }
        }

        /// <summary>
        ///     В случае наличия - первый анонимный атрибут будет восприниматься как ссылка на код проекта B# в текущей директории
        /// </summary>
        public bool TreatAnonymousAsBSharpProjectReference { get; set; }

        /// <summary>
        ///     Суффикс теневой копии
        /// </summary>
        public string ShadowSuffix
        {
            get { return Get("shadowsuffix", ""); }
            set { Set("shadowsuffix", value); }
        }

        /// <summary>
        ///     True- при наличии уже ранее запущенной копии, новую не запускает
        /// </summary>
        public bool EnsureShadow
        {
            get { return Get("ensureshadow", false); }
            set { Set("ensureshadow", value); }
        }

        public FileSet ConfigSet { get; set; }

        /// <summary>
        ///     Отложенный конструктор, логика подготовки
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
            if (TreatAnonymousAsBSharpProjectReference && !string.IsNullOrWhiteSpace(Arg1) && IsReal) {
                try {
                    LoadFromBSharp(); // загружаем параметры из B#
                    LogFormat = LogFormat.Replace("@{", "${");
                    helper.Apply(arguments, this); //а потом перегружаем из аргументов (чтобы консоль перекрывала)
                }
                catch (Exception ex) {
                    Log.Fatal(ex.Message, ex, this);
                    throw;
                }
            }
            if (!string.IsNullOrWhiteSpace(RepositoryPath)) {
                EnvironmentInfo.LocalRepositoryDirectory = RepositoryPath;
            }
            if (!string.IsNullOrWhiteSpace(ShadowEvidence)) {
                EnvironmentInfo.ShadowEvidence = ShadowEvidence;
            }

            if (!string.IsNullOrWhiteSpace(ManifestPath)) {
                if (File.Exists(ManifestPath)) {
                    EnvironmentInfo.ManifestPath = ManifestPath;
                }
                else {
                    EnvironmentInfo.ConfigDirectory = ManifestPath;
                }
            }
            if (string.IsNullOrWhiteSpace(LogFormat)) {
                LogFormat = "${Time} ${Message}";
            }
            if (LogLevel == LogLevel.None) {
                LogLevel = LogLevel.Info;
            }
            Log.Level = LogLevel;
            if (Debug) {
                Log.Debug("debugger launched");
            }
            InternalInitialize(arguments);

            InternalCheckValid();


            DebugPrintArguments();
            return this;
        }

        /// <summary>
        ///     выводит отладочную информацию о параметрах
        /// </summary>
        protected virtual void DebugPrintArguments() {
            Log.Debug("Resolved call parameters:");
            foreach (var p in this) {
                Log.Debug(string.Format("{0,-20} : {1}", p.Key, p.Value));
            }
        }

        /// <summary>
        ///     Метод для проверки валидности параметров вызова
        /// </summary>
        protected virtual void InternalCheckValid() {
        }

        /// <summary>
        ///     Вспомогательный метод проверки наличия файла
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected bool CheckFile(string name, string value) {
            var isValid = true;
            if (string.IsNullOrWhiteSpace(value)) {
                Log.Error(name + " not set up");
                isValid = false;
            }
            else {
                if (!File.Exists(value)) {
                    Log.Error(name + "file '" + value + "'not exists");
                    isValid = false;
                }
            }
            return isValid;
        }

        /// <summary>
        ///     Вспомогательный метод для батчевой проверки валидности параметров в модели Assert
        /// </summary>
        /// <param name="message"></param>
        /// <param name="checks"></param>
        protected void Assert(string message, params bool[] checks) {
            if (!checks.All(_ => _)) {
                throw new Exception(message);
            }
        }


        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        protected virtual void InternalInitialize(string[] args) {
        }


        /// <summary>
        ///     Определяет файловый параметр с приведением его к полной нормализованной форме
        /// </summary>
        /// <param name="defaultExtension">Расширение по умолчанию</param>
        /// <param name="names"></param>
        /// <returns></returns>
        public string ResolveFileName(string defaultExtension, params string[] names) {
            var result = this.ResolveBestString(names);
            if (string.IsNullOrWhiteSpace(result)) {
                return result;
            }
            result = EnvironmentInfo.ResolvePath(result);
            if (!string.IsNullOrWhiteSpace(defaultExtension)) {
                if (Path.GetExtension(result) != defaultExtension) {
                    if (!File.Exists(result) && File.Exists(result + defaultExtension)) {
                        result += defaultExtension;
                    }
                }
            }
            return result;
        }


        /// <summary>
        ///     Производит загрузку из проекта B#
        /// </summary>
        protected virtual void LoadFromBSharp() {
            var config = new ConfigurationLoader(ConfigurationOptions).Load();
            BSharpContext = config.GetContext();
            var xml = config.GetConfig();
            xml.Apply(this);
            Definition = xml;
            foreach (var x in xml.Attributes()) {
                var name = x.Name.LocalName;
                if (!ContainsKey(name)) {
                    Set(name, x.Value);
                }
            }
            foreach (var x in xml.Elements()) {
                var name = x.Name.LocalName;
                if (!ContainsKey(name)) {
                    var val = x.Attr("code");
                    if (string.IsNullOrWhiteSpace(val)) {
                        val = x.Value;
                    }
                    Set(name, val);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected virtual string GetBSharpRoot() {
            return Environment.CurrentDirectory;
        }

        private bool _redirectconsole = false;
        private TextWriter _redirection;
        private ILogWriter _cachedLogWriter;
        private ILogWriter _redirectedWriter;
        private BaseLogger _redirectlogger;
        private ILoggy _loggy;
        private ILogAppender _redirectappender;
        private ILogAppender _cachedAppender;
        private ILoggyManager _redirectedManager;

        private void OnLoggyManager(ILoggyManager manager) {
            SetupLoggyRedirection(manager);
        }
        /// <summary>
        /// Set output buffer for log
        /// </summary>
        /// <param name="sw">text writer to send messages</param>
        /// <param name="redirect">overrides ConsoleOut</param>
        /// <remarks>test proposal, tend to be moved to Log but now we have ambigous log/loggy infrastructure so it has to manage it itself</remarks>
        public void RedirectLog(TextWriter sw, bool redirect = true) {
            if(null==sw)return;
           
            _redirectconsole = redirect;
            if (null != _redirection) {
                if (_redirection == sw) return;
                throw new Exception("already redirected");
            }
            _redirection = sw;
            var lb = (LoggerBasedUserLog) Log;
            var deflogger = lb.GetLoggers().OfType<BaseLogger>().FirstOrDefault();
            if (null != deflogger) {
                _redirectlogger = deflogger;
                _redirectedWriter = new TextWriterLogWriter(sw) {CustomFormat = LogFormat};
                _redirectlogger.Writers.Add(_redirectedWriter);
                if (_redirectconsole) {
                    _cachedLogWriter = _redirectlogger.Writers.OfType<ConsoleLogWriter>().FirstOrDefault();
                    if (null != _cachedLogWriter) {
                        _cachedLogWriter.Active = false;
                    }
                }
            }
            Loggy.OnChangeManager += OnLoggyManager;
            SetupLoggyRedirection(Loggy.Manager);
        }

        private void SetupLoggyRedirection(ILoggyManager manager) {
            if (_redirectedManager == manager) return;
            _redirectedManager = manager;
            _loggy = manager.Get();
            _redirectappender = new TextWriterAppender(_redirection) {Format = LogFormat};
            _loggy.Appenders.Add(_redirectappender);
            if (_redirectconsole) {
                _cachedAppender = _loggy.Appenders.OfType<ConsoleAppender>().FirstOrDefault();
                if (null != _cachedAppender) {
                    _cachedAppender.Active = false;
                }
            }
        }

        public void ResetRedirectLog() {
            if (null != _redirection) {
                if (null != _cachedLogWriter) {
                    _cachedLogWriter.Active = true;
                    _cachedLogWriter = null;
                    
                }
                if (_redirectlogger!=null && null != _redirectedWriter) {
                    _redirectedWriter.Active = false;
                    _redirectlogger.Writers.Remove(_redirectedWriter);
                }
                
                Loggy.OnChangeManager -= OnLoggyManager;
                if (null != _cachedAppender) {
                    _cachedAppender.Active = true;
                    _cachedAppender = null;
                }
                if (null != _redirectedManager) {
                    Loggy.Default.Appenders.Remove(_redirectappender);
                }
                _redirection.Flush();
                _redirection = null;
                _redirectedWriter = null;
                _redirectlogger = null;
                _redirectappender = null;
                _redirectconsole = false;
                _redirectedManager = null;
            }
        }

        /// <summary>
        /// Explicitly wait for log operation finish
        /// </summary>
        public void WaitLog() {
            Log.Synchronize();
            Loggy.Flush();
        }
    }
}