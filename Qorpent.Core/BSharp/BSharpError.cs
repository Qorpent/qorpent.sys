using System;
using System.Xml.Linq;
using Qorpent.Serialization;

namespace Qorpent.BSharp {
	/// <summary>
	/// Структура, описывающая ошибки компиляции
	/// </summary>
	[Serialize]
	public class BSharpError {
		/// <summary>
		/// Уровень опасности ошибки
		/// </summary>
		[SerializeNotNullOnly]
		public ErrorLevel Level { get; set; }
		/// <summary>
		/// Тип ошибки
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpErrorType Type { get; set; }

		/// <summary>
		/// Фаза, на которой произошла ошибка
		/// </summary>
		[SerializeNotNullOnly]
		public BSharpCompilePhase Phase { get; set; }

		/// <summary>
		/// Может содержать пользовательское сообщение
		/// </summary>
		[SerializeNotNullOnly]
		public string Message { get; set; }

		/// <summary>
		/// Может содержать Xml с какими-то дополнительными данными
		/// </summary>
		[SerializeNotNullOnly]
		public XElement Xml { get; set; }
		/// <summary>
		/// Класс, которого касается ошибка
		/// </summary>
		[SerializeNotNullOnly]
		public IBSharpClass Class { get; set; }
		/// <summary>
		/// Системное исключение
		/// </summary>
		[SerializeNotNullOnly]
		public Exception Error { get; set; }
		/// <summary>
		///Дополнительные данные
		/// </summary>
		[SerializeNotNullOnly]
		public object Data { get; set; }
		/// <summary>
		/// Имя класса
		/// </summary>
		[SerializeNotNullOnly]
		public string ClassName { get; set; }
		/// <summary>
		/// Дополнительный класс для некоторых сообщений
		/// </summary>
		[SerializeNotNullOnly]
		public IBSharpClass AltClass { get; set; }
		/// <summary>
		/// Конвертирует строку для лога
		/// </summary>
		/// <returns></returns>
		public string ToLogString() {
			return String.Format(@"{0}:{1} {2} ({4},{5})", Type, Phase, Message, Xml, 
				null==Class?ClassName:Class.FullName,null==AltClass?"":AltClass.FullName);
		}
	}
}