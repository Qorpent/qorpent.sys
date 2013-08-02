﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
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
		protected IResourceConfig Config;

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
		public WebResourceRequest(Uri uri, IResourceConfig config) {
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
		public async Task<IResourceResponse> GetResponse(IResourceConfig config = null) {
			if (CheckCurrentCanBeReturned()) return Response;
			config = config ?? Config ?? ResourceConfig.Default;
			if (ResourceRequestState.Init == State) {
				if (null == Response) {
					Response = await InternalGetResponse(config);					
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
				if (ResourceRequestState.Error == State) {
					if (null == LastError) {
						LastError = new ResourceException("some error in request");
					}
					throw LastError;
				}
				if (ResourceRequestState.Finished == State) {
					{
						return true;
					}
				}
				throw new ResourceException("insuficient state for GetResponse call " + State);
			}
		}

		private async Task<IResourceResponse> InternalGetResponse(IResourceConfig config) {
			try {
				config = config ?? ResourceConfig.Default;
				State = ResourceRequestState.Creating;
				var nativeRequest = WebRequest.Create(Uri);
				SetupNativeRequest(nativeRequest, config);
				State = ResourceRequestState.Created;
				await PostDataToServer(config, nativeRequest);
				State = ResourceRequestState.Get;
				_acceptAllcertificates = config.AcceptAllCeritficates;
				var nativeResponse = await nativeRequest.GetResponseAsync();
				State = ResourceRequestState.Finished;
				return new WebResourceResponse(nativeResponse, nativeRequest, config);
			}
			catch (Exception ex) {
				State= ResourceRequestState.Error;
				LastError = new ResourceException("erorr in get response", ex);
			}
			return null;
		}

		private async Task PostDataToServer(IResourceConfig config, WebRequest nativeRequest) {
			if (null != config.RequestPostData) {
				State = ResourceRequestState.Post;
				using (var stream = new BinaryWriter(await nativeRequest.GetRequestStreamAsync())) {
					stream.Write(config.RequestPostData, 0, config.RequestPostData.Length);
				}
			}
		}

		/// <summary>
		///     Донастройка веб-запроса
		/// </summary>
		/// <param name="nativeRequest"></param>
		/// <param name="config"></param>
		private  void SetupNativeRequest(WebRequest nativeRequest, IResourceConfig config) {
			SetupHttpMethod(nativeRequest, config);
			nativeRequest.UseDefaultCredentials = true;
			nativeRequest.Proxy = ProxySelectorHelper.Select(Uri, config);
		}

		private static void SetupHttpMethod(WebRequest nativeRequest, IResourceConfig config) {
			var method = config.Method;
			if (string.IsNullOrWhiteSpace(method)) {
				method = null != config.RequestPostData ? "POST" : "GET";
			}
			nativeRequest.Method = method;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			Config = null;
			Response = null;
		}
		[ThreadStatic]
		private static bool _acceptAllcertificates = false;
		static WebResourceRequest() {
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;

		}

		private static bool ValidateServerCertficate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors) {
			if (sslpolicyerrors == SslPolicyErrors.None) return true;
			if (_acceptAllcertificates) return true;
			return false;
		}
	}
}