using System.Net.Sockets;
using System.Threading.Tasks;

namespace Qorpent.Host.Exe.SimpleSockets{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleSocketRequest<T,R>{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="socket"></param>
		public SimpleSocketRequest(Socket socket){
			this.Socket = socket;
		} 
		/// <summary>
		/// Ссылка на сокет
		/// </summary>
		public Socket Socket { get; private set; }

		private T _cachedQuery = default(T);
		/// <summary>
		/// Получить типизированный объект запроса
		/// </summary>
		/// <returns></returns>
		public async Task<T> GetQuery(){
			if (!Equals(default(T), _cachedQuery)) return _cachedQuery;
			return (_cachedQuery = await Socket.ReadAsync<T>());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		/// <param name="close"></param>
		public async Task Send(R result, bool close =false){
			await Socket.SendDataAsync(result);
			if (close){
				Socket.Close();
			}
			
		}
	}
}