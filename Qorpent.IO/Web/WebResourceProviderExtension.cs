using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		/// 
		/// </summary>
		/// <param name="config"></param>
		protected override void InternalProcessConfig(IResourceProviderConfig config)
		{
			base.InternalProcessConfig(config);
		}
		/// <summary>
		/// Обрабатывает только http и https схемы
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public override bool IsMatchUri(Uri uri) {
			var scheme = uri.Scheme.ToLowerInvariant();
			return scheme == "http" || scheme == "https";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public override IResourceRequest InternalCreateRequest(Uri uri, IResourceRequestConfig config) {
			return new WebResourceRequest(uri, config);
		} 
	}
}
