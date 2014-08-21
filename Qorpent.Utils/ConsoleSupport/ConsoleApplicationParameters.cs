using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Bxl;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils{
	/// <summary>
	/// Базовые параметры консольных приложений
	/// </summary>
	public class ConsoleApplicationParameters{
		/// <summary>
		/// Первый анонимный атрибут
		/// </summary>
		public string Arg1 { get; set; }
		/// <summary>
		/// Запуск режима отладки
		/// </summary>
		public bool Debug { get; set; }
		/// <summary>
		/// Уровень журнала
		/// </summary>
		public LogLevel LogLevel { get; set; }
		/// <summary>
		/// Формат журнала
		/// </summary>
		public string LogFormat { get; set; }
		/// <summary>
		/// Показать справку
		/// </summary>
		public bool Help { get; set; }
		/// <summary>
		/// Журнал
		/// </summary>
		public IUserLog Log { get; set; }
		/// <summary>
		/// Параметр изменения текущей директории
		/// </summary>
		public string WorkingDirectory { get; set; }
		/// <summary>
		/// Отложенный конструктор, логика подготовки 
		/// </summary>
		public virtual void Initialize(string[] arguments){
			var helper = new ConsoleArgumentHelper();
			Log = ConsoleLogWriter.CreateLog("main", LogLevel.Info, LogFormat);
			helper.Apply(arguments,this);
			if (!string.IsNullOrWhiteSpace(WorkingDirectory)){
				Environment.CurrentDirectory = Path.GetFullPath(WorkingDirectory);
			}
			if (TreatAnonymousAsBSharpProjectReference && !string.IsNullOrWhiteSpace(Arg1)){
				try{
					LoadFromBSharp(); // загружаем параметры из B#
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
		}

		/// <summary>
		/// В случае наличия - первый анонимный атрибут будет восприниматься как ссылка на код проекта B# в текущей директории
		/// </summary>
		public bool TreatAnonymousAsBSharpProjectReference { get; set; }
	}
}