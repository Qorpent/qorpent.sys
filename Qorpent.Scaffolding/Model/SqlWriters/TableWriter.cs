using System;
using System.Linq;
using System.Text;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Sql;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// 
	/// </summary>
	public class TableWriter : SqlCommandWriter{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cls"></param>
		public TableWriter(PersistentClass cls){
			this.Table = cls;
		}
		/// <summary>
		/// 
		/// </summary>
		public PersistentClass Table { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Mode == ScriptMode.Create){
				var sb = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(Table.Comment) && !NoComment){
					sb.AppendLine("-- " + Table.Comment);
				}
				sb.AppendLine("CREATE TABLE " + Table.FullSqlName + " (");
				var fields = Table.Fields.Values.OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
				for (var f = 0; f < fields.Length; f++){
					WriteField(fields[f], sb, f == fields.Length - 1);
				}
				sb.Append(")");
				WriteAllocation(sb);
				sb.Append(";");
				sb.AppendLine();
				return sb.ToString();
			}
			else{
				return "DROP TABLE " + Table.FullSqlName + ";";
			}
		}

		private void WriteAllocation(StringBuilder sb){
			if (Dialect == SqlDialect.SqlServer){
				var name = Table.AllocationInfo.FileGroup.Name;
				if (Table.AllocationInfo.Partitioned && Model.IsSupportPartitioning(SqlDialect.SqlServer)){
					name = Table.FullSqlName.Replace(".", "_") + "_PARTITION ( " + Table.AllocationInfo.PartitionField.Name + ")";
				}
				sb.Append(" ON " + name);
			}
			else if(Dialect==SqlDialect.PostGres){
				sb.Append(" TABLESPACE " + Table.AllocationInfo.FileGroup.Name);
			}
		}

		private void WriteField(Field field, StringBuilder sb,bool last){
			if (!string.IsNullOrWhiteSpace(field.Comment) && !NoComment){
				sb.AppendLine("\t-- " + field.Comment);
			}
			sb.AppendFormat("\t{0} {1} NOT NULL", field.Name,field.DataType.ResolveSqlDataType(Dialect) );
			WritePrimaryKey(field, sb);
			WriteUnique(field, sb);
			WriteForeignKey(field, sb);
			WriteDefaultValue(field, sb);
			if (!last){
				sb.AppendLine(",");
			}
			else{
				sb.AppendLine();
			}
		}

		private void WriteForeignKey(Field field, StringBuilder sb){
			if (field.IsPrimaryKey) return;
			if (!field.IsReference) return;
			if (field.GetIsCircular()) return;
			sb.Append(" CONSTRAINT " + field.GetConstraintName("FK") + " FOREIGN KEY REFERENCES "+field.ReferenceClass.FullSqlName+" ("+field.ReferenceField+")");
		}

		private void WriteUnique(Field field, StringBuilder sb){
			if(field.IsPrimaryKey)return;
			if (!field.IsUnique) return;
			sb.Append(" CONSTRAINT " + field.GetConstraintName("UNQ")+" UNIQUE");
		}

		private void WritePrimaryKey(Field field, StringBuilder sb){
			if (!field.IsPrimaryKey) return;
			if (field.Table.AllocationInfo.Partitioned && Model.IsSupportPartitioning(Dialect)){
				sb.Append(" CONSTRAINT " + field.GetConstraintName("PK") + " PRIMARY KEY NONCLUSTERED ON "+field.Table.AllocationInfo.FileGroup.Name);
			}
			else{
				sb.Append(" CONSTRAINT " + field.GetConstraintName("PK") + " PRIMARY KEY");
			}
		}

		private void WriteDefaultValue(Field field, StringBuilder sb){
			sb.Append(" DEFAULT ");
			if (field.IsAutoIncrement){
				WriteAutoIncrementDefault(field, sb);
			}
			else{
				WriteUsualDefault(field,sb);
			}
		}

		private void WriteUsualDefault(Field field, StringBuilder sb){
			var def = field.DefaultSqlValue;
			switch (def.DefaultValueType){
				case DbDefaultValueType.String:
					sb.Append("'" + def.Value.ToSqlString() + "'");
					return;
				case DbDefaultValueType.Native:
					if (def.Value == null || "".Equals(def.Value)){
						sb.Append("''");
					}
					else{
						sb.Append(def.Value);
					}
					return;
				case DbDefaultValueType.Expression:
					sb.Append("(" + def.Value + ")");
					return;
			}
		}

		private void WriteAutoIncrementDefault(Field field, StringBuilder sb){
			var seq = field.Table.SqlObjects.OfType<Sequence>().FirstOrDefault();
			if (seq != null){
				var seqname = seq.FullName;
				if (Dialect == SqlDialect.SqlServer){
					sb.Append("(NEXT VALUE FOR " + seqname + ")");
				}
				else if (Dialect == SqlDialect.PostGres){
					sb.Append("(nextval('" + seqname + "'))");
				}
				else{
					sb.Append("0");
				}
			}
			else{
				sb.Append("0");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher()
		{
			return "Table " + Table.FullSqlName;
		}
	}
}