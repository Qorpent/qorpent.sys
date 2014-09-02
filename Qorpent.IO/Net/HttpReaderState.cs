namespace Qorpent.IO.Net{
	/// <summary>
	/// Состояния сканера HTTP
	/// </summary>
	public enum HttpReaderState{
		/// <summary>
		/// Начальное положение каретки
		/// </summary>
		Start = 0,
		/// <summary>
		/// Преамбула HTTP
		/// </summary>
		Preamble = 1,
		/// <summary>
		/// Версия HTTP
		/// </summary>
		Version =2,
		/// <summary>
		/// Чтение статуса
		/// </summary>
		State = 4,
		/// <summary>
		/// Имя статуса
		/// </summary>
		StateName = 8,
		/// <summary>
		/// Начало контента
		/// </summary>
		Content =16,
		/// <summary>
		/// Позиция перед заголовком
		/// </summary>
		PreHeader = 32,
		/// <summary>
		/// Позиция перед значением хидера
		/// </summary>
		PreHeaderValue = 64,
		/// <summary>
		/// Статус ошибка
		/// </summary>
		Error = 1<<28,
		/// <summary>
		/// Статус - завершено
		/// </summary>
		Finish = 1<<29,


		
	}
}