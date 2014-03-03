namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// Тип объекта DDL
	/// </summary>
	public enum DbObjectType{
		/// <summary>
		/// Неопределенный
		/// </summary>
		None = 0,
		/// <summary>
		/// Таблица
		/// </summary>
		Table = 1,
		/// <summary>
		/// Вид
		/// </summary>
		View = 1<<1,
		/// <summary>
		/// Индекс
		/// </summary>
		Index = 1<<2,
		/// <summary>
		/// Функция
		/// </summary>
		Function = 1<<3,
		/// <summary>
		/// Процедура
		/// </summary>
		Procedure = 1<<4,
		/// <summary>
		/// 
		/// </summary>
		Trigger = 1<<5,
		/// <summary>
		/// Псевдоним
		/// </summary>
		Alias = 1<<6,
		/// <summary>
		/// Сборка
		/// </summary>
		ClrAssembly = 1<<7,
		/// <summary>
		/// CLR Тип
		/// </summary>
		ClrType = 1<<8,
		/// <summary>
		/// CLR функция
		/// </summary>
		ClrFunction = 1<<9,
		/// <summary>
		/// CLR процедура
		/// </summary>
		ClrProcedure = 1<<10,
		/// <summary>
		/// CLR триггер
		/// </summary>
		ClrTrigger = 1<<11,

		/// <summary>
		/// Поле таблицы
		/// </summary>
		Field = 1 << 12,
		
	}
}