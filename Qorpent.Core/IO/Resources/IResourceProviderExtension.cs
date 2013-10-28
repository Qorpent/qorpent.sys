using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Расширение провайдера ресурсов - основной интерфейс для имплементации
	/// </summary>
	public interface IResourceProviderExtension : IResourceProvider {
		/// <summary>
		/// Точка связывания с "родным" провайдером
		/// </summary>
		/// <param name="parentProvider"></param>
		void SetParent(IResourceProvider parentProvider);
		/// <summary>
		/// Точка расширения для перезаписи Uri исходного запроса
		/// </summary>
		/// <param name="sourceUri"></param>
		/// <returns></returns>
		Uri RewriteUri(Uri sourceUri);
		/// <summary>
		/// Флаговое свойство - признак поддержки перезаписи Url
		/// </summary>
		bool IsRewriteUriSupported { get; }
		/// <summary>
		/// Флаговое свойство - признак поддержки создания реальных запросов
		/// </summary>
		bool IsCreateRequestSupported { get; }
        /// <summary>
        ///     Поддерживает получение размера документа по Uri
        /// </summary>
        bool GetSizeSupported { get; }
	}
}