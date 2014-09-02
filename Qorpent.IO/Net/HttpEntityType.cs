using System;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Тип значения HTTP
	/// </summary>
	[Flags]
	public enum HttpEntityType{
		/// <summary>
		/// Неопределенное
		/// </summary>
		Undefined = 0,
		/// <summary>
		/// Определение метода
		/// </summary>
		Method = 1,
		/// <summary>
		/// Определение адреса
		/// </summary>
		Url = 1<<1,
		/// <summary>
		/// Версия протокола
		/// </summary>
		Version = 1<<2,
		/// <summary>
		/// Определение хоста
		/// </summary>
		Host = 1<<3,
		/// <summary>
		/// Статус отклика
		/// </summary>
		State = 1<<4,
		/// <summary>
		/// Имя статуса
		/// </summary>
		StateName = 1<<5,
		/// <summary>
		/// Имя заголовка
		/// </summary>
		HeaderName = 1<<6,
		/// <summary>
		/// Значение заголовка
		/// </summary>
		HeaderValue = 1<<7,
		/// <summary>
		/// Данные чанка
		/// </summary>
		Chunk = 1<<8,
		/// <summary>
		/// Начало контента
		/// </summary>
		ContentStart = 1<<9,
		/// <summary>
		/// Ошибка
		/// </summary>
		Error = 1<<29,
		/// <summary>
		/// Завершающая сущность
		/// </summary>
		Finish = 1<<30,
		
	}
}