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
		/// Группа файлов
		/// </summary>
		FileGroup =1,
		/// <summary>
		/// Таблица
		/// </summary>
		Table = 1<<1,
		/// <summary>
		/// Вид
		/// </summary>
		View = 1<<2,
		/// <summary>
		/// Индекс
		/// </summary>
		Index = 1<<3,
		/// <summary>
		/// Функция
		/// </summary>
		Function = 1<<4,
		/// <summary>
		/// Процедура
		/// </summary>
		Procedure = 1<<5,
		/// <summary>
		/// 
		/// </summary>
		Trigger = 1<<6,
		/// <summary>
		/// Псевдоним
		/// </summary>
		Alias = 1<<7,
		/// <summary>
		/// Сборка
		/// </summary>
		ClrAssembly = 1<<8,
		/// <summary>
		/// CLR Тип
		/// </summary>
		ClrType = 1<<9,
		/// <summary>
		/// CLR функция
		/// </summary>
		ClrFunction = 1<<10,
		/// <summary>
		/// CLR процедура
		/// </summary>
		ClrProcedure = 1<<11,
		/// <summary>
		/// CLR триггер
		/// </summary>
		ClrTrigger = 1<<12,

		/// <summary>
		/// Поле таблицы
		/// </summary>
		Field = 1 << 13
	}
}