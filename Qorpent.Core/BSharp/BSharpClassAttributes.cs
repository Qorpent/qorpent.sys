using System;

namespace Qorpent.BSharp {
	/// <summary>
	/// Атрибуты класса BSharp
	/// </summary>
	[Flags]
	public enum BSharpClassAttributes:ulong {
		/// <summary>
		/// Класс создан без указания импота по умолчанию с ключевым словом class
		/// </summary>
		Explicit  = 1,
		/// <summary>
		/// Явный признак класса-сироты
		/// </summary>
		Orphan = (ulong)1<<1,
		/// <summary>
		/// Признак абстрактного класса
		/// </summary>
		Abstract = (ulong)1<<2,
		/// <summary>
		/// Признак статического класса
		/// </summary>
		Static = (ulong)1<<3,
		/// <summary>
		/// Признак того что это класс-присадка для перекрытия реального класса
		/// </summary>
		Override = (ulong)1<<4,
		/// <summary>
		/// Признак того что это класс-присадка для дополнения реального класса
		/// </summary>
		Extension = (ulong)1<<5,
		/// <summary>
		/// Признак состояния в процесс билда
		/// </summary>
		InBuild = (ulong)1<<6,
		/// <summary>
		/// Флаг завершенного построения
		/// </summary>
		Built = (ulong)1<<7,
		/// <summary>
		/// Флаг ошибки в классе
		/// </summary>
		Error = (ulong)1<<8,
		/// <summary>
		/// Проигнорированный
		/// </summary>
		Ignored =(ulong)1<<9,
		/// <summary>
		/// В стадии ликовки
		/// </summary>
		InLink = (ulong)1<<10,
		/// <summary>
		/// Стадия линковки завершена
		/// </summary>
		Linked = (ulong)1<<11,
		/// <summary>
		/// Признак необходимости линковки
		/// </summary>
		RequireLinking = (ulong)1<<13,
		/// <summary>
		/// Признак необходимости разрешения классов
		/// </summary>
		RequireClassResolution = (ulong)1<<14,
		/// <summary>
		/// Потребность в резолюции словарей
		/// </summary>
		RequireDictionaryResolution = (ulong)1<<15,

		/// <summary>
		/// Потребность в резолюции дополнительных инклудов
		/// </summary>
		RequireAdvancedIncludes = (ulong)1 << 16,
		/// <summary>
		/// Необходимость регистрации словаря
		/// </summary>
		RequireDictionaryRegistration = (ulong)1<<17,

		/// <summary>
		/// Библиотечный, не изменяемый класс
		/// </summary>
		Library = (ulong)1 << 18,

		/// <summary>
		/// Признак класса - схемы
		/// </summary>
		Shema = (ulong)1<<19,
		/// <summary>
		/// Признак необходимости позднего биндинга при интерполяции
		/// </summary>
		RequireLateInterpolation = (ulong)1 << 20,
		/// <summary>
		/// Признак внедряемого класса
		/// </summary>
		Embed = (ulong)1<<21,
		/// <summary>
		/// Набор флагов для пакета исходников
		/// </summary>
		SrcPkgSet = Abstract | Static | Override | Extension 
			| RequireLinking | RequireClassResolution | RequireDictionaryResolution
			| RequireAdvancedIncludes | RequireDictionaryRegistration | Shema,
		/// <summary>
		/// Признак класса-набора данных
		/// </summary>
		Dataset = (ulong)1<<22,
		/// <summary>
		/// Признак класса - генератора
		/// </summary>
		Generator = (ulong)1<<23,
		/// <summary>
		/// Соединения
		/// </summary>
		Connection = (ulong)1<<24,
		/// <summary>
		/// Шаблоны
		/// </summary>
		Template = (ulong)1<<25,

		/// <summary>
		/// Класс без явного указания кода
		/// </summary>
		Anonymous = (ulong)1<<26,

		/// <summary>
		/// Класс Patch для применения на целевой
		/// </summary>
		Patch = (ulong)1<<27,

		/// <summary>
		/// Признак класса - генерика - ведет себя близко к статику (обрабатывается в ту же фазу), но интерполирует не по дефолтному ${...}, а по ^{...}
		/// </summary>
		Generic = (ulong)1 << 28,
		/// <summary>
		/// Marks cycled class
		/// </summary>
		Cycle = (ulong)1<<29,
		/// <summary>
		/// Класс, запрещающий неявныйрасчет элементов	
		/// </summary>
		ExplicitElements = (ulong)1<<30,
		/// <summary>
		/// Партицированный класс
		/// </summary>
		Partial = (ulong)1<<31,
		/// <summary>
		/// Классы директивного контроля над XML (препроцессор и постпроцессор)
		/// </summary>
		Direct = (ulong)1<<32,
		/// <summary>
		/// Набор мета-классов, не участвуют в компиляции, а являются расширением компилятора
		/// </summary>
		MetaClass = Patch | Generator | Dataset | Direct | Connection | Template,
	    RequireLateInterpolationExt = (ulong)1<<33
	}
}