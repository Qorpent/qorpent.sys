using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Состояния запроса
	/// </summary>
	[Flags]
	public enum ResourceRequestState {
		/// <summary>
		/// Неопределенный
		/// </summary>
		Undefined=1,
		/// <summary>
		/// Создан
		/// </summary>
		Init =2,
		/// <summary>
		/// Создан
		/// </summary>
		Creating = 4,
		/// <summary>
		/// Создан
		/// </summary>
		Created = 8,
		/// <summary>
		/// В процессе работы
		/// </summary>
		Get =16,
		/// <summary>
		/// Завершен
		/// </summary>
		Finished = 32,
		/// <summary>
		/// Ошибка
		/// </summary>
		Error =64,
		/// <summary>
		/// Отправка сообщения на источник, для вебоподобных соединений (POST,PUT)
		/// </summary>
		Post = 128,
	}
}