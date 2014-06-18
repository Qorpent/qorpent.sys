using System.Net.Sockets;
using System.Threading.Tasks;

namespace Qorpent.Host{
	/// <summary>
	/// Интерфейс реквеста простого сокета
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="R"></typeparam>
	public interface ISimpleSocketRequest<T, R>{
		/// <summary>
		/// Ссылка на сокет
		/// </summary>
		Socket Socket { get; }

		/// <summary>
		/// Получить типизированный объект запроса
		/// </summary>
		/// <returns></returns>
		Task<T> GetQuery();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <param name="close"></param>
		Task Send(R result, bool close =false);
	}
}