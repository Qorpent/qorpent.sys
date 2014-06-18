using System;
using Qorpent.Host.Exe.Security;

namespace Qorpent.Host.Exe.SimpleSockets
{
	/// <summary>
	/// 
	/// </summary>
	public class SimpleSocket{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static ISimpleSocketServer CreateServer<T, R>(ISimpleSocketHandler<T, R> handler, SimpleSocketConfig config = null)
		{
			if (null == handler)
			{
				throw new ArgumentNullException("handler");
			}
			var port = SimpleSocketConfig.DEFAULT_PORT;
			if (typeof (T) == typeof (AuthProtocol)){
				port = AuthProtocol.DefaultPort;
			}
			return new SimpleSocketServer<T, R>(handler, config ?? new SimpleSocketConfig{Port = port});
		}
	
	}
}
