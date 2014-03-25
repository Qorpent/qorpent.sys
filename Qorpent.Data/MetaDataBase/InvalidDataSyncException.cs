using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class InvalidDataSyncException : Exception{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="records"></param>
		public InvalidDataSyncException(IEnumerable<DatabaseUpdateRecord> records){
			this.Records = records.ToArray();
		}
		/// <summary>
		/// 
		/// </summary>
		public DatabaseUpdateRecord[] Records { get; private set; }
	}
}