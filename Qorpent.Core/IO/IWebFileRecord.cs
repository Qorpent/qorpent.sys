using System;
using System.IO;
using System.Text;

namespace Qorpent.IO{
	/// <summary>
	/// Интерфейс записи о файле для веб - приложений
	/// </summary>
	public interface IWebFileRecord{
		/// <summary>
		/// Локальное имя записи
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Версия записи
		/// </summary>
		DateTime Version { get; set; }

		/// <summary>
		/// Кэш-ключ записи
		/// </summary>
		string ETag { get; set; }

		/// <summary>
		/// Кодировка
		/// </summary>
		Encoding Encoding { get; set; }

		/// <summary>
		/// Записывает файл в целевой поток
		/// </summary>
		/// <param name="output"></param>
		long Write(Stream output);

		/// <summary>
		/// Возврат значения в виде массива байт
		/// </summary>
		/// <returns></returns>
		byte[] GetData();

		/// <summary>
		/// Возврат контента в виде строки
		/// </summary>
		/// <returns></returns>
		string Read();
		/// <summary>
		/// Полное имя
		/// </summary>
		string FullName { get; set; }
		/// <summary>
		/// Признак фиксированного контента
		/// </summary>
		bool IsFixedContent { get; set; }
		/// <summary>
		/// Фиксированный контент
		/// </summary>
		string FixedContent { get; set; }

		/// <summary>
		/// Фиксированные бинарные данные
		/// </summary>
		byte[] FixedData { get; set; }

		/// <summary>
		/// Тип MIME для файла
		/// </summary>
		string MimeType { get; set; }

	    string Role { get; set; }
	    long Length { get; set; }

	    /// <summary>
		/// Открытие потока на чтение
		/// </summary>
		/// <returns></returns>
		Stream Open();
	}
}