using System;
using Qorpent.IO.Resources;
using Qorpent.IoC;

namespace Qorpent.IO.Web
{
	/// <summary>
	/// Расширение загрузчика ресурса
	/// </summary>
	[ContainerComponent(Lifestyle=Lifestyle.Transient,Name="resource.provider.web",ServiceType = typeof(IResourceProviderExtension))]
	public class WebResourceProviderExtension : ResourceProviderExtensionBase
	{
		/// <summary>
		///ctor 
		/// </summary>
		public WebResourceProviderExtension()
		{
			IsCreateRequestSupported = true;
		}

		/// <summary>
		/// Обрабатывает только http и https схемы
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public override bool IsSupported(Uri uri) {
			var scheme = uri.Scheme.ToLowerInvariant();
			return scheme == "http" || scheme == "https";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public override IResourceRequest InternalCreateRequest(Uri uri, IResourceConfig config) {
			return new WebResourceRequest(uri, config);
		} 
	}
}
