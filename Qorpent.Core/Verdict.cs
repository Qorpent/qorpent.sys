using System;
using System.Collections.Generic;

namespace Qorpent
{
	/// <summary>
	/// Структура, описывающая обоснованное решение по вопросу о разрешении того или иного действия
	/// </summary>
	public class Verdict
	{
		private IList<string> _messages;

		/// <summary>
		/// Итоговое решение
		/// </summary>
		public VerdictType Type { get; set; }
		/// <summary>
		/// Связанная ошибка
		/// </summary>
		public Exception Error { get; set; }

		/// <summary>
		/// Оптимизированный метод проверки наличия/отсутствия сообщений
		/// </summary>
		/// <returns></returns>
		public bool HasMessages() {
			return null != _messages && 0 != _messages.Count;
		}
		/// <summary>
		/// Сообщения
		/// </summary>
		public IList<string> Messages {
			get { return _messages ??(_messages = new List<string>()) ; }
		}
	}
}
