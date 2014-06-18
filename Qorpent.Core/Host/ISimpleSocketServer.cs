using System;
using System.Threading;

namespace Qorpent.Host{
	/// <summary>
	/// 
	/// </summary>
	public interface ISimpleSocketServer:IDisposable{
		/// <summary>
		/// 
		/// </summary>
		CancellationTokenSource Cancel { get; }

		/// <summary>
		/// 
		/// </summary>
		void Start();
		/// <summary>
		/// 
		/// </summary>
		void Stop();



	}
}