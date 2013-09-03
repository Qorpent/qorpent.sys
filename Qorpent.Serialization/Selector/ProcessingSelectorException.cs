using System;
using Qorpent.Serialization;

namespace Qorpent.Selector {
	/// <summary>
	///     Базовый класс исключения минервы
	/// </summary>
	public class ProcessingSelectorException : Qorpent.QorpentException {
		/// <summary>
		///     Язык запроса
		/// </summary>
		public readonly SelectorLanguage Language;

		/// <summary>
		///     Запрос
		/// </summary>
		public readonly string Query;

		/// <summary>
		///     Конструктор по умолчанию
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerexception"></param>
		public ProcessingSelectorException(string message = "", Exception innerexception = null)
			: base(message, innerexception) {}

		/// <summary>
		///     Специальный конструктор
		/// </summary>
		/// <param name="comment"></param>
		/// <param name="code"></param>
		/// <param name="lang"></param>
		/// <param name="query"></param>
		/// <param name="innerexception"></param>
		public ProcessingSelectorException(string comment, int code, SelectorLanguage lang, string query,
		                                          Exception innerexception = null) : base(
			                                          string.Format("Ошибка селектора {0} ({1}) при поиске '{2}' ({3})",
			                                                        "PS" + code,
			                                                        comment, query, lang)
			                                          , innerexception) {
			Code = "PS" + code;
			Language = lang;
			Query = query;
		}
		/// <summary>
		/// Код ошибки
		/// </summary>
		public string Code { get; set; }
	}
}