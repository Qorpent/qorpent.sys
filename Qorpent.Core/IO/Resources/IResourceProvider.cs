﻿using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	///		Общее описание службы получения контента
	/// </summary>
	public interface IResourceProvider {
		/// <summary>
		///		Выполняет конфигурацию провайдера ресурсов
		/// </summary>
		/// <param name="config">Конфигурация провайдера ресурсов</param>
		void Configure(IResourceConfig config = null);
		/// <summary>
		///		Формирует объект запроса ресурса, который может использоваться для получения контента
		/// </summary>
		/// <param name="uri">Uri документа</param>
		/// <param name="config">Конфигурация провайдера ресурсов</param>
		/// <returns>Запрос ресурса</returns>
		IResourceRequest CreateRequest(Uri uri, IResourceConfig config = null);
		/// <summary>
		///		Проверка, что Uri может быть обработан
		/// </summary>
		/// <param name="uri">Uri документа</param>
		/// <returns>Признак возможности обработки ресурса</returns>
		bool IsSupported(Uri uri);
        /// <summary>
        ///     Получение размера документа по его Uri
        /// </summary>
        /// <param name="uri">Uri документа</param>
        /// <returns>Размер документа</returns>
	    int GetSize(Uri uri);
	}
}
