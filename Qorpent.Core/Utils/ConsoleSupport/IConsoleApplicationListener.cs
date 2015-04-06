namespace Qorpent.Utils{
	/// <summary>
	/// 
	/// </summary>
	public interface IConsoleApplicationListener{
		/// <summary>
		/// ¬озвращает следующее сообщение дл€ потока
		/// </summary>
		/// <returns></returns>
		string GetMessage();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		void EmitOutput(string data);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		void EmitError(string data);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		void Send(string message);
	}
}