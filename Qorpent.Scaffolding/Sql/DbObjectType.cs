	namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Тип объекта DDL
	/// </summary>
	public enum DbObjectType:long{
		/// <summary>
		/// Неопределенный
		/// </summary>
		None = 0,
		/// <summary>
		/// Группа файлов
		/// </summary>
		FileGroup =1,
		/// <summary>
		/// Схема партицирования
		/// </summary>
		PartitionScheme = 1 << 1,
		/// <summary>
		/// Таблица
		/// </summary>
		Table = 1<<2,
		/// <summary>
		/// Схема автоматического партицирования
		/// </summary>
		AutoPartition = 1 << 3,
		/// <summary>
		/// Вид
		/// </summary>
		View = 1<<4,
		/// <summary>
		/// Индекс
		/// </summary>
		Index = 1<<5,
		/// <summary>
		/// Функция
		/// </summary>
		Function = 1<<6,
		/// <summary>
		/// Процедура
		/// </summary>
		Procedure = 1<<7,
		/// <summary>
		/// 
		/// </summary>
		Trigger = 1<<8,
		/// <summary>
		/// Псевдоним
		/// </summary>
		Alias = 1<<9,
		/// <summary>
		/// Сборка
		/// </summary>
		ClrAssembly = 1<<10,
		/// <summary>
		/// CLR Тип
		/// </summary>
		ClrType = 1<<11,
		/// <summary>
		/// CLR функция
		/// </summary>
		ClrFunction = 1<<12,
		/// <summary>
		/// CLR процедура
		/// </summary>
		ClrProcedure = 1<<13,
		/// <summary>
		/// CLR триггер
		/// </summary>
		ClrTrigger = 1<<14,

		/// <summary>
		/// Поле таблицы
		/// </summary>
		Field = 1 << 15,
		
		
	}
}