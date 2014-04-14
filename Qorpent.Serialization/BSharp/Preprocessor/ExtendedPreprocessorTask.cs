using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.Utils.Extensions;
namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// 
	/// </summary>
	public class ExtendedPreprocessorTask : BSharpBuilderTaskBase{
		/// <summary>
		/// 
		/// </summary>
		public ExtendedPreprocessorTask(){
			Phase = BSharpBuilderPhase.PreProcess;
			Index = TaskConstants.LoadAllSourcesTaskIndex+10;
		}

		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			//сначла собираем скрипты
			var scripts = ExtractScrtips(Project.Sources).OrderBy(_=>_.Index).ToArray();
			IList<Task> pendingScripts = new List<Task>();
			foreach (var script in scripts){
				if (script.Async){
					pendingScripts.Add(script.ExecuteAsync());
				}
				else{
					script.Execute();
				}
			}
			Task.WaitAll(pendingScripts.ToArray());
		}

		private IEnumerable<PreprocessorScript> ExtractScrtips(IEnumerable<XElement> sources){
			var result = new ConcurrentBag<PreprocessorScript>();
			sources.AsParallel().ForAll(_ =>{
				foreach (var s in ExtractScripts(_)){
					result.Add(s);
				}
			});
			return result;
		}

		private IEnumerable<PreprocessorScript> ExtractScripts(XElement e){
			var usingElement = e.Elements("using").FirstOrDefault();//вначае должен быть импорт псевдонима
			if(null==usingElement)yield break;
			var import = usingElement.Attributes().FirstOrDefault(_ => _.Value == "Qorpent.BSharp.PreprocessScript");
			if(null==import)yield break;
			var alias = import.Name.LocalName;
			foreach (var scripte in e.Elements(alias)){
				yield return new PreprocessorScript(Project,scripte);
			}
		}
	}

	internal abstract class PreprocessorCommandBase{
		protected IBSharpProject _project;
		public int Index;
		public bool Async;
		public bool Parallel;
		protected XElement _e;

		/// <summary>
		/// 
		/// </summary>
		public PreprocessorCommandBase(IBSharpProject project,XElement e){
			_e = e;
			_project = project;
			Initialize();
		}

		protected virtual void Initialize(){
			_e.Apply(this);
		}

		/// <summary>
		/// выполняет скрипт асинхронно
		/// </summary>
		/// <returns></returns>
		public Task ExecuteAsync(XElement e=null){
			return Task.Run(()=>Execute(e));
		}

		/// <summary>
		/// Выполняет сам скрип
		/// </summary>
		public abstract void Execute(XElement e);
	}

	internal class PreprocessorCommand : PreprocessorCommandBase{
		/// <summary>
		/// 
		/// </summary>
		public PreprocessorCommand(IBSharpProject project,XElement e) : base(project, e){
		}
		/// <summary>
		/// Селектор
		/// </summary>
		public string For;
		/// <summary>
		/// Выполняет сам скрип
		/// </summary>
		public override void Execute(XElement e =null ){
			var srcdegree = Parallel ? Environment.ProcessorCount : 1;
			var selector = GetElements(e);
			selector.AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(el =>{
				foreach (var operation in Operations){
					operation.Execute(el);
				}
			});
		}
		protected override void Initialize()
		{
			base.Initialize();
			foreach (var element in _e.Elements()){
				var operation = PreprocessOperation.Create(element);
				if (null != operation){
					Operations.Add(operation);
				}
			}
		}

		private IEnumerable<XElement> GetElements(XElement src){
			if ("ROOTELEMENTS" == For) return src.Elements();
			if (string.IsNullOrWhiteSpace(For) || "ALLELEMENTS" == For) return src.Descendants();
			return src.XPathSelectElements(For);
		}

		public readonly IList<PreprocessOperation> Operations = new List<PreprocessOperation>();
	}

	internal abstract class PreprocessOperation{
		public static PreprocessOperation Create(XElement e){
			switch (e.Name.LocalName){
				case "renameattribute" :
					return e.Apply(new RenameAttributeOperation());
				case "renameelement":
					return e.Apply(new RenameElementOperation());
				default:
					throw new Exception("unkonown operation "+e.Name.LocalName);			
			}
		}

		public abstract void Execute(XElement el);
	}
	/// <summary>
	/// 
	/// </summary>
	internal class RenameAttributeOperation : PreprocessOperation{
	

		public override void Execute(XElement el){
			XAttribute target = el.Attribute(From);
			if (null !=target){
				target.Remove();
				el.SetAttributeValue(To,target.Value);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string To;
		/// <summary>
		/// 
		/// </summary>
		public string From;
	}

	/// <summary>
	/// 
	/// </summary>
	internal class RenameElementOperation : PreprocessOperation
	{


		public override void Execute(XElement el)
		{
			if (el.Name.LocalName == From){
				el.Name = To;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string To;
		/// <summary>
		/// 
		/// </summary>
		public string From;
	}

	/// <summary>
	/// Описывает скрипт препроцессора
	/// </summary>
	internal class PreprocessorScript : PreprocessorCommandBase{
		/// <summary>
		/// 
		/// </summary>
		public PreprocessorScript(IBSharpProject project,XElement e) : base(project, e){
		}
		protected override void Initialize()
		{
			base.Initialize();
			_e.Elements("command").DoForEach(_ => Commands.Add(new PreprocessorCommand(_project,_)));
		}

		readonly IList<PreprocessorCommand> Commands =new List<PreprocessorCommand>();
		public override void Execute(XElement e=null){
			var srcdegree = Parallel ? Environment.ProcessorCount : 1;
			var commands = Commands.OrderBy(_ => _.Index).ToArray();
			_project.Sources.AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(src =>
			{
				IList<Task> pending = new List<Task>();
				foreach (var command in commands){
					if (command.Async){
						pending.Add(command.ExecuteAsync(src));
					}
					else{
						command.Execute(src);
					}
				}
				Task.WaitAll(pending.ToArray());
			});
		}
	}


}