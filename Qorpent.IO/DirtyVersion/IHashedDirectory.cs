using System;
using System.Collections.Generic;
using System.IO;
using Qorpent.IO.DirtyVersion.Storage;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Интерфейс полноценной HashedDirectory
	/// </summary>
	public interface IHashedDirectory {
		/// <summary>
		/// Выполняет сохранение файла с формированием хэш -записи
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		HashedDirectoryRecord Write(string filename,string data);

		/// <summary>
		/// Выполняет сохранение файла с формированием хэш -записи
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		HashedDirectoryRecord Write(string filename, Stream data);

		/// <summary>
		/// Проверка на существовании в директории хэшированного файла
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		bool Exists(string filename);

		/// <summary>
		/// Проверка на существовании по записи
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		bool Exists(HashedDirectoryRecord record);

		/// <summary>
		/// Считывает последнюю версию файла
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		Stream Open(string filename);

		/// <summary>
		/// Считывает последнюю или указанную версию файла
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		Stream Open(HashedDirectoryRecord record);

		/// <summary>
		/// Возвращает реальный полный путь к файлу
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		string ResolveNativeFileName(HashedDirectoryRecord record);

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindLast(string filename);

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="beforeTime"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindLast(string filename, DateTime beforeTime);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindLast(HashedDirectoryRecord file);

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindFirst(string filename);

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fromTime"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindFirst(string filename, DateTime fromTime);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		HashedDirectoryRecord FindFirst(HashedDirectoryRecord file);

		/// <summary>
		/// Перечисляет все записи для указанного файла (хэши)
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		IEnumerable<HashedDirectoryRecord> EnumerateFiles(HashedDirectoryRecord file);

		/// <summary>
		/// Конвертирует хэш в путь
		/// </summary>
		/// <returns></returns>
		string ConvertToHasedFileName(string filename);

		/// <summary>
		/// Формирует хэш строки
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		string MakeHash(string filename);
	}
}