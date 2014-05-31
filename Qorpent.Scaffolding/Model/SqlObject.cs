using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SqlObject{
		/// <summary>
		/// Тип объекта
		/// </summary>
		public SqlObjectType ObjectType { get; set; }
		/// <summary>
		/// Ссылка на класс-контейнер
		/// </summary>
		public PersistentClass MyClass { get; set; }
		/// <summary>
		/// Признак объекта, который должен формироваться до определения таблицы
		/// </summary>
		public bool PreTable { get; set; }

		/// <summary>
		/// Формирует глобальные объекты уровня базы данных
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDatabaseWide(PersistentModel model)
		{
			yield break;
		}
		/// <summary>
		/// Формирует стандартные объекты для таблицы
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDefaults(PersistentClass cls){
			yield break;
		}
		/// <summary>
		/// Формирует специальные объекты, определенные в таблице, конкретный элемент
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> Create(PersistentClass cls, XElement e){
			yield break;
		}
	}
}