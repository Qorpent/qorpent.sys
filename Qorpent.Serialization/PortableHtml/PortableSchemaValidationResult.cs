using System;

namespace Qorpent.PortableHtml{
	/// <summary>
	/// Результат проверки схемы PHTML
	/// </summary>
	public class PortableSchemaValidationResult{
		/// <summary>
		/// Признак валидного результата
		/// </summary>
		public bool Ok{
			get { return PortableHtmlSchemaErorr.None == SchemaError; }
		}
		/// <summary>
		/// Код ошибки в схеме PHTML
		/// </summary>
		public PortableHtmlSchemaErorr SchemaError { get;  set; }
		/// <summary>
		/// Системное исключение
		/// </summary>
		public Exception Exception { get; set; }
	}
}