using System;
using System.Runtime.Serialization;

namespace Qorpent.Json {
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class JsonParserException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//
		/// <summary>
		/// 
		/// </summary>
		public JsonParserException() {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public JsonParserException(string message) : base(message) {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public JsonParserException(string message, Exception inner) : base(message, inner) {}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected JsonParserException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {}
	}
}