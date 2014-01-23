using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Атрибуты класса BSharp
	/// </summary>
	[Flags]
	public enum BSharpClassAttributes {
		/// <summary>
		/// Класс создан без указания импота по умолчанию с ключевым словом class
		/// </summary>
		Explicit  = 1,
		/// <summary>
		/// Явный признак класса-сироты
		/// </summary>
		Orphan = 1<<1,
		/// <summary>
		/// Признак абстрактного класса
		/// </summary>
		Abstract = 1<<2,
		/// <summary>
		/// Признак статического класса
		/// </summary>
		Static = 1<<3,
		/// <summary>
		/// Признак того что это класс-присадка для перекрытия реального класса
		/// </summary>
		Override = 1<<4,
		/// <summary>
		/// Признак того что это класс-присадка для дополнения реального класса
		/// </summary>
		Extension = 1<<5,
		/// <summary>
		/// Признак состояния в процесс билда
		/// </summary>
		InBuild = 1<<6,
		/// <summary>
		/// Флаг завершенного построения
		/// </summary>
		Built = 1<<7,
		/// <summary>
		/// Флаг ошибки в классе
		/// </summary>
		Error = 1<<8,
		/// <summary>
		/// Проигнорированный
		/// </summary>
		Ignored =1<<9,
		/// <summary>
		/// В стадии ликовки
		/// </summary>
		InLink = 1<<10,
		/// <summary>
		/// Стадия линковки завершена
		/// </summary>
		Linked = 1<<11,
		/// <summary>
		/// Признак необходимости линковки
		/// </summary>
		RequireLinking = 1<<13,
		/// <summary>
		/// Признак необходимости разрешения классов
		/// </summary>
		RequireClassResolution = 1<<14,
		/// <summary>
		/// Потребность в резолюции словарей
		/// </summary>
		RequireDictionaryResolution = 1<<15,

		/// <summary>
		/// Потребность в резолюции дополнительных инклудов
		/// </summary>
		RequireAdvancedIncludes = 1 << 16,
		/// <summary>
		/// Необходимость регистрации словаря
		/// </summary>
		RequireDictionaryRegistration = 1<<17,

		/// <summary>
		/// Библиотечный, не изменяемый класс
		/// </summary>
		Library  =1 << 18,

		/// <summary>
		/// Признак класса - схемы
		/// </summary>
		Shema = 1<<19,
		/// <summary>
		/// Признак необходимости позднего биндинга при интерполяции
		/// </summary>
		RequireLateInterpolation = 1 << 20,
		/// <summary>
		/// Признак внедряемого класса
		/// </summary>
		Embed = 1<<21,
		/// <summary>
		/// Набор флагов для пакета исходников
		/// </summary>
		SrcPkgSet = Abstract | Static | Override | Extension 
			| RequireLinking | RequireClassResolution | RequireDictionaryResolution
			| RequireAdvancedIncludes | RequireDictionaryRegistration | Shema,
		/// <summary>
		/// Признак класса-набора данных
		/// </summary>
		Dataset = 1<<22,
		/// <summary>
		/// Признак класса - генератора
		/// </summary>
		Generator = 1<<23,
		
	}
}