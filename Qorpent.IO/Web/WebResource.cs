﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Результат выполнения запроса
	/// </summary>
	public class WebResource : IResource {
		/// <summary>
		/// Основной конструктор
		/// </summary>
		/// <param name="nativeResponse"></param>
		/// <param name="config"></param>
		public WebResource(WebResponse nativeResponse, IResourceConfig config = null) {
			NativeResponse = nativeResponse;
			Config = config;
		}
		/// <summary>
		/// Акцессор к исходному запросу
		/// </summary>
		public WebResponse NativeResponse { get; protected set; }

		/// <summary>
		/// Метаданные ресурса
		/// </summary>
		public IResourceConfig Config { get; protected set; }


		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		public async Task<byte[]> GetData() {
			var buffer = new byte[NativeResponse.ContentLength];
			using (var s = await Open()) {
				await s.ReadAsync(buffer, 0,(int) NativeResponse.ContentLength);
			}
			return buffer;
		}

		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		public Task<Stream> Open() {
			return Task.Run(() => NativeResponse.GetResponseStream());
		}


		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			NativeResponse = null;
			Config = null;
		}
	}

}