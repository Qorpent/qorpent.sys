namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Мапинг иденичного поля  на внешнюю таблицу
	/// </summary>
	public struct TableMap{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="fromtable"></param>
		/// <param name="fromfield"></param>
		/// <param name="totable"></param>
		public TableMap(string fromtable, string fromfield, string totable){
			FromTable = fromtable;
			FromField = fromfield;
			ToTable = totable;
		}
		/// <summary>
		/// 
		/// </summary>
		public string ToTable;
		/// <summary>
		/// 
		/// </summary>
		public string FromField;
		/// <summary>
		/// 
		/// </summary>
		public string FromTable;
	}
}