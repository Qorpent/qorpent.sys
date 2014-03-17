using System.Collections.Generic;

namespace Qorpent.Scaffolding.Sql{
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
}