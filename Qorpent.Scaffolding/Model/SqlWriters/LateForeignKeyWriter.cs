using Qorpent.Data;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class LateForeignKeyWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="circularRef"></param>
		public LateForeignKeyWriter(Field circularRef){
			CircularRef = circularRef;
		}

		/// <summary>
		/// </summary>
		public Field CircularRef { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Mode == ScriptMode.Create){
				string result = "ALTER TABLE " + CircularRef.Table.FullSqlName + " ADD CONSTRAINT " +
				                CircularRef.GetConstraintName("FK") +
				                " FOREIGN KEY ("+CircularRef.Name.SqlQuoteName()+") REFERENCES " + CircularRef.ReferenceClass.FullSqlName + " (" +
				                CircularRef.ReferenceField.SqlQuoteName() +
				                ")";
				if (Dialect == SqlDialect.PostGres){
					result += " DEFERRABLE";
				}
				result += ";";
				return result;
			}
			else{
				return "ALTER TABLE " + CircularRef.Table.FullSqlName + " DROP CONSTRAINT " + CircularRef.GetConstraintName("FK") +
				       ";";
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "FK " + CircularRef.GetConstraintName("FK");
		}
	}
}