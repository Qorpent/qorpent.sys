using System;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс запроса
	/// </summary>
	public interface IResourceRequest:IDisposable {
		/// <summary>
		///		Текущее состояние запроса
		/// </summary>
		ResourceRequestState State { get; }
		/// <summary>
		///		Выполняет асинхронный запрос ресурса
		/// </summary>
		/// <returns>Задача на получение ресурса</returns>
		Task<IResourceResponse> GetResponse(IResourceConfig config = null);
		
	}
}