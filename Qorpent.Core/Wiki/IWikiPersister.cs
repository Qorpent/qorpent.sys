﻿using System;
using System.Collections.Generic;

namespace Qorpent.Wiki {
	/// <summary>
	/// Интерфейс, идентичный IWikiSource, но отвечающий за сохранение в хранилище
	/// </summary>
	public interface IWikiPersister {
		/// <summary>
		/// Возвращает полностью загруженные страницы Wiki
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Get(params string[] codes);
		/// <summary>
		/// Возвращает страницы, только с загруженным признаком хранения в БД
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> Exists(params string[] codes);
		/// <summary>
		/// Метод сохранения изменений в страницу
		/// </summary>
		/// <param name="pages"></param>
		void Save(params WikiPage[] pages);

		/// <summary>
		/// Сохраняет в Wiki файл с указанным кодом
		/// </summary>

		void SaveBinary(WikiBinary binary);

		/// <summary>
		/// Загружает бинарный контент
		/// </summary>
		/// <param name="code"></param>
		/// <param name="withData">Флаг, что требуется подгрузка бинарных данных</param>
		/// <returns></returns>
		WikiBinary LoadBinary(string code, bool withData = true);

		/// <summary>
		/// Поиск страниц по маске 
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IEnumerable<WikiPage> FindPages(string search);

		/// <summary>
		/// Поиск файлов по маске
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		IEnumerable<WikiBinary> FindBinaries(string search);

		/// <summary>
		/// Возвращает версию файла
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		DateTime GetBinaryVersion(string code);
		/// <summary>
		/// Возвращает версию страницы
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		DateTime GetPageVersion(string code);
	}
}