using Qorpent.Data;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	///     Форматирование схемы
	/// </summary>
	public class SchemaWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="schema"></param>
		public SchemaWriter(Schema schema){
			Schema = schema;
			Parameters = Schema;
		}

		/// <summary>
		/// </summary>
		public Schema Schema { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Mode == ScriptMode.Create){
				if (Dialect == SqlDialect.SqlServer){
					return "if (SCHEMA_ID('${Name}') is null) exec sp_executesql N'CREATE SCHEMA ${Name}';";
				}
				if (Dialect == SqlDialect.PostGres){
					return "CREATE SCHEMA IF NOT EXISTS ${Name};";
				}

				return "CREATE SCHEMA ${Name};";
			}
			else{
				return "DROP SCHEMA ${Name};";
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "Schema " + Schema.Name;
		}
	}
}