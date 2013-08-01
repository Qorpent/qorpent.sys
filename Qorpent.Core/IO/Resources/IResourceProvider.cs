using System;
using System.Collections.Generic;

namespace Qorpent.IO.Resources
{
	/// <summary>
	/// Общее описание службы получения контента
	/// </summary>
	public interface IResourceProvider {
		/// <summary>
		/// Выполняет конфигурацию провайдера ресурсов
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		void Configure(IResourceProviderConfig config = null);
		/// <summary>
		/// Формирует объект запроса ресурса, который может использоваться для получения контента
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		IResourceRequest CreateRequest(Uri uri, IResourceConfig config = null);
		/// <summary>
		/// Проверка, что Uri может быть обработан
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		bool IsSupported(Uri uri);
	}
}
