using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Типы коллекций контекста результата 
	/// </summary>
	[Flags]
	public enum BSharpContextDataType {
		/// <summary>
		/// Рабочие классы для экспорта
		/// </summary>
		Working = 1,
		/// <summary>
		/// Неразрезольвленные классы
		/// </summary>
		Orphans = 1<<1,
		/// <summary>
		/// Перекрытия классов
		/// </summary>
		Overrides = 1<<2,
		/// <summary>
		/// Расшиения классов
		/// </summary>
		Extensions = 1<<3,
		/// <summary>
		/// Абстрактные классы
		/// </summary>
		Abstracts = 1<<4,
		/// <summary>
		/// Статические классы
		/// </summary>
		Statics = 1<<5,
		/// <summary>
		/// Классы с ошибками
		/// </summary>
		Errors = 1<<6,
		/// <summary>
		/// Проигнорированные классы
		/// </summary>
		Ignored=1<<7,
		/// <summary>
		/// По умолчанию - рабочие классы
		/// </summary>
		Default = Working,
		/// <summary>
		/// Обобщенный тип индексируемого в качестве исходного текста класса
		/// </summary>
		SrcPkg = 1<<8,
	}
}