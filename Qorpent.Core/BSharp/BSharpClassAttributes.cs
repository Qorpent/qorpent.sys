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
		Linked = 1<<11
	}
}