using System.Net.Sockets;
using System.Threading.Tasks;

namespace Qorpent.Host.SimpleSockets{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleSocketClient<T,R>where T:new(){
		private SimpleSocketConfig _config;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public SimpleSocketClient(SimpleSocketConfig config = null){
			this._config = config ?? new SimpleSocketConfig();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public R Call(T data){
			var _result = CallAsync(data);
			_result.Wait();
			return _result.Result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public async Task<R> CallAsync(T data){
			var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(_config.GetEndPoint());
			await socket.SendDataAsync(data);
			//return default(R);
			return await socket.ReadAsync<R>();
		}
	}
}