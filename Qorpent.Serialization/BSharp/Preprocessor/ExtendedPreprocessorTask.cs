using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	/// 
	/// </summary>
	public class ExtendedPreprocessorTask : BSharpBuilderTaskBase{
		/// <summary>
		/// 
		/// </summary>
		public ExtendedPreprocessorTask(BSharpBuilderPhase phase){

			Phase = phase;
			if (phase == BSharpBuilderPhase.PreProcess){
				Index = TaskConstants.LoadAllSourcesTaskIndex + 10;
			}
			else{
				Index = TaskConstants.CompileBSharpTaskIndex + 10;
			}
		}

		/// <summary>
		/// Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			var message = Phase==BSharpBuilderPhase.PreProcess? "предварительная":"дополнительная";
			
			Project.Log.Warn("Выполняется "+message+" обработка кода ВНИМАНИЕ - данный функционал не считается безопасным");
			var sw = Stopwatch.StartNew();
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
			sw.Stop();
			Project.Log.Warn(message+" обработка кода выполнялась "+sw.Elapsed);
		}

		private IEnumerable<ProcessorScript> ExtractScrtips(IEnumerable<XElement> sources){
			var result = new ConcurrentBag<ProcessorScript>();
			sources.AsParallel().ForAll(_ =>{
				foreach (var s in ExtractScripts(_)){
					result.Add(s);
				}
			});
			return result.OrderBy(_=>_.Source.Attr("order").ToInt());
		}

		private IEnumerable<ProcessorScript> ExtractScripts(XElement e){
			var usingElement = e.Elements("using").FirstOrDefault();//вначае должен быть импорт псевдонима
			if(null==usingElement)yield break;
			var type = "Qorpent.BSharp.PreprocessScript";
			if (Phase != BSharpBuilderPhase.PreProcess){
				type = "Qorpent.BSharp.PostprocessScript";
			}
			var import = usingElement.Attributes().FirstOrDefault(_ => _.Value == type);
			if(null==import)yield break;
			var alias = import.Name.LocalName;
			int order = 1;
			foreach (var scripte in e.Elements(alias)){
				order += 10;
				scripte.SetAttributeValue("order",order);
				yield return new ProcessorScript(Project,scripte,Phase);
			}
		}
	}
}