using System;
using System.Collections.Generic;

namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// Провайдер саппора Sql
	/// </summary>
	public interface ISqlProvider{
		/// <summary>
		/// Общий метод генерации для заданного типа объекта с некими хинтами
		/// </summary>
		/// <param name="dbObject"></param>
		/// <param name="mode"></param>
		/// <param name="hintObject"></param>
		/// <returns></returns>
		string GetSql(DbObject dbObject, DbGenerationMode mode, object hintObject);
		/// <summary>
		/// Метод генерации SQL для всей коллекции объектов
		/// </summary>
		/// <param name="objects"></param>
		/// <param name="mode"></param>
		/// <param name="hintObject"></param>
		/// <returns></returns>
		string GetSql(IEnumerable<DbObject> objects, DbGenerationMode mode, object hintObject);
	}
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum DbGenerationMode{
		/// <summary>
		/// Неопределенный режим
		/// </summary>
		None,
		/// <summary>
		/// Признак генерации в режиме скрипта
		/// </summary>
		Script,
		/// <summary>
		/// Признак генерации в рамках процедуры
		/// </summary>
		Procedure,
		/// <summary>
		/// Признак генерации в защищенном режиме
		/// </summary>
		Safe,
		/// <summary>
		/// Особый режим с постепенным накатом части обновлений
		/// </summary>
		Patch

	}
}