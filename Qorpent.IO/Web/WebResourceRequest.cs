using System;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Обертка над Web-запросом
	/// </summary>
	public class WebResourceRequest : IResourceRequest {
		/// <summary>
		/// Стандартный конструктор
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		public WebResourceRequest(Uri uri, IResourceRequestConfig config) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Текущее состояние запроса
		/// </summary>
		public ResourceRequestState State { get; private set; }

		/// <summary>
		/// Выполняет синхронный запрос ресурса
		/// </summary>
		/// <returns></returns>
		public IResourceResponse GetResponse(IResourceResponseCallConfig config = null) {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Асинхронный интерфейс вызова запроса
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public Task<IResourceResponse> BeginGetResponse(IResourceResponseCallConfig config = null) {
			//TODO: это временная затычка!!! по идее надо нормально обернуть асинхронный режим HttpWebRequest
			return Task.Run(() => GetResponse());
		}
	}
}