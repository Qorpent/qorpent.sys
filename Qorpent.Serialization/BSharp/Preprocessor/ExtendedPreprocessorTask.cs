using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;

namespace Qorpent.BSharp.Preprocessor{
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
}