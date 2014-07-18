using System;

namespace Qorpent.Serialization.IntermediateFormat {
	/// <summary>
	/// Общая ошибка ZIF
	/// </summary>
	public class IntermediateFormatException : Exception {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="query"></param>
		/// <param name="ex"></param>
		public IntermediateFormatException(string message, IntermediateFormatQuery query, Exception ex = null)
			: base(message, ex) {
			this.Query = query;
		}
		/// <summary>
		/// Ссылка на запрос
		/// </summary>
		protected IntermediateFormatQuery Query { get; set; }
	}
}