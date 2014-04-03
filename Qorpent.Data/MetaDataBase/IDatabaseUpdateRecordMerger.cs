using System.Collections.Generic;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public interface IDatabaseUpdateRecordMerger{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		DatabaseUpdateRecord Merge(IEnumerable<DatabaseUpdateRecord> source);
	}
}