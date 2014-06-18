using System.Net;
using Qorpent.Config;

namespace Qorpent.Host.Exe.SimpleSockets{
	/// <summary>
	/// Конфигурация BinaryServer
	/// </summary>
	public class SimpleSocketConfig:ConfigBase{
		/// <summary>
		/// 
		/// </summary>
		public const int DEFAULT_PORT = 10534;
		/// <summary>
		/// 
		/// </summary>
		public SimpleSocketConfig(){
			Port = DEFAULT_PORT;
		}
		/// <summary>
		/// 
		/// </summary>
		public int Port{
			get { return Get<int>("port"); }
			set { Set("port",value);}
		}


		/// <summary>
		/// Возвращает адрес для прослушивания
		/// </summary>
		/// <returns></returns>
		public IPEndPoint GetEndPoint(){
			return new IPEndPoint(IPAddress.Loopback,Port);

		}
	}
}