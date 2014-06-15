using Qorpent.Scaffolding.Model.SqlObjects;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class PartitionDefinitionWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="definition"></param>
		public PartitionDefinitionWriter(PartitionDefinition definition){
			Definition = definition;
			Parameters = definition;
		}

		/// <summary>
		/// </summary>
		public PartitionDefinition Definition { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Dialect == SqlDialect.SqlServer){
				if (Mode == ScriptMode.Create){
					return @"
CREATE PARTITION FUNCTION ${Name}Func (${Type})
AS RANGE LEFT FOR VALUES ('${Start}')
CREATE PARTITION SCHEME ${Name} AS PARTITION ${Name}Func ALL TO (${FileGroup})
";
				}
				else{
					return @"
DROP PARTITION SCHEME ${Name} 
DROP PARTITION FUNCTION ${Name}Func
";
				}
			}
			return "";
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			if (Dialect == SqlDialect.SqlServer){
				return "PARTDEF " + Definition.Name;
			}
			return "";
		}
	}
}