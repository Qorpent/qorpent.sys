﻿using System.Linq;
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
					WriteField(fields[f], sb, f == fields.Length - 1);
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
			          Table.PrimaryKey.Name.SqlQuoteName() + "=" + id + ")  INSERT " + Table.FullSqlName + " (" +
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

		private void WriteField(Field field, StringBuilder sb, bool last){
			if (!string.IsNullOrWhiteSpace(field.Comment) && !NoComment){
				sb.AppendLine("\t-- " + field.Comment);
			}
			sb.AppendFormat("\t{0} {1} NOT NULL", field.Name.SqlQuoteName(), field.DataType.ResolveSqlDataType(Dialect));
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
			sb.Append(" CONSTRAINT " + field.GetConstraintName("FK") + " FOREIGN KEY REFERENCES " +
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

		private void WriteDefaultValue(Field field, StringBuilder sb){
			sb.Append(" DEFAULT ");
			if (field.IsAutoIncrement){
				WriteAutoIncrementDefault(field, sb);
			}
			else{
				WriteUsualDefault(field, sb);
			}
		}

		private void WriteUsualDefault(Field field, StringBuilder sb){
			DefaultValue def = field.DefaultSqlValue;
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