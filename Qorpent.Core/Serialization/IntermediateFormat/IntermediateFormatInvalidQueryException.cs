using System;
using Qorpent.Mvc;

namespace Qorpent.Serialization.IntermediateFormat {
	/// <summary>
	/// Исключение неверной конфигурации запроса
	/// </summary>
	public class IntermediateFormatInvalidQueryException : IntermediateFormatException {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="valid"></param>
		/// <param name="query"></param>
		/// <param name="ex"></param>
		public IntermediateFormatInvalidQueryException(ValidationResult valid, IntermediateFormatQuery query,Exception ex=null):base("Неверная конфигурация запроса ZIF: "+string.Join("; ",valid.Messages),query,ex) {
			
		}
	}
}