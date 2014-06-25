using System.Collections.Generic;
using Qorpent.Log;

namespace Qorpent.BSharp.Builder{
	/// <summary>
	///     Базовый класс для расширений билдера B#
	/// </summary>
	public abstract class BSharpBuilderExtensionBase : IBSharpBuilderExtension{
		private BSharpBuilderBase _builder;
		private IUserLog _log;
		private IBSharpProject _project;
		private IList<IBSharpBuilderTask> _tasks;

		/// <summary>
		///     Акцессор к проекту
		/// </summary>
		protected IBSharpProject Project{
			get { return _project; }
		}

		/// <summary>
		///     Акцессор к журналу
		/// </summary>
		protected IUserLog Log{
			get { return _log; }
		}

		/// <summary>
		///     Акцессор к списку задач
		/// </summary>
		protected IList<IBSharpBuilderTask> Tasks{
			get { return _tasks; }
		}

		/// <summary>
		///     Акцессор к объекту построителя
		/// </summary>
		protected BSharpBuilderBase Builder{
			get { return _builder; }
		}

		/// <summary>
		///     Вызывается при присоединении расширения к билдеру
		/// </summary>
		/// <param name="builder"></param>
		public void SetUp(IBSharpBuilder builder){
			_builder = (BSharpBuilderBase) builder;
			_tasks = Builder.Tasks;
			_log = Builder.Log;
			_project = Builder.GetProject();
			WriteSetupStartLog();
			RefineProject();
			PrepareTasks();
			WriteSetupEndLog();
		}

		/// <summary>
		///     Запись в журнал при начале установки расширения
		/// </summary>
		protected virtual void WriteSetupEndLog(){
			Log.Info("SetUp finish " + GetType().Name);
		}

		/// <summary>
		///     Перекрыть при изменении в составе задач
		/// </summary>
		protected virtual void PrepareTasks(){
		}

		/// <summary>
		///     Перекрыть при необходимости внесения изменений в сам проект
		/// </summary>
		protected virtual void RefineProject(){
		}

		/// <summary>
		///     Запись об окончании подготовки билдера
		/// </summary>
		protected virtual void WriteSetupStartLog(){
			Log.Info("SetUp start " + GetType().Name);
		}
	}
}