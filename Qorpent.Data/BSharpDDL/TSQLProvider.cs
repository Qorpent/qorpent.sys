﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.BSharpDDL{
	internal class TSQLProvider : SqlProviderBase{
		public override string GetSql(DbObject dbObject, DbGenerationMode mode, object hintObject){
			switch (dbObject.ObjectType){
					case DbObjectType.Field:
						return GetField(dbObject as DbField, mode, hintObject);
					case DbObjectType.Table:
						return GetTable(dbObject as DbTable, mode, hintObject);
				default:
					throw new NotImplementedException(dbObject.ObjectType.ToString());
			}
		}


		protected override void CheckScriptDelimiter(DbGenerationMode mode, StringBuilder sb){
			if (mode.HasFlag(DbGenerationMode.Script)){
				sb.AppendLine("GO");
			}
		}

		protected override string GetEnsureSchema(string schema,DbGenerationMode mode){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				return "IF SCHEMA_ID('" + schema + "') IS NULL EXEC sp_executesql N'CREATE SCHEMA " + schema + "'";
			}
			else{
				return "CREATE SCHEMA " + schema;
			}
			
		}

		protected override void GenerateTemporalUtils (StringBuilder sb, DbGenerationMode mode, object hintObject){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				sb.AppendLine(@"
IF OBJECT_ID('tempdb..#ensurefld') IS NOT NULL DROP PROC #ensurefld
IF OBJECT_ID('tempdb..#ensureunq') IS NOT NULL DROP PROC #ensureunq
IF OBJECT_ID('tempdb..#ensurefk') IS NOT NULL DROP PROC #ensurefk
GO
CREATE PROCEDURE #ensurefld @t nvarchar(255), @f nvarchar(255) , @sql nvarchar(max) as begin
	declare @q  nvarchar(max) set @q = 'ALTER TABLE  '+@t+' ADD '+@f+' '+@sql
	IF NOT EXISTS ( SELECT TOP 1 * FROM sys.columns where object_id = OBJECT_ID(@t) and name = @f ) 
		EXEC sp_executesql @q
END
GO
CREATE PROCEDURE #ensureunq @t nvarchar(255), @f nvarchar(255) as begin
	declare @n nvarchar(255) set @n = 'UQ_'+upper(replace(@t,'.','_'))+'_'+upper(@f)
	if not exists (SELECT *  FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE 
    WHERE CONSTRAINT_NAME = @n) begin
		declare @q nvarchar(max) set @q = 'ALTER TABLE '+@t+' ADD CONSTRAINT '+@n+' UNIQUE ('+@f+')'
		exec sp_executesql @q
	end
END
GO
CREATE PROCEDURE #ensurefk @t nvarchar(255), @f nvarchar(255) , @rt nvarchar(255), @rf nvarchar(255), @cu bit as begin
	declare @n nvarchar(255) set @n = 'FK_'+upper(replace(@t,'.','_'))+'_'+upper(@f)
	declare @upd nvarchar(255) set @upd = ' ON UPDATE CASCADE '
	if(@cu = 0) set @upd = ''
	if not exists (SELECT *  FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
    WHERE CONSTRAINT_NAME = @n) begin
		declare @q nvarchar(max) set @q = 'ALTER TABLE '+@t+' ADD CONSTRAINT '+@n+' FOREIGN KEY ('+@f+')  REFERENCES '+@rt +'('+@rf+')'+@upd
		exec sp_executesql @q
	end
END
GO");
			}
		}

		protected override void GenerateConstraints(DbObject[] ordered, StringBuilder sb, DbGenerationMode mode, object hintObject){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				var nonpkfields = ordered.OfType<DbTable>().SelectMany(_ => _.Fields.Values).Where(_ => !_.IsPrimaryKey);
				foreach (var nonpkfield in nonpkfields){
					if (nonpkfield.IsUnique){
						sb.AppendLine(string.Format("exec #ensureunq '{0}.{1}','{2}'", nonpkfield.Table.Schema, nonpkfield.Table.Name,
						                            nonpkfield.Name));
						CheckScriptDelimiter(mode,sb);
					}
					if (nonpkfield.IsRef)
					{
						sb.AppendLine(string.Format("exec #ensurefk '{0}.{1}','{2}','{3}','{4}',{5}", nonpkfield.Table.Schema, nonpkfield.Table.Name,
							nonpkfield.Name,nonpkfield.RefTable,nonpkfield.RefField,nonpkfield.NoCascadeUpdates?0:1));
						CheckScriptDelimiter(mode, sb);
					}
				}
			}
		}

		protected override void DropTemporalUtils(StringBuilder sb, DbGenerationMode mode, object hintObject)
		{
			if (mode.HasFlag(DbGenerationMode.Safe)){
				sb.AppendLine("DROP PROC #ensurefld");
				sb.AppendLine("DROP PROC #ensureunq");
				sb.AppendLine("DROP PROC #ensurefk");
				CheckScriptDelimiter(mode, sb);
			}
		}


		private string GetTable(DbTable dbTable, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			CheckStartSafeBlock(mode,sb);
			var fullname = GetFullName(dbTable,mode);
			if (!string.IsNullOrWhiteSpace(dbTable.Comment)){
				sb.AppendLine("-- " + dbTable.Comment);
			}
			sb.AppendLine("CREATE TABLE " + fullname + " (");
			var _fields = dbTable.Fields.Values
			                    .OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
			var fields = _fields;
			if (mode.HasFlag(DbGenerationMode.Safe)){
				fields = fields.Where(_ => _.IsPrimaryKey).ToArray();
			}
			sb.AppendLine(string.Join(",\r\n", fields.Select(_ => GetInTableFieldString(mode, hintObject, _))));

			sb.AppendLine(")");
			CheckException(mode, sb,  "IF (ERROR_NUMBER()=2714) PRINT 'Таблица \"" + dbTable.Schema + "." + dbTable.Name +
			                    "\" была создана ранее'");
			CheckScriptDelimiter(mode,sb);

			WriteSafeModeFileds(mode, hintObject, _fields, sb);

			GenerateRequiredRecord(dbTable, mode, sb, fullname, _fields);

			WriteTableDescription(dbTable, mode, sb);
			return sb.ToString();
		}

		private void GenerateRequiredRecord(DbTable dbTable, DbGenerationMode mode, StringBuilder sb, string fullname,
		                                    DbField[] _fields){
			if (dbTable.RequireDefaultRecord){
				sb.AppendLine(string.Format("IF NOT EXISTS (SELECT TOP 1 Id from {0} where Id=-1) BEGIN", fullname));
				var pk = _fields.First(_ => _.IsPrimaryKey);
				if (pk.IsIdentity){
					sb.AppendLine("\tSET IDENTITY_INSERT " + fullname + " ON");
				}
				var intos = "Id";
				var values = "-1";
				if (_fields.Any(_ => _.Name == "Code" && _.IsUnique)){
					intos += ",Code";
					values += ",'/'";
				}
				if (_fields.Any(_ => _.Name == "Name"))
				{
					intos += ",Name";
					values+= ",'/'";
				}
				sb.AppendLine(string.Format("\tINSERT INTO {0} ({1}) VALUES ({2}) ", fullname, intos, values));
				if (pk.IsIdentity){
					sb.AppendLine("\tSET IDENTITY_INSERT " + fullname + " OFF");
				}
				sb.AppendLine("END");
				CheckScriptDelimiter(mode, sb);
			}
		}

		private  void WriteTableDescription(DbTable dbTable, DbGenerationMode mode, StringBuilder sb){
			if (!string.IsNullOrWhiteSpace(dbTable.Comment)){
				CheckStartSafeBlock(mode, sb);
				sb.AppendLine("EXECUTE sp_addextendedproperty N'MS_Description', '" + dbTable.Comment + "', N'SCHEMA', N'" +
				              dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "'");
				if (mode.HasFlag(DbGenerationMode.Safe)){
					WriteEndSafeBlock(sb);
					sb.AppendLine("EXECUTE sp_updateextendedproperty N'MS_Description', '" + dbTable.Comment + "', N'SCHEMA', N'" +
					              dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "'");
					WriteEndExceptionBlock(sb);
				}
			}
			foreach (var fld in dbTable.Fields.Values.Where(_ => !string.IsNullOrWhiteSpace(_.Comment)))
			{
				CheckStartSafeBlock(mode, sb);
				sb.AppendLine("EXECUTE sp_addextendedproperty N'MS_Description', '" + fld.Comment + "', N'SCHEMA', N'" +
							  dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "', N'COLUMN', N'" + fld.Name + "'");
				CheckException(mode, sb, "EXECUTE sp_updateextendedproperty N'MS_Description', '" + fld.Comment + "', N'SCHEMA', N'" +
							  dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "', N'COLUMN', N'" + fld.Name + "'");
				CheckScriptDelimiter(mode, sb);
			}
		}

		private void WriteSafeModeFileds(DbGenerationMode mode, object hintObject, DbField[] _fields, StringBuilder sb){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				foreach (var fld in _fields.Where(_ => !_.IsPrimaryKey)){
					sb.AppendLine(string.Format("EXEC #ensurefld '{0}.{1}','{2}','{3}'", fld.Table.Schema, fld.Table.Name, fld.Name,
					                            GetField(fld, mode, hintObject, true).Replace("'","''")));
					CheckScriptDelimiter(mode, sb);
				}
			}
		}

		private static string GetFullName(DbObject obj,DbGenerationMode mode){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				return "[" + obj.Schema + "].[" + obj.Name + "]";
			}
			else{
				return obj.Schema + "." + obj.Name;
			}
		}

		private string GetInTableFieldString(DbGenerationMode mode, object hintObject, DbField field){
			var result = "\t" + GetSql(field, mode, hintObject);
			if (!string.IsNullOrWhiteSpace(field.Comment)){
				result = "\t-- " + field.Comment + "\r\n" + result;
			}
			return result;
		}

		private static void CheckException(DbGenerationMode mode, StringBuilder sb, string exceptioncode){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				WriteEndSafeBlock(sb);
				sb.AppendLine(exceptioncode);
				WriteEndExceptionBlock(sb);
			}
		}

		private static void CheckStartSafeBlock(DbGenerationMode mode, StringBuilder sb){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				sb.AppendLine("BEGIN TRY");
			}
		}

		private static void WriteEndExceptionBlock(StringBuilder sb){
			sb.AppendLine("IF (@@TRANCOUNT!=0) ROLLBACK");
			sb.AppendLine("END CATCH");
		}

		private static void WriteEndSafeBlock(StringBuilder sb){
			sb.AppendLine("END TRY");
			sb.AppendLine("BEGIN CATCH");
		}

		private string GetField(DbField field, DbGenerationMode mode, object hintObject, bool restonly = false){
			var name = field.Name;
			if (mode.HasFlag(DbGenerationMode.Safe)){
				name = "[" + name + "]";
			}
			var result = string.Format("{0} {1} NOT NULL",restonly?"": name, GetSql(field.DataType));
			
			if (field.IsIdentity)
			{
				result += " IDENTITY (10,10) ";
			}
			var refname = field.Name.ToUpper();
			if(null!=field.Table)refname=field.Table.Schema.ToUpper() + "_" + field.Table.Name.ToUpper() + "_" + field.Name.ToUpper();
			if (field.IsPrimaryKey)
			{
				result += " CONSTRAINT PK_" + refname + " PRIMARY KEY";
			}
			if (!mode.HasFlag(DbGenerationMode.Safe)){
				
			
				
				if (field.IsUnique){
					result += " CONSTRAINT UQ_" + refname + " UNIQUE";
				}
				if (field.IsRef){
					result += " CONSTRAINT FK_" + refname + " FOREIGN KEY REFERENCES " + field.RefTable + " (" + field.RefField +
						")  ";
					if (!field.NoCascadeUpdates){
						result += " ON UPDATE CASCADE ";
					}
				}
			}
			if (null!=field.DefaultValue){
				result += GetSql(field, field.DefaultValue);
			}
			return result;
		}

		private string GetSql(DbField dbObject, DbDefaultValue defaultValue){

			if (dbObject.IsIdentity) return "";
			if (dbObject.IsPrimaryKey) return "";
			if (defaultValue.DefaultValueType == DbDefaultValueType.Native){
				if (dbObject.DataType.DbType == DbType.String){
					return " DEFAULT '" + defaultValue.Value.ToStr().Replace("'", "''") + "'";
				}else if (dbObject.DataType.DbType == DbType.Decimal){
					return " DEFAULT " + defaultValue.Value.ToDecimal();
				}else if (dbObject.DataType.DbType == DbType.DateTime){
					return " DEFAULT '" + defaultValue.Value.ToDate().ToString("yyyy-MM-dd hh:mm:ss") + "'";
				}
				else{
					return " DEFAULT " + defaultValue.Value.ToInt();
				}
			}else if (defaultValue.DefaultValueType == DbDefaultValueType.String){
				return " DEFAULT '" + defaultValue.Value.ToStr().Replace("'","''") + "'";
			}
			else{
				return " DEFAULT (" + defaultValue.Value.ToStr()+")";
			}
		}


		protected override string GetSql(DbDataType dataType){
			switch (dataType.DbType){
					case DbType.AnsiString:
						goto case DbType.String;
					case DbType.StringFixedLength:
						goto case DbType.String;
					case DbType.String:
						int size = dataType.Size;
						if (dataType.Size == 0){
							size = DEFAULTSTRINGSIZE;
						}
						if (size == -1){
							return "nvarchar(max)";
						}
					return "nvarchar(" + dataType.Size + ")";
				case DbType.Int32:
					return "int";
					case DbType.Int64:
					return "bigint";
					case DbType.Boolean:
					return "bit";
				case DbType.Decimal:
					var dsize = dataType.Size;
					if (0 >= dsize) dsize = DEFAULTDECIMALSIZE;
					var precession = dataType.Precession;
					if (0 >= precession) precession = DEFAULTDECIMALPRECESSION;
					return "decimal(" + dsize + "," + precession + ")";
				case DbType.Double:
					return "float";
				default: throw new Exception("unknown type "+dataType.DbType );

			}
		}
	}
}