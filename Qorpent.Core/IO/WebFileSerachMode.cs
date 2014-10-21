namespace Qorpent.IO{
	/// <summary>
	/// Режим поиска файла в провайдере
	/// </summary>
	public enum WebFileSerachMode{
		/// <summary>
		/// Точный
		/// </summary>
		Exact = 1,
		/// <summary>
		/// Сначала попытка поиска по полному пути, потом по маске
		/// </summary>
		ExactThenIgnore = 2,
		/// <summary>
		/// С игнорированием пути
		/// </summary>
		IgnorePath = 4
	}
}