using System;
using System.Linq;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Выполняет слияние 2-х репозиториев
	/// </summary>
	public class MetaFileRegistryMerger{
		class MergePair{
			/// <summary>
			/// Целевой файл
			/// </summary>
			public MetaFileDescriptor Target;
			/// <summary>
			/// История цели
			/// </summary>
			public RevisionDescriptor[] TargetHistory;
			/// <summary>
			/// Исходный файл
			/// </summary>
			public MetaFileDescriptor Source;
			/// <summary>
			/// История файла исходного
			/// </summary>
			public RevisionDescriptor[] SourceHistory;
			/// <summary>
			/// 
			/// </summary>
			public bool RequireUpdate{
				get{
					if (null == Target) return true;
					if (SourceHistory.Any(_ => TargetHistory.All(__ => __.RevisionTime < _.RevisionTime))) return true;
					return false;
				}
			}

			public RevisionDescriptor[] GetImportRevisions(MergeFlags flags){
				if (null == Target){
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
		/// <param name="target"></param>
		/// <param name="source"></param>
		/// <param name="flags"></param>
		public void Merge(IMetaFileRegistry target, IMetaFileRegistry source, MergeFlags flags = MergeFlags.Default){
			var pairs = GetMergingObjects(target, source);
			foreach (var mergePair in pairs){
				var importVersions = mergePair.GetImportRevisions(flags);
				foreach (var revisionDescriptor in importVersions.OrderBy(_=>_.RevisionTime)){
					var file = source.GetByRevision(mergePair.Source.Code, revisionDescriptor.Revision);
					target.Register(file);
				}
			}
		}

		private static MergePair[] GetMergingObjects(IMetaFileRegistry target, IMetaFileRegistry source){
			var targetCodes = target.GetCodes().ToArray();
			var pairs = source.GetCodes().Select(_ =>{
				var result = new MergePair();
				result.Source = source.GetCurrent(_);
				result.SourceHistory = source.GetRevisions(_).ToArray();
				if (-1 != Array.IndexOf(targetCodes, _)){
					result.Target = target.GetCurrent(_);
					result.TargetHistory = target.GetRevisions(_).ToArray();
				}
				return result;
			}).Where(_ => _.RequireUpdate).ToArray();
			return pairs;
		}
	}
}