using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Выполняет слияние 2-х репозиториев
	/// </summary>
	public class MetaFileRegistryMerger{
		/// <summary>
		/// 
		/// </summary>
		public string ExcludeRegex { get; set; }
		/// <summary>
		/// Пользовательский специальный мержер
		/// </summary>
		public ICustomMerger CustomMerger { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="flags"></param>
		public void Merge(IMetaFileRegistry target, IMetaFileRegistry source, MergeFlags flags = MergeFlags.Default ){
			var pairs = GetDelta(target, source);
			if(null!=pairs)
			foreach (var metaFileRegistryDelta in pairs){
				Console.WriteLine(metaFileRegistryDelta);
			}
			if (null != CustomMerger){
				CustomMerger.Merge(pairs);
			}
			Merge(flags, pairs);
		}
		/// <summary>
		/// Выполняет мерж с прямым указанием сводимых объектов
		/// </summary>
		/// <param name="flags"></param>
		/// <param name="pairs"></param>
		public  void Merge(MergeFlags flags, IEnumerable<MetaFileRegistryDelta> pairs){
			IList<Task> tasks = new List<Task>();
			foreach (var mergePair in pairs){
				tasks.Add(Task.Run(() =>{
					var importVersions = mergePair.GetImportRevisions(flags);
					foreach (var revisionDescriptor in importVersions.OrderBy(_ => _.RevisionTime)){
						var file = mergePair.Source.GetByRevision(mergePair.Code, revisionDescriptor.Revision);
						mergePair.Target.Register(file);
					}
				}
					          ));
			}
			Task.WaitAll(tasks.ToArray());
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Debug { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string IncludeRegex { get; set; }

		/// <summary>
		/// Расчитывает разницу между актуальныйм и сводимым репозиторием
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <returns></returns>
		public IEnumerable<MetaFileRegistryDelta> GetDelta(IMetaFileRegistry target, IMetaFileRegistry source){
			var targetCodes = target.GetCodes().ToArray();
			return source.GetCodes().Where(IsMatch).AsParallel().Select(_ =>
			{
				try{
					Stopwatch sw = null;
					if (Debug){
						Console.WriteLine(_);
						sw = Stopwatch.StartNew();
					}
					var result = new MetaFileRegistryDelta {Target = target,Source = source,Code = _, SourceHistory = source.GetRevisions(_).ToArray()};
					if (Debug){
						sw.Stop();
						Console.WriteLine(sw.Elapsed);
					}
					if (-1 != Array.IndexOf(targetCodes, _)){
						result.TargetHistory = target.GetRevisions(_).ToArray();
					}
					return result;
				}
				catch (Exception e){
					throw new Exception("catched!\r\n"+ e.ToString());
				}
			}).Where(_ => _.RequireUpdate).ToArray();
		}

		private bool IsMatch(string arg){
			
			if (!string.IsNullOrWhiteSpace(ExcludeRegex) && Regex.IsMatch(arg, ExcludeRegex)) return false;
			if (!string.IsNullOrWhiteSpace(IncludeRegex) && !Regex.IsMatch(arg, IncludeRegex)) return false;
			return true;
		}
	}
}