using System;
using System.Runtime.Serialization;

namespace Qorpent.BSharp.Runtime {
	/// <summary>
	/// </summary>
	[Serializable]
	public class BSharpRuntimeException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		/// <summary>
		/// </summary>
		public BSharpRuntimeException() {}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		public BSharpRuntimeException(string message) : base(message) {}

		/// <summary>
		/// </summary>
		/// <param name="message"></param>
		/// <param name="inner"></param>
		public BSharpRuntimeException(string message, Exception inner) : base(message, inner) {}

		/// <summary>
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected BSharpRuntimeException(
			SerializationInfo info,
			StreamingContext context) : base(info, context) {}
	}
}