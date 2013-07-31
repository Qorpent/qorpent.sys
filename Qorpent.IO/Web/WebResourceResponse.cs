using System;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Обертка над Web-запросом
	/// </summary>
	public class WebResourceResponse:IResourceResponse {
		/// <summary>
		/// Текущее состояние отклика
		/// </summary>
		public ResourceResponseState State { get; private set; }

		/// <summary>
		/// Синхронный доступ к ресурсу
		/// </summary>
		/// <returns></returns>
		public IResource GetResource() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Асинхронный доступ к ресурсу
		/// </summary>
		/// <returns></returns>
		public Task<IResource> BeginGetResource() {
			throw new NotImplementedException();
		}
	}
}