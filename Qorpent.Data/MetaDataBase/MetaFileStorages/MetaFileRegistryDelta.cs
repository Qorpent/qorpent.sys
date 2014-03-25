using System.Linq;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public class MetaFileRegistryDelta{
		
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
		/// <summary>
		/// 
		/// </summary>
		public IMetaFileRegistry Source { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public IMetaFileRegistry Target { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
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
}