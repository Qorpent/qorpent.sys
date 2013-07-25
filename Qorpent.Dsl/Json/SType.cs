using System;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Тип контекста
	/// </summary>
	[Flags]
	public enum SType {
		/// <summary>
		/// Начальное состояние
		/// </summary>
		Init = 0,
		/// <summary>
		/// В литерале
		/// </summary>
		InLiteral =1,
		/// <summary>
		/// В цифре
		/// </summary>
		InDigit= 2,
		/// <summary>
		///В строке
		/// </summary>
		InString =4,
		/// <summary>
		/// В значении
		/// </summary>
		InValue = InLiteral |InDigit |InString, 
		/// <summary>
		/// В теле объекта
		/// </summary>
		OpenObject = 8,
		/// <summary>
		/// В теле массива
		/// </summary>
		OpenArray =16,
		/// <summary>
		/// Внутри блока
		/// </summary>
		InBlock = OpenObject|OpenArray,
		/// <summary>
		/// После имени свойства
		/// </summary>
		AfterName = 32,
		/// <summary>
		/// После двоеточия
		/// </summary>
		AfterCol = 64,
		/// <summary>
		/// После значения свойства
		/// </summary>
		AfterPropValue = 128,
		/// <summary>
		/// После запятой у свойства
		/// </summary>
		AfterPropCom = 256,
		/// <summary>
		/// После свойства
		/// </summary>
		AfterProp = 512,
		/// <summary>
		/// После значения
		/// </summary>
		AfterArrayValue =1024,
		/// <summary>
		/// После запятой в значении
		/// </summary>
		AfterArrayCom = 2048,
		/// <summary>
		/// Вообще после запятой
		/// </summary>
		AfterCom = AfterPropCom |AfterArrayCom,
		/// <summary>
		/// Вообще после элемента
		/// </summary>
		AfterItem = AfterProp | AfterArrayValue

	}
}