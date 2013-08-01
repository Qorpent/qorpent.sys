using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Обертка над Web-запросом
	/// </summary>
	public class WebResourceResponse:IResourceResponse {
		
		/// <summary>
		/// Основной конструктор обертки над откликом
		/// </summary>
		/// <param name="nativeResponse"></param>
		/// <param name="nativeRequest"></param>
		/// <param name="config"></param>
		public WebResourceResponse(WebResponse nativeResponse, WebRequest nativeRequest, IResourceConfig config) {
			State = ResourceResponseState.Init;
			NativeResponse = nativeResponse;
			NativeRequest = nativeRequest;
			Config = config;
			var enc = nativeResponse.Headers[HttpResponseHeader.ContentEncoding];
			if (!string.IsNullOrWhiteSpace(enc)) {
				config.ResponseEncoding = Encoding.GetEncoding(enc);
			}
			
		}

		/// <summary>
		/// Конфигурация
		/// </summary>
		public IResourceConfig Config { get; protected set; }
		/// <summary>
		/// Доступ к системному запросу
		/// </summary>
		public WebRequest NativeRequest { get; protected set; }

		/// <summary>
		/// Доступ к системному отклику
		/// </summary>
		public WebResponse NativeResponse { get; protected set; }

		/// <summary>
		/// Текущее состояние отклика
		/// </summary>
		public ResourceResponseState State { get; protected set; }

		/// <summary>
		/// Кэшированный результат запроса
		/// </summary>
		protected IResource Resource;

		/// <summary>
		/// Синхронный доступ к ресурсу
		/// </summary>
		/// <returns></returns>
#pragma warning disable 1998
		public async Task<IResource> GetResource() {
#pragma warning restore 1998
			if (CheckCanReturnCurrent()) return Resource;
			if (ResourceResponseState.Init == State)
			{
				if (null == Resource)
				{
					Resource =  InternalGetResource();
					State = ResourceResponseState.Finished;
				}
			}

			if (ResourceResponseState.Error == State)
			{
				if (null == LastError)
				{
					LastError = new ResourceException("some error in request");
				}
				throw LastError;
			}

			return Resource;
		}

		private  IResource InternalGetResource() {
			State = ResourceResponseState.Get;
			var result = new WebResource(NativeResponse,Config);
			return result;
		}

		private bool CheckCanReturnCurrent() {
			if (ResourceResponseState.Init == State) {
				return false;
			}
			if (ResourceResponseState.Error == State)
			{
				if (null == LastError)
				{
					LastError = new ResourceException("some error in request");
				}
				throw LastError;
			}
			if (ResourceResponseState.Finished == State)
			{
				{
					return true;
				}
			}
			throw new ResourceException("insuficient state for GetResource call " + State);
		}

		/// <summary>
		/// Последнее сообщение об ошибке
		/// </summary>
		public Exception LastError { get; protected set; }


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			NativeRequest = null;
			NativeResponse = null;
			Config = null;
			Resource = null;

		}
	}
}