using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Qorpent.BSharp.Preprocessor;
using Qorpent.Bxl;
using Qorpent.IO.Resources;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;
#if EMBEDQPT
using IBxlParser = Qorpent.Bxl.BxlParser;
#endif
namespace Qorpent.BSharp.Builder{
	/// <summary>
	///     Базовый билдер BSharp
	/// </summary>
	public abstract class BSharpBuilderBase : ServiceBase, IBSharpBuilder{
		private readonly IList<Task> pending = new List<Task>();

		/// <summary>
		///     Ссылка на целевой исходный проект
		/// </summary>
		protected IBSharpProject Project;

		private IBxlParser _bxl;
		private IBSharpCompiler _compiler;
		private IResourceProvider _resources;
		private IList<IBSharpBuilderTask> _tasks;

		/// <summary>
		///     Доступ к ресурсам
		/// </summary>
		[Inject]
		public IResourceProvider Resources{
			get { return _resources ?? (_resources = GetDefaultResources()); }
			set { _resources = value; }
		}

		/// <summary>
		///     Цели построения
		/// </summary>
		public IList<IBSharpBuilderTask> Tasks{
			get { return _tasks ?? (_tasks = new List<IBSharpBuilderTask>()); }
			private set { _tasks = value; }
		}


		/// <summary>
		///     Компилятор
		/// </summary>
		[Inject]
		public IBxlParser Bxl{
			get { return _bxl ?? (_bxl = GetDefaultBxl()); }
			set { _bxl = value; }
		}

		/// <summary>
		///     Компилятор
		/// </summary>
		[Inject]
		public IBSharpCompiler Compiler{
			get { return _compiler ?? (_compiler = GetDefaultCompiler()); }
			set { _compiler = value; }
		}

		/// <summary>
		/// </summary>
		/// <param name="project"></param>
		public void Initialize(IBSharpProject project){
			Project = project;
			PrepareTasks();
			PostInitialize();
			foreach (IBSharpBuilderTask t in Tasks){
				t.SetProject(Project);
			}
		}

		/// <summary>
		///     Построить проект
		/// </summary>
		/// <returns></returns>
		public IBSharpContext Build(){
			Log.Trace("build start");
			IBSharpContext result = GetInitialContext();
			Project.Context = result;
			Log.Trace("initial context ready");
			PreProcess(result);
			PreVerify(result);
			try{
				PreBuild(result);
				InternalBuild(result);
				PostBuild(result);
			}
			catch (Exception e){
				RegisterError(result, e);
			}
			PostVerify(result);
			PostProcess(result);
			Log.Trace("build finished");
			Task.WaitAll(pending.ToArray());
			return result;
		}


		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected virtual IBxlParser GetDefaultBxl(){
			return new BxlParser();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpCompiler GetDefaultCompiler(){
			return new BSharpCompiler();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected virtual IResourceProvider GetDefaultResources(){
			return new DefaultResourceProvider();
		}

		/// <summary>
		/// </summary>
		protected virtual void PrepareTasks(){
			IBSharpProject _realproject = Project;
			if (Project.IsFullyQualifiedProject || string.IsNullOrWhiteSpace(Project.ProjectName)){
				Log.Trace("load prepared project");
				PrepareTasksFromProject(Project);
			}
			else{
				Log.Trace("start compile projects");
				_realproject = Project.SafeOverrideProject(CompileRealProject());
				_realproject.SetParent(Project);
				Project = _realproject;
				PrepareTasksFromProject(_realproject);
				Log.Trace("internal project loaded");
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="project"></param>
		protected abstract void PrepareTasksFromProject(IBSharpProject project);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpProject CompileRealProject(){
			IBSharpContext compiledProject = CompileInternalProject();
			
			return ConvertToBSharpBuilderProject(compiledProject);
		}

		/// <summary>
		/// </summary>
		/// <param name="compiledProject"></param>
		/// <returns></returns>
		protected abstract IBSharpProject ConvertToBSharpBuilderProject(IBSharpContext compiledProject);

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected abstract IBSharpContext CompileInternalProject();

		/// <summary>
		/// </summary>
		protected virtual void PostInitialize(){
			PrepareExtensions();
		}

		/// <summary>
		///     Загрузка расширений
		/// </summary>
		protected virtual void PrepareExtensions(){
		    if (Project.DoCompileExtensions)
		    {
		        var assembly = DoCompileExtensions(Project.GetCompileDirectory());
		        if (null != assembly)
		        {

		            foreach (Type ext in assembly.GetTypes())
		            {
		                if (typeof (IBSharpBuilderExtension).IsAssignableFrom(ext))
		                {
		                    var extension = (IBSharpBuilderExtension) Activator.CreateInstance(ext);
		                    Log.Info("Register extension: " + ext.FullName);
		                    extension.SetUp(this);
		                }
		            }
		        }
		    }
			foreach (string ext in Project.Extensions){
				foreach (Type type in GetExtensionTypes(ext)){
					var extension = (IBSharpBuilderExtension) Activator.CreateInstance(type);
                    Log.Info("Register extension: " + type.FullName);
                    extension.SetUp(this);
				}
			}
		}

	    private Assembly DoCompileExtensions(string compiledirectory) {
	        Log.Info("Приступаю к компиляции расширений");
            var codeProvider = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } });
	        var assemblies = new[] {Assembly.GetEntryAssembly().GetName()};

            var assemblyReferences = new[]
           {
                "System.dll",
                "System.Core.dll",
                "System.Xml.dll",
                "mscorlib.dll"
            }
           .Union(from ass in assemblies
                  select new Uri(ass.CodeBase).LocalPath)
           .Union(from ass in Assembly.GetEntryAssembly().GetReferencedAssemblies()
                  where ass.Name!="WindowsBase"
                  select ass.Name.StartsWith("System")||ass.Name.StartsWith("ms") || ass.Name.StartsWith("Microsoft")? ass.Name+".dll" :
                  Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetEntryAssembly().CodeBase).LocalPath),ass.Name+".dll"))
                
           .Distinct(StringComparer.OrdinalIgnoreCase)
           .ToArray();

	        foreach (var assemblyReference in assemblyReferences) {
	            Console.WriteLine("ass: "+assemblyReference);
	        }

            var tmp = Path.Combine(Project.RootDirectory, "tools/.tmp");
            Directory.CreateDirectory(tmp);
            var file = Path.Combine(tmp, "bsc_dyn.dll");
            if (File.Exists(file))
            {
                File.Delete(file);
            }

	        CompilerParameters cp = new CompilerParameters {
	            GenerateInMemory = true,
	            OutputAssembly = file,
	            IncludeDebugInformation = true,
	            TempFiles = new TempFileCollection(tmp)
	        };

	        cp.ReferencedAssemblies.AddRange(assemblyReferences);
	        var fp = Path.GetFullPath(compiledirectory).NormalizePath();
	        if (!Directory.Exists(fp)) return null;
	        var files = Directory.GetFiles(fp, "*.cs", SearchOption.AllDirectories)
	            .Where(_ =>
	                !_.NormalizePath().Replace(fp,"").Contains("/tests/")
	                &&
	                (Project.Conditions.ContainsKey("dev") || !_.NormalizePath().Replace(fp, "").Contains("/dev/"))
	            ).ToArray();
            Project.Log.Debug("Compiled directory: "+fp);
            Project.Log.Debug("Files: "+string.Join(", ",files.Select(Path.GetFileName)) );
            var compilerResults = codeProvider.CompileAssemblyFromFile(cp, 
                files
                );
            if (compilerResults.Errors.HasErrors)
            {
                Log.Error("Errors in compilation!");
                foreach (var error in compilerResults.Errors) {
                    Log.Error(error.ToString());
                }
               
                throw new ApplicationException("Exit without plugins");
            }
            return compilerResults.CompiledAssembly;
        }

        /// <summary>
        ///     Конвертирует строку с описанием сборки или типа в перечисление типов расширений
        /// </summary>
        /// <param name="extensionDescriptor"></param>
        /// <returns></returns>
        private IEnumerable<Type> GetExtensionTypes(string extensionDescriptor){
			if (extensionDescriptor.IsLiteral(EscapingType.JsonLiteral)){
				IComponentDefinition pkg = Container.FindComponent(typeof (IBSharpBuilderExtension), extensionDescriptor + ".bsbext");
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
					Assembly assembly = Assembly.Load(extensionDescriptor);
					foreach (Type t in assembly.GetTypes()){
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
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostVerify(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PostVerify);
		}

		private void ExecutePhase(IBSharpContext result, BSharpBuilderPhase phase){
			Log.Trace("\tstart phase " + phase);
			IBSharpBuilderTask[] tasks = Tasks.Where(_ => _.Phase == phase).OrderBy(_ => _.Index).ToArray();
			foreach (IBSharpBuilderTask t in tasks){
				Log.Trace("\t\t" + t.GetType().Name + " started");
				if (t.Async){
					pending.Add(Task.Run(() => ExecuteTask(result, t)));
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
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PreVerify(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PreVerify);
		}

		/// <summary>
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostBuild(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PostBuild);
		}

		/// <summary>
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PreBuild(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PreBuild);
		}

		/// <summary>
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PreProcess(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PreProcess);
		}

		/// <summary>
		/// </summary>
		/// <param name="result"></param>
		protected virtual void PostProcess(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.PostProcess);
		}

		/// <summary>
		///     Регистратор ошибок
		/// </summary>
		/// <param name="result"></param>
		/// <param name="e"></param>
		protected virtual void RegisterError(IBSharpContext result, Exception e){
			result.RegisterError(BSharpErrors.Generic(e));
		}

		/// <summary>
		///     Внутернний метод билда
		/// </summary>
		/// <param name="result"></param>
		protected virtual void InternalBuild(IBSharpContext result){
			ExecutePhase(result, BSharpBuilderPhase.Build);
		}

		/// <summary>
		///     Внутренний метод подготовки результата
		/// </summary>
		/// <returns></returns>
		protected virtual IBSharpContext GetInitialContext(){
			return new BSharpContext();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public IBSharpProject GetProject(){
			return Project;
		}
	}
}