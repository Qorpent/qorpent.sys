using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс запроса
	/// </summary>
	public interface IResourceRequest {
		/// <summary>
		/// Текущее состояние запроса
		/// </summary>
		ResourceRequestState State { get; }
		/// <summary>
		/// Выполняет синхронный запрос ресурса
		/// </summary>
		/// <returns></returns>
		IResourceResponse GetResponse(IResourceResponseCallConfig config = null);
		/// <summary>
		/// Асинхронный интерфейс вызова запроса
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		Task<IResourceResponse> BeginGetResponse(IResourceResponseCallConfig config = null);
	}
}