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
		class MergePair{
		
			/// <summary>
			/// История цели
			/// </summary>
			public RevisionDescriptor[] TargetHistory;
		
			/// <summary>
			/// История файла исходного
			/// </summary>
			public RevisionDescriptor[] SourceHistory;
			/// <summary>
			/// 
			/// </summary>
			public bool RequireUpdate{
				get{
					if (null == TargetHistory) return true;
					if (SourceHistory.Any(_ => TargetHistory.All(__ => __.RevisionTime < _.RevisionTime))) return true;
					return false;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			public string Code { get; set; }

			public RevisionDescriptor[] GetImportRevisions(MergeFlags flags){
				if (null == TargetHistory){
					if (flags.HasFlag(MergeFlags.Snapshot)){
						return new[]{SourceHistory.OrderBy(_ => _.Revision).Last()};
					}
					return SourceHistory;
				}
				if (flags.HasFlag(MergeFlags.FullHistory)){
					//equal named revisions not copied in any case
					return SourceHistory.Where(_=>TargetHistory.All(__=>__.Revision!=_.Revision)).ToArray();
				}
				var result = 
					SourceHistory.Where(_ => TargetHistory.All(__ => __.RevisionTime < _.RevisionTime && __.Revision!=_.Revision)).OrderBy(_ => _.RevisionTime).ToArray();
				if (flags.HasFlag(MergeFlags.FullLateHistory)){
					return result;
				}
				return new[]{result.Last()};
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public string ExcludeRegex { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="flags"></param>
		public void Merge(IMetaFileRegistry target, IMetaFileRegistry source, MergeFlags flags = MergeFlags.Default){
			var pairs = GetMergingObjects(target, source);
			IList<Task> tasks = new List<Task>();
			foreach (var mergePair in pairs){
				tasks.Add(Task.Run(() =>{
					var importVersions = mergePair.GetImportRevisions(flags);
					foreach (var revisionDescriptor in importVersions.OrderBy(_ => _.RevisionTime)){
						var file = source.GetByRevision(mergePair.Code, revisionDescriptor.Revision);
						target.Register(file);
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
		private  IEnumerable<MergePair> GetMergingObjects(IMetaFileRegistry target, IMetaFileRegistry source){
			var targetCodes = target.GetCodes().ToArray();
			return source.GetCodes().Where(IsMatch).AsParallel().Select(_ =>
			{
				try{
					Stopwatch sw = null;
					if (Debug){
						Console.WriteLine(_);
						sw = Stopwatch.StartNew();
					}
					var result = new MergePair{Code = _, SourceHistory = source.GetRevisions(_).ToArray()};
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
			if (string.IsNullOrWhiteSpace(ExcludeRegex)){
				return true;
			}
			return !Regex.IsMatch(arg, ExcludeRegex);
		}
	}
}