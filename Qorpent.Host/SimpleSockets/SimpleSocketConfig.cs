using System.Net;
using Qorpent.Config;

namespace Qorpent.Host.Exe.SimpleSockets{
	/// <summary>
	/// Конфигурация BinaryServer
	/// </summary>
	public class SimpleSocketConfig:ConfigBase{
		/// <summary>
		/// Возвращает адрес для прослушивания
		/// </summary>
		/// <returns></returns>
		public IPEndPoint GetEndPoint(){
			return new IPEndPoint(IPAddress.Loopback,10534);
		}
	}
}