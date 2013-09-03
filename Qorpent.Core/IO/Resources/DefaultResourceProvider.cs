using System;
using System.Linq;
using Qorpent.Config;
using Qorpent.IoC;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Обобщенный сервис, реализующий диспетчиризацию и загрузку ресурсов (чистый руль)
	/// </summary>	
	[ContainerComponent(ServiceType = typeof(IResourceProvider),Name = "resource.provider.default")]
	public class DefaultResourceProvider : ServiceBase, IResourceProvider {
		/// <summary>
		/// Устанавливает после загрузки из IoC себя в качестве родителя расширениям
		/// </summary>
		public override void OnContainerCreateInstanceFinished()
		{
			base.OnContainerCreateInstanceFinished();
			if (null != Extensions) {
				foreach (var e in Extensions) {
					e.SetParent(this);
				}
			}
		}
		private IResourceConfig _config;

		/// <summary>
		/// Расширения провайдера ресурсов - собственно компоненты, делающие дело
		/// </summary>
		[Inject]
		public IResourceProviderExtension[] Extensions { get; set; }

		/// <summary>
		/// Выполняет конфигурацию провайдера ресурсов
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public void Configure(IResourceConfig config = null) {
			_config = config;
			ConfigureExtensions();
		}

		private void ConfigureExtensions() {
			if (null != Extensions) {
				foreach (var e in Extensions) {
					e.Configure(_config);
				}
			}
		}

		/// <summary>
		/// Формирует объект запроса ресурса, который может использоваться для получения контента
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public IResourceRequest CreateRequest(Uri uri, IResourceConfig config = null)
		{
			if (null == Extensions || 0==Extensions.Length) throw new ResourceException("нет расширений для реализации запросов");
			var realuri = RewriteUri(uri);
			var resourcegetter = Extensions.FirstOrDefault(_ => _.IsCreateRequestSupported && _.IsSupported(realuri));
			if (null == resourcegetter) throw new ResourceException("нет расширений для реализации запроса "+uri+" => "+realuri);
			return resourcegetter.CreateRequest(uri, config);
		}

		/// <summary>
		/// Проверка, что Uri может быть обработан
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public bool IsSupported(Uri uri) {
			if (null == Extensions) return false;
			var realuri = RewriteUri(uri);
			return Extensions.Any(_ => _.IsCreateRequestSupported && _.IsSupported(realuri));
		}

		/// <summary>
		/// Акцессор к движку перезаписи адресов
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public Uri RewriteUri(Uri uri) {
			var realuri = uri;
			if (null != Extensions) {
				foreach (var e in Extensions.Where(_ => _.IsRewriteUriSupported)) {
					realuri = e.RewriteUri(realuri);
				}
			}
			return realuri;
		}

		/// <summary>
		/// Акцессор до конфигурации
		/// </summary>
		public IConfig Config { get { return _config; } }
	}
}