using Qorpent.Data;
using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class SequenceWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="sequence"></param>
		public SequenceWriter(Sequence sequence){
			Sequence = sequence;
			Parameters = sequence;
		}

		/// <summary>
		/// </summary>
		public Sequence Sequence { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Mode == ScriptMode.Create){
				if (Dialect == DbDialect.SqlServer){
					return "CREATE SEQUENCE ${FullName} AS " + Sequence.DataType.ResolveSqlDataType(Dialect) +
					       " START WITH ${Start} INCREMENT BY ${Step}${Cycle};";
				}
				if (Dialect == DbDialect.PostGres){
					return "CREATE SEQUENCE ${FullName} INCREMENT BY ${Step} START WITH ${Start};";
				}
			}
			else{
				if (Dialect == DbDialect.SqlServer || Dialect == DbDialect.PostGres){
					return "DROP SEQUENCE ${FullName};";
				}
			}
			return "";
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			if (Dialect == DbDialect.SqlServer || Dialect == DbDialect.PostGres){
				return "Sequence " + Sequence.FullName;
			}
			return "";
		}
	}
}