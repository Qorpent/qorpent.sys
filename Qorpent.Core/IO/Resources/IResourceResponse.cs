using System;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс отклика ресурс
	/// </summary>
	public interface IResourceResponse:IDisposable {
		/// <summary>
		/// Текущее состояние отклика
		/// </summary>
		ResourceResponseState State { get; }
		/// <summary>
		/// Асинхронный доступ к ресурсу
		/// </summary>
		/// <returns></returns>
		Task<IResource> GetResource();
		
	}
}