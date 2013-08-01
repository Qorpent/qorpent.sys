using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	///     Обертка над Web-запросом
	/// </summary>
	public class WebResourceRequest : IResourceRequest {
		/// <summary>
		///     Конфигурация
		/// </summary>
		protected IResourceRequestConfig Config;

		/// <summary>
		///     Сохраненный результат запроса
		/// </summary>
		protected IResourceResponse Response;

		/// <summary>
		///     Целевой адрес
		/// </summary>
		protected Uri Uri;

		/// <summary>
		///     Стандартный конструктор
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		public WebResourceRequest(Uri uri, IResourceRequestConfig config) {
			State = ResourceRequestState.Init;
			Config = config;
			Uri = uri;
		}

		/// <summary>
		///     Акцессор к последней ошибке
		/// </summary>
		public Exception LastError { get; protected set; }

		/// <summary>
		///     Текущее состояние запроса
		/// </summary>
		public ResourceRequestState State { get; private set; }

		/// <summary>
		///     Выполняет синхронный запрос ресурса
		/// </summary>
		/// <returns></returns>
		public async Task<IResourceResponse> GetResponse(IResourceResponseCallConfig config = null) {
			if (CheckCurrentCanBeReturned()) return Response;

			if (ResourceRequestState.Init == State) {
				if (null == Response) {
					Response = await InternalGetResponse(config);
					State = ResourceRequestState.Finished;
				}
			}

			if (ResourceRequestState.Error == State) {
				if (null == LastError) {
					LastError = new ResourceException("some error in request");
				}
				throw LastError;
			}

			return Response;
		}

		/// <summary>
		///     Получить параметр из конфигурации
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="code"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public T GetParameter<T>(string code, T def = default(T)) {
			if (null == Config) return def;
			return Config.Get(code, def);
		}

		private bool CheckCurrentCanBeReturned() {
			lock (this) {
				if (ResourceRequestState.Init == State) {
					return false;
				}
				else if (ResourceRequestState.Error == State) {
					if (null == LastError) {
						LastError = new ResourceException("some error in request");
					}
					throw LastError;
				}
				else if (ResourceRequestState.Finished == State) {
					{
						return true;
					}
				}
				else {
					throw new ResourceException("insuficient state for GetResponse call " + State);
				}

				return false;
			}
		}

		private async Task<IResourceResponse> InternalGetResponse(IResourceResponseCallConfig config) {
			try {
				State = ResourceRequestState.Creating;
				var nativeRequest = WebRequest.Create(Uri);
				SetupNativeRequest(nativeRequest, config);
				State = ResourceRequestState.Created;
				if (null != config.PostData) {
					State = ResourceRequestState.Post;
					using (var stream = new BinaryWriter(await nativeRequest.GetRequestStreamAsync())) {
						stream.Write(config.PostData, 0, config.PostData.Length);
					}
				}
				State = ResourceRequestState.Get;
				var nativeResponse = await nativeRequest.GetResponseAsync();

				return new WebResourceResponse(nativeResponse, nativeRequest, config);
			}
			catch (Exception ex) {
				State= ResourceRequestState.Error;
				LastError = new ResourceException("erorr in get response", ex);
			}
			return null;
		}

		/// <summary>
		///     Донастройка веб-запроса
		/// </summary>
		/// <param name="nativeRequest"></param>
		/// <param name="config"></param>
		protected virtual void SetupNativeRequest(WebRequest nativeRequest, IResourceResponseCallConfig config) {
			if (null == config) return;
			string method = config.Method;
			if (string.IsNullOrWhiteSpace(method)) {
				if (null != config.PostData) {
					method = "POST";
				}
				else {
					method = "GET";
				}
			}
			nativeRequest.Method = method;
		}

		public void Dispose() {
			Config = null;
			Response = null;
		}
	}
}