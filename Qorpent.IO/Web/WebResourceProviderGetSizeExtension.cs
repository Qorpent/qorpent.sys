using System;
using System.Net;
using Qorpent.IO.Resources;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Web {
    /// <summary>
    ///     Расширение для <see cref="IResourceProvider"/>, умеющее получать размер документа по Uri
    /// </summary>
    [ContainerComponent(Lifestyle = Lifestyle.Transient, Name = "getsize.provider.web", ServiceType = typeof(IResourceProviderExtension))]
    public class WebResourceProviderGetSizeExtension : ResourceProviderExtensionBase {
        /// <summary>
        ///     Расширение для <see cref="IResourceProvider"/>, умеющее получать размер документа по Uri
        /// </summary>
        public WebResourceProviderGetSizeExtension() {
            GetSizeSupported = true;
        }
        /// <summary>
        ///     Получение размера документа по его Uri
        /// </summary>
        /// <param name="uri">Uri документа</param>
        /// <returns>Размер документа</returns>
        public override int GetSize(Uri uri) {
            var webRequest = CreateWebRequest(uri);
            PrepareWebRequest(webRequest);
            return GetContentLength(webRequest);
        }
        /// <summary>
        ///     Проверяет возможность создания ресурса для указанного URI
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns></returns>
        public override bool IsSupported(Uri uri) {
            return true;
        }
        /// <summary>
        ///     Создание запроса по переданному Uri
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <returns></returns>
        private WebRequest CreateWebRequest(Uri uri) {
            return WebRequest.Create(uri);
        }
        /// <summary>
        ///     Подготовка запроса
        /// </summary>
        /// <param name="webRequest">Запрос</param>
        private void PrepareWebRequest(WebRequest webRequest) {
            webRequest.Method = "HEAD";
        }
        /// <summary>
        ///     Получение длины документа
        /// </summary>
        /// <param name="webRequest">Запрос</param>
        /// <returns>Размер документа</returns>
        private int GetContentLength(WebRequest webRequest) {
            return webRequest.GetResponse().Headers["Content-Length"].ToInt();
        }
    }
}
