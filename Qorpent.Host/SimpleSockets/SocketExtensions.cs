using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO;

namespace Qorpent.Host.SimpleSockets{
	/// <summary>
	/// 
	/// </summary>
	internal static class SocketExtensions
	{
		public static SocketAwaitable ReceiveAsync(this Socket socket,
		                                           SocketAwaitable awaitable)
		{
			awaitable.Reset();
			if (!socket.ReceiveAsync(awaitable.m_eventArgs))
				awaitable.m_wasCompleted = true;
			return awaitable;
		}

		public static SocketAwaitable AcceptAsync(this Socket socket,
												   SocketAwaitable awaitable)
		{
			awaitable.Reset();
			if (!socket.AcceptAsync(awaitable.m_eventArgs))
				awaitable.m_wasCompleted = true;
			return awaitable;
		}

		public static SocketAwaitable SendAsync(this Socket socket,
		                                        SocketAwaitable awaitable)
		{
			awaitable.Reset();
			if (!socket.SendAsync(awaitable.m_eventArgs))
				awaitable.m_wasCompleted = true;
			return awaitable;
		}

		public static async Task<Socket> AcceptSimpleSocketAsync(this Socket s){
			var args = new SocketAsyncEventArgs();
			var awaitable = new SocketAwaitable(args);
			await s.AcceptAsync(awaitable);
			return args.AcceptSocket;
		}

		public static int DEFAULT_BUFFER_SIZE = 0x100;
		public static async Task<T> ReadAsync<T>(this Socket s){
			// Reusable SocketAsyncEventArgs and awaitable wrapper 
			var args = new SocketAsyncEventArgs();
			args.SetBuffer(new byte[DEFAULT_BUFFER_SIZE], 0, DEFAULT_BUFFER_SIZE);
			
			var awaitable = new SocketAwaitable(args);
			var buffer = new byte[DEFAULT_BUFFER_SIZE];
			var current = 0;
			var size = 0;
			// Do processing, continually receiving from the socket 
			byte[] finisher = null;
			while (true)
			{
				await s.ReceiveAsync(awaitable);
				int bytesRead = args.BytesTransferred;
				if (bytesRead <= 0) break;
				size += bytesRead;
				if (size > buffer.Length)
				{
					Array.Resize(ref buffer, buffer.Length + DEFAULT_BUFFER_SIZE);
				}
				args.Buffer.CopyTo(buffer,current);
				current = size;
				if (size >= 4 && null==finisher){
					finisher = new[]{buffer[0], buffer[1], buffer[2], buffer[3]};
				}
				if (size >= 8){
					bool br = true;
					for (var i = 0; i < 4; i++){
						if (finisher[i] != buffer[size - (4 - i)]){
							br = false;
							break;
						}
					}
					if (br) break;
				}
			}
			var stream = new MemoryStream(buffer,4,size-8);
			if (typeof (IBinarySerializable).IsAssignableFrom(typeof (T))){
				var result = Activator.CreateInstance<T>();
				var br = new BinaryReader(stream,Encoding.UTF8);
				((IBinarySerializable) result).Read(br);
				return result;
			}
			else{
				var bf = new BinaryFormatter();
				var result = bf.Deserialize(stream);
				return (T) result;
			}
		}

		public static byte[] QUOTE_SEQUENCE = new[]{(byte) 'Q', (byte) 'H', (byte) 'S', (byte) 'S'};
		public static async Task SendDataAsync(this Socket s, object data){
			var stream = new MemoryStream();
			stream.Write(QUOTE_SEQUENCE, 0, 4);
			if (data is IBinarySerializable){
				using (var w = new BinaryWriter(stream, Encoding.UTF8,true)){
					((IBinarySerializable) data).Write(w);
				}
			}
			else
			{
				
				var bf = new BinaryFormatter();
				bf.Serialize(stream,data);
				stream.Flush();
				
			}
			stream.Write(QUOTE_SEQUENCE, 0, 4);
			stream.Flush();
			// Reusable SocketAsyncEventArgs and awaitable wrapper 
			var args = new SocketAsyncEventArgs();
			args.SetBuffer(stream.ToArray(), 0, (int)stream.Length);
			var awaitable = new SocketAwaitable(args);
			await s.SendAsync(awaitable);
		}
	}
}