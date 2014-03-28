namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Структура привязки к объекту
	/// </summary>
	public struct ObjectKey{
		/// <summary>
		/// Таблица
		/// </summary>
		public string Table { get; set; }
		/// <summary>
		/// Идентификатор
		/// </summary>
		public int Id { get; set; }
		/// <summary>
		/// Код
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString(){
			return string.Format("{0}.{1}.{2}", Table, Id, Code);

		}
	}
}