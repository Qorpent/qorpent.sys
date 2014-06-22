namespace Qorpent.IO.Syncronization{
	/// <summary>
	/// Варианты команд для файла в целевой
	/// </summary>
	public enum FileOperation{
		/// <summary>
		/// Без действия
		/// </summary>
		None = 0,
		/// <summary>
		/// Создание
		/// </summary>
		Create =1,
		/// <summary>
		/// Обновление
		/// </summary>
		Update = 2,
		/// <summary>
		/// Удаление
		/// </summary>
		Delete = 4,
	}
}