using System;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Ошибки, связанные с разбором HTTP
	/// </summary>
	[Serializable]
	public class HttpReaderException : IOException{
		/// <summary>
		/// 
		/// </summary>
		public HttpReaderException(){
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public HttpReaderException(string message) : base(message){
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public HttpReaderException(string message, Exception inner) : base(message, inner){
		}
	}
}