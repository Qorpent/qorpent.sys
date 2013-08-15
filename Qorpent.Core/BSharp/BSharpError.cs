using System;
using System.Xml.Linq;

namespace Qorpent.BSharp {
	/// <summary>
	/// Структура, описывающая ошибки компиляции
	/// </summary>
	public class BSharpError {
		/// <summary>
		/// Уровень опасности ошибки
		/// </summary>
		public ErrorLevel Level { get; set; }
		/// <summary>
		/// Тип ошибки
		/// </summary>
		public BSharpErrorType Type { get; set; }

		/// <summary>
		/// Фаза, на которой произошла ошибка
		/// </summary>
		public BSharpCompilePhase Phase { get; set; }

		/// <summary>
		/// Может содержать пользовательское сообщение
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Может содержать Xml с какими-то дополнительными данными
		/// </summary>
		public XElement Xml { get; set; }
		/// <summary>
		/// Класс, которого касается ошибка
		/// </summary>
		public IBSharpClass Class { get; set; }
		/// <summary>
		/// Системное исключение
		/// </summary>
		public Exception Error { get; set; }
	}
}