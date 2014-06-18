using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent.Host.SimpleSockets{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class SocketAwaitable : INotifyCompletion
	{
		private readonly static Action SENTINEL = () => { };

		internal bool m_wasCompleted;
		internal Action m_continuation;
		internal SocketAsyncEventArgs m_eventArgs;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventArgs"></param>
		public SocketAwaitable(SocketAsyncEventArgs eventArgs)
		{
			if (eventArgs == null) throw new ArgumentNullException("eventArgs");
			m_eventArgs = eventArgs;
			eventArgs.Completed += delegate
			{
				var prev = m_continuation ?? Interlocked.CompareExchange(
					ref m_continuation, SENTINEL, null);
				if (prev != null) prev();
			};
		}
		/// <summary>
		/// 
		/// </summary>
		internal void Reset()
		{
			m_wasCompleted = false;
			m_continuation = null;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public SocketAwaitable GetAwaiter() { return this; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsCompleted { get { return m_wasCompleted; } }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="continuation"></param>
		public void OnCompleted(Action continuation)
		{
			if (m_continuation == SENTINEL ||
			    Interlocked.CompareExchange(
				    ref m_continuation, continuation, null) == SENTINEL)
			{
				Task.Run(continuation);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public void GetResult()
		{
			if (m_eventArgs.SocketError != SocketError.Success)
				throw new SocketException((int)m_eventArgs.SocketError);
		}
	}
}