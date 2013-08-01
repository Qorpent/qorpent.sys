using System;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Абстрактная основа для реализаций конкретных расширений провайдера ресурсов
	/// </summary>
	public abstract class ResourceProviderExtensionBase : IResourceProviderExtension {
		/// <summary>
		/// Акцессор до родительского провайдера
		/// </summary>
		protected IResourceProvider _parent;
		/// <summary>
		/// Акцессор к конфигу
		/// </summary>
		public IResourceProviderConfig _config;

		/// <summary>
		/// Выполняет конфигурацию провайдера ресурсов
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public void Configure(IResourceProviderConfig config = null) {
			_config = config;

			if (null != config) {
				InternalProcessConfig(config);
			}
		}
		/// <summary>
		/// Точка перекрытия - обработка конфига
		/// </summary>
		/// <param name="config"></param>
		protected virtual void InternalProcessConfig(IResourceProviderConfig config) {}

		/// <summary>
		/// Формирует объект запроса ресурса, который может использоваться для получения контента
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public IResourceRequest CreateRequest(Uri uri, IResourceConfig config = null) {
			if (!IsCreateRequestSupported) {
				throw new ResourceException("cannot retrieve request by settings");
			}
			if (!IsMatchUri(uri)) {
				throw new ResourceException("cannot process such uris "+uri);
			}
			return InternalCreateRequest(uri, config);
		}
		/// <summary>
		/// Точка перекрытия для создания реального запроса
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public virtual IResourceRequest InternalCreateRequest(Uri uri, IResourceConfig config)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Точка связывания с "родным" провайдером
		/// </summary>
		/// <param name="parentProvider"></param>
		public void SetParent(IResourceProvider parentProvider) {
			_parent = parentProvider;
		}


		/// <summary>
		/// Точка расширения для перезаписи Uri исходного запроса
		/// </summary>
		/// <param name="sourceUri"></param>
		/// <returns></returns>
		public virtual Uri RewriteUri(Uri sourceUri) {
			return sourceUri;
		}

		/// <summary>
		/// Флаговое свойство - признак поддержки перезаписи Url
		/// </summary>
		public bool IsRewriteUriSupported { get; protected set; }

		/// <summary>
		/// Флаговое свойство - признак поддержки создания реальных запросов
		/// </summary>
		public bool IsCreateRequestSupported { get; protected set; }

		/// <summary>
		/// Проверяет возможность создания ресурса для указанного URI
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public virtual bool IsMatchUri(Uri uri) {
			return false;
		}
	}
}