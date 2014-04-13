using System;
using System.Runtime.Serialization;

namespace Qorpent.Charts {
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class ChartException : Exception {
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//
		/// <summary>
		/// 
		/// </summary>
		public ChartException() {
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="config"></param>
		public ChartException(string message, IChartConfig config) : base(message) {
			this.Config = config;
		}
		/// <summary>
		/// 
		/// </summary>
		public IChartConfig Config { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="config"></param>
		/// <param name="inner"></param>
		public ChartException(string message, IChartConfig config, Exception inner) : base(message, inner) {
			Config = config;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ChartException(SerializationInfo info, StreamingContext context) : base(info, context) {
		}
	}
}