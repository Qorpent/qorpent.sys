using System;

namespace Qorpent.BSharp.Builder {
	/// <summary>
	/// Флаги расположения выходного потока
	/// </summary>
	[Flags]
	public enum BSharpBuilderOutputAttributes {
		//ФЛАГИ ОПРЕДЕЛЕНИЯ ТОГО, ЧТО ВООБЩЕ ВКЛЮЧАЕТСЯ В НАБОР

		/// <summary>
		/// Признак включения рабочих откомпилированных классов
		/// </summary>
		IncludeWork = 1,
		/// <summary>
		/// В папку с системной информацией должен сохраниться большой XML со всем контекстом
		/// </summary>
		IncludeFullContext = 1<<1,
		/// <summary>
		/// В выдаче присуствуют классы - сироты
		/// </summary>
		IncludeOrphans = 1<<2,

		/// <summary>
		/// В специальную папку должны выдаваться "объектные файлы" - вытяжки из полного контекста по отдельным классам
		/// </summary>
		IncludeObjectFiles =1<<3,


		//ФЛАГИ РАСПРЕДЕЛЕНИЯ РАБОЧИХ ФАЙЛОВ ПО ПАПКАМ

		/// <summary>
		/// Все классы собираются в один большой файл
		/// </summary>
		SingleFile = 1<<4,
		/// <summary>
		/// Используются полные имена классов
		/// </summary>
		UseFullName =1<<5,
		/// <summary>
		/// Пространства имен образуют плоский список папок:
		/// /
		/// /sys
		/// /sys.myns
		/// /eco
		/// /eco.colset
		/// </summary>
		PlainNamespace = 1 << 6,
		/// <summary>
		/// Пространства имен образуют иерархический список папок:
		/// /
		/// /sys
		/// /sys/myns
		/// /eco
		/// /eco/colset
		/// </summary>
		TreeNamespace = 1 << 7,

		/// <summary>
		/// Классы распределяются по прототипам, а не по пространствам имен
		/// </summary>
		PrototypeAlign = 1 << 8,

		/// <summary>
		/// По умолчанию просто выводится директория с плоскими нэймспэйсами
		/// </summary>
        Default = IncludeWork | PlainNamespace,
	}
}