using System;
using System.Collections.Concurrent;

namespace Qorpent.Utils{
	/// <summary>
	/// 
	/// </summary>
	public class ConsoleApplicationListener : IConsoleApplicationListener{
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetMessage(){
			string result;
			var dequed = _messages.TryDequeue(out result);
			if (!dequed) return null;
			return result;
		}

		readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public void Send(string message){
			_messages.Enqueue(message);
		}
		/// <summary>
		/// 
		/// </summary>
		public Action<string> OnOutput { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Action<string> OnError { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public virtual void EmitOutput(string data){
			if (null != OnOutput) OnOutput(data);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public virtual void EmitError(string data){
			if (null != OnError) OnError(data);
		}
	}
}