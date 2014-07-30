namespace Qorpent.Data {
	/// <summary>
	///		Хинты кэша объектов
	/// </summary>
	public class ObjectDataCacheHints {
		/// <summary>
		///		Признак того, что не нужно вносить дочерние элементы в кэш
		/// </summary>
		public bool NoChildren { get; set; }
		/// <summary>
		/// Признак Key - запроса единичной сущности
		/// </summary>
		public bool KeyQuery { get; set; }
		/// <summary>
		/// Ключ для Key- query
		/// </summary>
		public object Key { get; set; }
		/// <summary>
		/// Пустой дефолтный хинт
		/// </summary>
		public static ObjectDataCacheHints Empty = new ObjectDataCacheHints();
		/// <summary>
		/// Стандартный блокировщик разрисовки по иерархии на том же объекте
		/// </summary>
		public static ObjectDataCacheHints NoChild = new ObjectDataCacheHints{NoChildren = true};
	}
}
