using System;

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
			return new SimpleSocketServer<T, R>(handler, config ?? new SimpleSocketConfig());
		}
	
	}
}
