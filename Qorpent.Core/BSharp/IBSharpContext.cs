﻿using System.Collections.Generic;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс результирующего контекста BSharp
	/// </summary>
	public interface IBSharpContext {
		/// <summary>
		/// Загружает исходные определения классов
		/// </summary>
		/// <param name="rawclasses"></param>
		void Setup(IEnumerable<IBSharpClass> rawclasses);

		/// <summary>
		///     Присоединяет и склеивается с другим результатом
		/// </summary>
		/// <param name="othercontext"></param>
		void Merge(IBSharpContext othercontext);

		/// <summary>
		/// Разрешает класс по коду и заявленному пространству имен
		/// </summary>
		/// <param name="code"></param>
		/// <param name="ns"></param>
		/// <returns></returns>
		IBSharpClass Get( string code, string ns = null);

		/// <summary>
		/// Возвращает коллекцию классов по типу классов
		/// </summary>
		/// <param name="datatype"></param>
		/// <returns></returns>
		IEnumerable<IBSharpClass> Get(BSharpContextDataType datatype);

		/// <summary>
		/// Возвращает ошибки указанного уровня
		/// </summary>
		/// <param name="level"></param>
		/// <returns></returns>
		IEnumerable<BSharpError> GetErrors(ErrorLevel level = ErrorLevel.None);

		/// <summary>
		/// Строит рабочий индекс классов
		/// </summary>
		void Build();
		/// <summary>
		/// Регистрирует ошибку в контексте
		/// </summary>
		/// <param name="error"></param>
		void RegisterError(BSharpError error);

		/// <summary>
		/// Реестр перезагрузок классов
		/// </summary>
		List<IBSharpClass> Ignored { get; set; }

		/// <summary>
		/// Компилятор
		/// </summary>
		IBSharpCompiler Compiler { get; set; }


	    /// <summary>
	    /// Разрешает словари
	    /// </summary>
	    void ResolveDictionaries();
	}
}