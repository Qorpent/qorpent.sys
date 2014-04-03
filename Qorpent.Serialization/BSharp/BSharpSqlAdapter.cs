using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	/// Основная реализация SQL-адаптера
	/// </summary>
	public class BSharpSqlAdapter:IBSharpSqlAdapter{
		/// <summary>
		/// Считывает набор данных и оборачивает в XML элемент с указанным именем, по умолчанию item - для dataset
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="query"></param>
		/// <param name="elementName"></param>
		/// <returns></returns>
		public IEnumerable<XElement> ExecuteReader(string connection, string query, string elementName = "item"){
			return InternalExecuteReader(connection, query, elementName).ToArray();
		}

		private static IEnumerable<XElement> InternalExecuteReader(string connection, string query, string elementName){
			using (var c = DatabaseExtensions.CreateDatabaseConnectionFromString(connection)){
				c.Open();
				var cmd = c.CreateCommand();
				cmd.CommandText = query;
				using (var r = cmd.ExecuteReader()){
					while (r.Read() || r.NextResult()){
						var e = new XElement(elementName);
						for (int i = 0; i < r.FieldCount; i++){
							var name = r.GetName(i);
							e.SetAttr(name, r[i] is DBNull ? "" : r[i]);
						}
						yield return e;
					}
				}
			}
		}
	}
}