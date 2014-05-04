using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Qorpent.Bxl;
using Qorpent.IO.Resources;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.BSharp.Builder {
    /// <summary>
	/// Базовый билдер BSharp
	/// </summary>
	public abstract class BSharpBuilderBase : ServiceBase, IBSharpBuilder {

		/// <summary>
		/// Доступ к ресурсам
		/// </summary>
		[Inject]
		public IResourceProvider Resources {
			get { return _resources ?? (_resources = GetDefaultResources()); }
			set { _resources = value; }
		}
		/// <summary>
		/// Компилятор
		/// </summary>
		[Inject]
		public IBSharpCompiler Compiler {
			get { return _compiler ??(_compiler = GetDefaultCompiler()); }
			set { _compiler  = value; }
		}
		/// <summary>
		/// Цели построения
		/// </summary>
		public IList<IBSharpBuilderTask> Tasks {
			get { return _tasks ?? (_tasks = new List<IBSharpBuilderTask>()); }
			private set { _tasks = value; }
		}
        

		/// <summary>
		/// Компилятор
		/// </summary>
		[Inject]
		public IBxlParser Bxl
		{
			get { return _bxl ?? (_bxl = GetDefaultBxl()); }
			set { _bxl = value; }
		}
       

        /// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IBxlParser GetDefaultBxl() {
			return new BxlParser();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpCompiler GetDefaultCompiler() {
			return new BSharpCompiler();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IResourceProvider GetDefaultResources() {
			return new DefaultResourceProvider();
		}
		/// <summary>
		/// Ссылка на целевой исходный проект
		/// </summary>
		protected IBSharpProject Project;
		private IResourceProvider _resources;
		private IBSharpCompiler _compiler ;
		private IBxlParser _bxl;
		private IList<IBSharpBuilderTask> _tasks;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		public void Initialize(IBSharpProject project) {
			Project = project;
			PrepareTasks();
			PostInitialize();
            foreach (var t in Tasks)
            {
                t.SetProject(Project);
            }
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void PrepareTasks() {
			var _realproject = Project;
			if (Project.IsFullyQualifiedProject || string.IsNullOrWhiteSpace(Project.ProjectName)) {
				Log.Trace("load prepared project");
				PrepareTasksFromProject(Project);
			}
			else {
				Log.Trace("start compile projects");
				_realproject = Project.SafeOverrideProject( CompileRealProject());
				_realproject.SetParent(Project);
				Project = _realproject;
				PrepareTasksFromProject(_realproject);
				Log.Trace("internal project loaded");
			}
			
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="project"></param>
		protected abstract void PrepareTasksFromProject(IBSharpProject project);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpProject CompileRealProject() {
			var compiledProject = CompileInternalProject();
			return ConvertToBSharpBuilderProject(compiledProject);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiledProject"></param>
		/// <returns></returns>
		protected abstract IBSharpProject ConvertToBSharpBuilderProject(IBSharpContext compiledProject);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract IBSharpContext CompileInternalProject();

		/// <summary>
		/// 
		/// </summary>
		protected virtual void PostInitialize() {
		    PrepareExtensions();
		}
        /// <summary>
        /// Загрузка расширений
        /// </summary>
	    protected virtual void PrepareExtensions() {
            foreach (var ext in Project.Extensions) {
                foreach (var type in GetExtensionTypes(ext)) {
                    var extension = (IBSharpBuilderExtension)Activator.CreateInstance(type);
                    extension.SetUp(this);
                }    
            }
	    }
        /// <summary>
        /// Конвертирует строку с описанием сборки или типа в перечисление типов расширений
        /// </summary>
        /// <param name="extensionDescriptor"></param>
        /// <returns></returns>
	    private IEnumerable<Type> GetExtensionTypes(string extensionDescriptor) {
	        if (extensionDescriptor.IsLiteral(EscapingType.JsonLiteral)){
		        var pkg = Container.FindComponent(typeof (IBSharpBuilderExtension), extensionDescriptor + ".bsbext");
		        if (null != pkg){
			        yield return pkg.ImplementationType;
		        }
		        else{
			        throw new Exception("cannot find IBSharpBuilderExtension for " + extensionDescriptor + ".bsbext");
		        }
	        }
	        else{
		        if (extensionDescriptor.Contains(",")) yield return Type.GetType(extensionDescriptor);
		        else{
			        var assembly = Assembly.Load(extensionDescriptor);
			        foreach (var t in assembly.GetTypes()){
				        if (typeof (IBSharpBuilderExtension).IsAssignableFrom(t)){
					        if (!t.IsAbstract){
						        yield return t;
					        }
				        }
			        }
		        }
	        }
        }

	    /// <summary>
		/// Построить проект
		/// </summary>
		/// <returns></returns>
		public IBSharpContext Build() {
			Log.Trace("build start");
			var result = GetInitialContext();
			Log.Trace("initial context ready");
			PreProcess(result);
			PreVerify(result);
			try {
				PreBuild(result);
				InternalBuild(result);
				PostBuild(result);
			}
			catch (Exception e) {
				RegisterError(result, e);
			}
			PostVerify(result);
			PostProcess(result);
			Log.Trace("build finished");
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostVerify(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PostVerify);
		}
		IList<Task> pending = new List<Task>();
		private void ExecutePhase(IBSharpContext result, BSharpBuilderPhase phase) {
			Log.Trace("\tstart phase "+phase);
			var tasks = Tasks.Where(_ => _.Phase == phase).OrderBy(_ => _.Index).ToArray();
			foreach (var t in tasks) {
				Log.Trace("\t\t"+t.GetType().Name + " started");
				if (t.Async){
					pending.Add(Task.Run(()=>ExecuteTask(result,t)));
				}
				else{
					ExecuteTask(result, t);
				}
			}
			Task.WaitAll(pending.ToArray());
			Log.Trace("\tend phase " + phase);
		}

	    private void ExecuteTask(IBSharpContext result, IBSharpBuilderTask t){
		    t.Execute(result);
		    Log.Trace("\t\t" + t.GetType().Name + " executed");
	    }

	    /// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PreVerify(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PreVerify);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostBuild(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PostBuild);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual  void PreBuild(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PreBuild);
		
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PreProcess(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PreProcess);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostProcess(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.PostProcess);
		}

		/// <summary>
		/// Регистратор ошибок
		/// </summary>
		/// <param name="result"></param>
		/// <param name="e"></param>
		protected virtual void RegisterError(IBSharpContext result, Exception e) {
			result.RegisterError(BSharpErrors.Generic(e));
		}

		/// <summary>
		/// Внутернний метод билда
		/// </summary>
		/// <param name="result"></param>
		protected virtual void InternalBuild(IBSharpContext result) {
			ExecutePhase(result, BSharpBuilderPhase.Build);
		}

		/// <summary>
		/// Внутренний метод подготовки результата
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpContext GetInitialContext() {
			return new BSharpContext();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IBSharpProject GetProject() {
			return Project;
		}
	}
}