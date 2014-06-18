using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Qorpent.Host.Exe.SimpleSockets{
	/// <summary>
	/// Tcp listener wrapper with call/result binary protocol
	/// </summary>
	internal class SimpleSocketServer<T,R> : ISimpleSocketServer,IDisposable{
		private ISimpleSocketHandler<T,R> _handler;
		private SimpleSocketConfig _config;
		
		/// <summary>
		/// 
		/// </summary>
		internal SimpleSocketServer(ISimpleSocketHandler<T,R> handler, SimpleSocketConfig config =null){
			_handler = handler;
			_config = config ;
			_cancel = new CancellationTokenSource();
		}

		void _accept_Completed(object sender, SocketAsyncEventArgs e){
			var socket = e.AcceptSocket;
			if (Cancel.IsCancellationRequested)
			{
				socket.Close();
				ShutDown();
				return;
			}
			var request = new SimpleSocketRequest<T, R>(socket);
			Register(request);
			Accept();
		}

		private void Register(SimpleSocketRequest<T, R> request){
			_agenda.Add(_handler.Execute(request));
			if (_agenda.Count > 100){
				foreach (var task in _agenda.ToArray()){
					if (task.IsCompleted) _agenda.Remove(task);
				}
			}
		}

		private CancellationTokenSource _cancel;
		private Socket _listener;
		private bool _started;
		/// <summary>
		/// 
		/// </summary>
		public CancellationTokenSource Cancel
		{
			get { return _cancel; }
		}

		/// <summary>
		/// 
		/// </summary>
		public void Start(){
			if (_started) return;
			lock (this){
				_cancel = new CancellationTokenSource();
				SetupSocket();
				_started = true;
				Accept();
			}
		}

		public void Stop(){
			Cancel.Cancel();
			ShutDown();
			
		}

		IList<Task> _agenda = new List<Task>();
		private void Accept(){
			if (Cancel.IsCancellationRequested)
			{
				ShutDown();
				return;
			}
			var args = new SocketAsyncEventArgs();
			args.Completed += _accept_Completed;
			_listener.AcceptAsync(args);	
		}

		private void SetupSocket(){
			_listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
			_listener.Bind(_config.GetEndPoint());
			_listener.Listen((int)SocketOptionName.MaxConnections);

		}

		private void ShutDown(){
			Task.WaitAll(_agenda.ToArray());
			_agenda.Clear();
			if (null != _listener){
					_listener.Close();
					_listener = null;
				}
			_started = false;
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose(){
			Stop();
		}
	}
}