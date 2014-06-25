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
	/// </summary>
	public class ExtendedPreprocessorTask : BSharpBuilderTaskBase{
		/// <summary>
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
		///     Выполнение цели
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			string message = Phase == BSharpBuilderPhase.PreProcess ? "предварительная" : "дополнительная";

			Project.Log.Warn("Выполняется " + message + " обработка кода ВНИМАНИЕ - данный функционал не считается безопасным");
			Stopwatch sw = Stopwatch.StartNew();
			//сначла собираем скрипты
			ProcessorScript[] scripts = ExtractScrtips(Project.Sources).OrderBy(_ => _.Index).ToArray();
			IList<Task> pendingScripts = new List<Task>();
			foreach (ProcessorScript script in scripts){
				if (script.Async){
					pendingScripts.Add(script.ExecuteAsync());
				}
				else{
					script.Execute();
				}
			}
			Task.WaitAll(pendingScripts.ToArray());
			sw.Stop();
			Project.Log.Warn(message + " обработка кода выполнялась " + sw.Elapsed);
		}

		private IEnumerable<ProcessorScript> ExtractScrtips(IEnumerable<XElement> sources){
			var result = new ConcurrentBag<ProcessorScript>();
			sources.AsParallel().ForAll(_ =>{
				foreach (ProcessorScript s in ExtractScripts(_)){
					result.Add(s);
				}
			});
			return result.OrderBy(_ => _.Source.Attr("order").ToInt());
		}

		private IEnumerable<ProcessorScript> ExtractScripts(XElement e){
			XElement usingElement = e.Elements("using").FirstOrDefault(); //вначае должен быть импорт псевдонима
			if (null == usingElement) yield break;
			string type = "Qorpent.BSharp.PreprocessScript";
			if (Phase != BSharpBuilderPhase.PreProcess){
				type = "Qorpent.BSharp.PostprocessScript";
			}
			XAttribute import = usingElement.Attributes().FirstOrDefault(_ => _.Value == type);
			if (null == import) yield break;
			string alias = import.Name.LocalName;
			int order = 1;
			foreach (XElement scripte in e.Elements(alias)){
				order += 10;
				scripte.SetAttributeValue("order", order);
				yield return new ProcessorScript(Project, scripte, Phase);
			}
		}
	}
}