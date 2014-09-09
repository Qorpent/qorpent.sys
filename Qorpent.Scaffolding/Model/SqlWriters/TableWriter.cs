using System.Linq;
using System.Text;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.SqlWriters{
	/// <summary>
	/// </summary>
	public class TableWriter : SqlCommandWriter{
		/// <summary>
		/// </summary>
		/// <param name="cls"></param>
		public TableWriter(PersistentClass cls){
			Table = cls;
		}

		/// <summary>
		/// </summary>
		public PersistentClass Table { get; set; }


		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected override string GetText(){
			if (Mode == ScriptMode.Create){
				var sb = new StringBuilder();
				if (!string.IsNullOrWhiteSpace(Table.Comment) && !NoComment){
					sb.AppendLine("-- " + Table.Comment);
				}
				sb.AppendLine("CREATE TABLE " + Table.FullSqlName + " (");
				Field[] fields = Table.GetOrderedFields().Where(_ => !_.NoSql).ToArray();
				for (int f = 0; f < fields.Length; f++){
					WriteField(Dialect,fields[f], sb, f == fields.Length - 1);
				}
				sb.Append(")");
				WriteAllocation(sb);
				sb.Append(";");
				sb.AppendLine();
				GenerateDefaultRows(sb, 0, "/", "NULL/ROOT");
				GenerateDefaultRows(sb, -1, "ERR", "ERROR/LOST", -1);
				SetSqlComment(sb, Table, null);
				foreach (Field f in fields){
					SetSqlComment(sb, f.Table, f);
				}

				return sb.ToString();
			}
			else{
				return "DROP TABLE " + Table.FullSqlName + ";";
			}
		}

		private void GenerateDefaultRows(StringBuilder sb, int id, string code, string name, int parent = 0){
			sb.Append("IF NOT EXISTS (SELECT TOP 1 * FROM " + Table.FullSqlName + " where " +
			          Table.PrimaryKey.Name.SqlQuoteName() + "=" + id + ")  INSERT INTO " + Table.FullSqlName + " (" +
			          Table.PrimaryKey.Name.SqlQuoteName());
			if (Table.Fields.ContainsKey("code")){
				sb.Append(", " + Table.Fields["code"].Name.SqlQuoteName());
			}
			if (Table.Fields.ContainsKey("name")){
				sb.Append(", " + Table.Fields["name"].Name.SqlQuoteName());
			}
			if (Table.Fields.ContainsKey("parent")){
				sb.Append(", " + Table.Fields["parent"].Name.SqlQuoteName());
			}
			if (id == -1){
				var refs = Table.GetReferences().Where(_ => !_.NoSql && _.Name != "Parent");
				foreach (var r in refs){
					sb.Append(", " + r.Name.SqlQuoteName());
				}

			}
			sb.Append(") VALUES (" + id);
			if (Table.Fields.ContainsKey("code")){
				sb.Append(", '" + code + "'");
			}
			if (Table.Fields.ContainsKey("name")){
				sb.Append(", '" + name.ToSqlString() + "'");
			}
			if (Table.Fields.ContainsKey("parent")){
				sb.Append(", " + parent);
			}
			if (id == -1){
				var refs = Table.GetReferences().Where(_ => !_.NoSql && _.Name != "Parent");
				foreach (var r in refs){
					if (r.DataType.IsString){
						sb.Append(", 'ERR'");
					}
					else{
						sb.Append(", -1");
					}
				}

			}
			sb.Append(");");
			sb.AppendLine();
		}


		private void SetSqlComment(StringBuilder sb, PersistentClass table, Field fld){
			string comment = table.Comment;
			if (null != fld) comment = fld.Comment;
			if (string.IsNullOrWhiteSpace(comment)) return;
			if (Dialect == SqlDialect.SqlServer){
				sb.AppendFormat("EXECUTE sp_addextendedproperty N'MS_Description', '{0}', N'SCHEMA', N'{1}', N'TABLE', N'{2}'",
				                comment.ToSqlString(), table.Schema, table.Name.ToLowerInvariant());
				if (null != fld){
					sb.AppendFormat(", N'COLUMN', '{0}'", fld.Name.ToLowerInvariant());
				}
			}
			else if (Dialect == SqlDialect.PostGres){
				if (null == fld){
					sb.AppendFormat("COMMENT ON TABLE {0} IS '{1}'", table.FullSqlName, comment.ToSqlString());
				}
				else{
					sb.AppendFormat("COMMENT ON COLUMN {0}.{1} IS '{2}'", table.FullSqlName, fld.Name.SqlQuoteName(),
					                comment.ToSqlString());
				}
			}
			sb.Append(";");
			sb.AppendLine();
		}

		private void WriteAllocation(StringBuilder sb){
			if (Dialect == SqlDialect.SqlServer){
				string name = Table.AllocationInfo.FileGroup.Name;
				if (Table.AllocationInfo.Partitioned && Model.IsSupportPartitioning(SqlDialect.SqlServer)){
					name = Table.FullSqlName.Replace(".", "_").Replace("\"","") + "_PARTITION ( " + Table.AllocationInfo.PartitionField.Name + ")";
				}
				sb.Append(" ON " + name);
			}
			else if (Dialect == SqlDialect.PostGres){
				sb.Append(" TABLESPACE " + Table.AllocationInfo.FileGroup.Name);
			}
		}

		private void WriteField(SqlDialect dialect,Field field, StringBuilder sb, bool last){
			if (!string.IsNullOrWhiteSpace(field.Comment) && !NoComment){
				sb.AppendLine("\t-- " + field.Comment);
			}
			string notnull="NOT NULL";
			if (field.IsReference && field.ReferenceClass.TargetClass == null){
				notnull = "";
			}

			if (!string.IsNullOrWhiteSpace(field.ComputeAs)){
				sb.AppendFormat("\t{0} AS {1}", field.Name.SqlQuoteName(),field.ComputeAs);
			}
			else {
				sb.AppendFormat("\t{0} {1} {2}", field.Name.SqlQuoteName(), field.DataType.ResolveSqlDataType(Dialect),notnull);
				WritePrimaryKey(field, sb);
				WriteUnique(field, sb);
				WriteForeignKey(field, sb);
				if (!(field.IsReference && null == field.ReferenceClass.TargetClass)){
					WriteDefaultValue(dialect, field, sb); //not model references - only exception to get default value	
				}
				else{
					sb.Append(" DEFAULT NULL");
				}
				
			}

			if (!last){
				sb.AppendLine(",");
			}
			else{
				sb.AppendLine();
			}
		}

		private void WriteComputeAs(SqlDialect dialect, Field field, StringBuilder sb){
			sb.Append(" AS ");
			sb.Append("(");
			sb.Append(field.ComputeAs);
			sb.Append(")");
		}

		private void WriteForeignKey(Field field, StringBuilder sb){
			if (field.IsPrimaryKey) return;
			if (!field.IsReference) return;
			if (field.GetIsCircular()) return;
			var fk = Dialect == SqlDialect.PostGres ? " " : " FOREIGN KEY ";
			sb.Append(" CONSTRAINT " + field.GetConstraintName("FK") + fk + "REFERENCES " +
			          field.ReferenceClass.FullSqlName + " (" + field.ReferenceField.SqlQuoteName() + ")");
			if (Dialect == SqlDialect.PostGres){
				sb.Append(" DEFERRABLE");
			}
		}

		private void WriteUnique(Field field, StringBuilder sb){
			if (field.IsPrimaryKey) return;
			if (!field.IsUnique) return;
			sb.Append(" CONSTRAINT " + field.GetConstraintName("UNQ") + " UNIQUE");
		}

		private void WritePrimaryKey(Field field, StringBuilder sb){
			if (!field.IsPrimaryKey) return;
			if (field.Table.AllocationInfo.Partitioned && Model.IsSupportPartitioning(Dialect)){
				sb.Append(" CONSTRAINT " + field.GetConstraintName("PK") + " PRIMARY KEY NONCLUSTERED ON " +
				          field.Table.AllocationInfo.FileGroup.Name);
			}
			else{
				sb.Append(" CONSTRAINT " + field.GetConstraintName("PK") + " PRIMARY KEY");
			}
		}

		private void WriteDefaultValue(SqlDialect dialect, Field field, StringBuilder sb)
		{
			sb.Append(" DEFAULT ");
			if (field.IsAutoIncrement){
				WriteAutoIncrementDefault(field, sb);
			}
			else{
				WriteUsualDefault(dialect,field, sb);
			}
		}

		private void WriteUsualDefault(SqlDialect dialect,Field field, StringBuilder sb){
			DefaultValue def = field.DefaultSqlValue;
			switch (def.DefaultValueType){

				case DbDefaultValueType.String:
					sb.Append("'" + def.Value.ToSqlString() + "'");
					return;
				case DbDefaultValueType.Native:
					if (def.Value == null || "".Equals(def.Value)){
						sb.Append("''");
					}else if (def.Value is bool){
						if (dialect == SqlDialect.PostGres){
							sb.Append(def.Value.ToString().ToLower());
						}
						else{
							sb.Append("0");
						}
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
			Sequence seq = field.Table.SqlObjects.OfType<Sequence>().FirstOrDefault();
			if (seq != null){
				string seqname = seq.FullName;
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
		/// </summary>
		/// <returns></returns>
		protected override string GetDigestFinisher(){
			return "Table " + Table.FullSqlName + " (" +
			       string.Join(", ", Table.GetOrderedFields().Where(_ => !_.NoSql).Select(_ => _.Name)) + ")";
		}
	}
}