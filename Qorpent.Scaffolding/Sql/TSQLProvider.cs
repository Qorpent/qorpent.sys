using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.SqlGeneration{
	internal class TSQLProvider : SqlProviderBase{

		public TSQLProvider(){
			UseNewFeatures = true;
		}
		public override string GetSql(DbObject dbObject, DbGenerationMode mode, object hintObject){
			
			if (mode.HasFlag(DbGenerationMode.Drop)){
				var prefix = "IF OBJECT_ID('" + dbObject.FullName + "') IS NOT NULL DROP ";
				switch (dbObject.ObjectType)
				{
					case  DbObjectType.FileGroup:
						return "";
				
					case DbObjectType.Table:
						return prefix+"TABLE " + dbObject.FullName;
					case DbObjectType.Function:
						return prefix +"FUNCTION " + dbObject.FullName;
					case DbObjectType.Procedure:
						return prefix+"PROCEDURE " + dbObject.FullName;
					case DbObjectType.Trigger:
						return prefix+"TRIGGER " + dbObject.FullName;
					case DbObjectType.View:
						return prefix + "VIEW " + dbObject.FullName;
					default:
						throw new NotImplementedException(dbObject.ObjectType.ToString());
				}
			}
			else{
				if (dbObject.IsRawSql){
					var sb = new StringBuilder();
					var normalized = NormalizeSql(dbObject,dbObject.RawSql);
					sb.AppendLine(normalized);
					CheckScriptDelimiter(mode,sb);
					return sb.ToString();

				}
				switch (dbObject.ObjectType){
					case DbObjectType.FileGroup:
						return GetFileGroup(dbObject as DbFileGroup, mode, hintObject);
					case DbObjectType.Field:
						return GetField(dbObject as DbField, mode, hintObject);
					case DbObjectType.Table:
						return GetTable(dbObject as DbTable, mode, hintObject);
					case DbObjectType.Function:
						return GetFunction(dbObject as DbFunction, mode, hintObject);
					case DbObjectType.Trigger:
						return GetTrigger(dbObject as DbTrigger, mode, hintObject);
					case DbObjectType.Procedure:
						return GetProcedure(dbObject as DbFunction, mode, hintObject);
					case DbObjectType.View:
						return GetView(dbObject as DbView, mode, hintObject);
					default:
						throw new NotImplementedException(dbObject.ObjectType.ToString());
				}
			}

		}
		private string tobit(object any){
			return any.ToBool() ? "1" : "0";
		}
		private string GetFileGroup(DbFileGroup dbFileGroup, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			sb.AppendLine(string.Format("exec #ensurefg '{0}',{1},{2},{3},{4}", dbFileGroup.Name ,dbFileGroup.FileCount,dbFileGroup.FileSize,tobit(dbFileGroup.WithIdx),tobit(dbFileGroup.IsDefault)));
				 return sb.ToString();
		}

		private string NormalizeSql(DbObject dbObject, string rawSql){
			var result = rawSql;
			if (rawSql.Contains("--PARENT_FIELD_SET--")){
				var sb = new StringBuilder();
				foreach (var fld in (dbObject.ParentElement as DbTable).Fields.Values.OrderBy(_=>_.Idx)){
					sb.AppendLine(fld.Name + ", --" + fld.Comment);
				}
				result = result.Replace("--PARENT_FIELD_SET--", sb.ToString());
			}
			result = EmbedReferenceSubQueries(dbObject,result);
			return result;
		}

		private static string EmbedReferenceSubQueries(DbObject obj,string result){
			return Regex.Replace(result,
			                       @"(?ix)--\s*PARENT_REF_SET\sFOR\s\((?<fields>[^\)]+)\)\sWITH\s\((?<outers>[^\)]+)\)\s*--",
			                       match
			                       => {
				                       var fields = match.Groups["fields"].Value.SmartSplit();
									   var outers = match.Groups["outers"].Value.SmartSplit();
				                       var sb = new StringBuilder();
				                       foreach (var field in fields){
					                       var table = obj.ParentElement as DbTable; 
					                       var fld = table.Fields[field];
					                       foreach (var outer in outers){
						                       sb.AppendLine(
							                       string.Format("(select x.{1} from {0} x where x.Id = {2}.{3}.{4}) as {4}{1},",
							                                     fld.RefTable, outer, table.Schema, table.Name, fld.Name
								                       ));
					                       }
				                       }

				                       return sb.ToString();
			                       });
			
		}

		private string GetView(DbView dbView, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			var name = dbView.Schema + "." + dbView.Name;
			sb.AppendLine(string.Format("CREATE VIEW {0}   AS ", name));
			var sql = NormalizeSql(dbView, dbView.Body).Trim();
			if (sql.EndsWith(",")){
				sql = sql.Substring(0, sql.Length - 1);
			}
			if (!sql.Contains("SELECT")){
				sql = " SELECT \r\n" + sql + "\r\n FROM " + dbView.ParentElement.Schema +
				      "." + dbView.ParentElement.Name;
			}
			sb.AppendLine(sql);
			return sb.ToString();
		}

		private string GetTrigger(DbTrigger dbTrigger, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			var name = dbTrigger.Schema + "." + dbTrigger.Name;
			var tablename = dbTrigger.ParentElement.Schema + "." + dbTrigger.ParentElement.Name;
			sb.AppendLine(string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP TRIGGER {0}", name,tablename));
			CheckScriptDelimiter(mode, sb);
			sb.AppendLine("-- " + dbTrigger.Comment);
			var trigtype = "AFTER";
			if (dbTrigger.Before){
				trigtype = "INSTEAD OF";
			}
			var operations = new List<string>();
			if(dbTrigger.Insert)operations.Add("INSERT");
			if (dbTrigger.Update) operations.Add("UPDATE");
			if (dbTrigger.Delete) operations.Add("DELETE");
			var ops = string.Join(",", operations);
			
			sb.AppendLine(string.Format("CREATE TRIGGER {0}  ON {1} {2} {3} AS BEGIN", name,tablename,trigtype,ops));
			sb.AppendLine(NormalizeSql(dbTrigger,dbTrigger.Body));
			sb.AppendLine("END");
			return sb.ToString();
		}

		private string GetProcedure(DbFunction dbFunction, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			var name = dbFunction.Schema + "." + dbFunction.Name;
			sb.AppendLine(string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP PROC {0}", name));
			CheckScriptDelimiter(mode, sb);
			sb.AppendLine("-- " + dbFunction.Comment);
			sb.AppendLine(string.Format("CREATE PROCEDURE {0}  {1}  AS BEGIN", name,
				string.Join(", ", dbFunction.Parameters.Select(_ => "@" + _.Code + " " + GetSql(_.DataType)+" = null"))
				));
			sb.AppendLine(NormalizeSql(dbFunction,dbFunction.Body));
			sb.AppendLine("END");
			return sb.ToString();
		}

		private string GetFunction(DbFunction dbFunction, DbGenerationMode mode, object hintObject){
			var sb = new StringBuilder();
			var name = dbFunction.Schema + "." + dbFunction.Name;
			sb.AppendLine(string.Format("IF OBJECT_ID('{0}') IS NOT NULL DROP FUNCTION {0}", name));
			CheckScriptDelimiter(mode,sb);
			sb.AppendLine("-- " + dbFunction.Comment);
			sb.AppendLine(string.Format("CREATE FUNCTION {0} ( {1} ) RETURNS {2} AS BEGIN", name ,
				string.Join(", ",dbFunction.Parameters.Select(_=>"@"+_.Code+" "+GetSql(_.DataType))),
				GetSql(dbFunction.ReturnType)
				));
			sb.AppendLine( NormalizeSql(dbFunction,dbFunction.Body));
			sb.AppendLine("END");
			return sb.ToString();
		}


		protected override void CheckScriptDelimiter(DbGenerationMode mode, StringBuilder sb){
			if (mode.HasFlag(DbGenerationMode.Script)){
				sb.AppendLine("GO");
			}
		}

		protected override string GetEnsureSchema(string schema,DbGenerationMode mode){
			if (schema == "dbo") return "";
			if (mode.HasFlag(DbGenerationMode.Safe)){
				return "IF SCHEMA_ID('" + schema + "') IS NULL EXEC sp_executesql N'CREATE SCHEMA " + schema + "' \r\nGO\r\n";
			}
			else{
				return "CREATE SCHEMA " + schema + "\r\nGO\r\n";
			}
			
		}
		/// <summary>
		/// Использовать новые фичи
		/// </summary>
		public bool UseNewFeatures { get; set; }

		protected override string GetEnsureSequence(DbField dbField){
			if (!UseNewFeatures) return "";
			var name = dbField.Table.Schema + "." + dbField.Table.Name + "_SEQ";
			var dtype = GetSql(dbField.DataType);
			return string.Format("IF OBJECT_ID('{0}') IS NULL CREATE SEQUENCE {0} AS {1} START WITH 10  INCREMENT BY 10 CACHE 10",name,dtype);
		}

		protected override string GetDropSequence(DbField fld){
			if (!UseNewFeatures) return "";
			var name = fld.Table.Schema + "." + fld.Table.Name + "_SEQ";
			return string.Format("DROP SEQUENCE {0}", name);
		}

		protected override void GenerateTemporalUtils (StringBuilder sb, DbGenerationMode mode, object hintObject){
			sb.AppendLine(@"
IF OBJECT_ID('tempdb..#ensurefg') IS NOT NULL DROP PROC #ensurefg
GO
CREATE PROCEDURE #ensurefg @n nvarchar(255),@filecount int = 1, @filesize int = 100, @withidx bit = 0, @isdefault bit = 0 AS begin
	declare @q nvarchar(max) 
	set @filesize = isnull(@filesize,100)
	if @filesize <=3 set @filesize  =3
	set @filecount = ISNULL(@filecount,1)
	if @filecount < 1 set @filecount= 1
	set @withidx = isnull(@withidx,0)
	set @isdefault = isnull(@isdefault,0)
	set @q = 'ALTER DATABASE '+DB_NAME()+' ADD FILEGROUP '+@n
	print @q
	BEGIN TRY
		exec sp_executesql @q
	END TRY
	BEGIN CATCH
	END CATCH
	declare @basepath nvarchar(255) set @basepath = reverse((Select top 1 filename from sys.sysfiles))
	set @basepath = REVERSE( RIGHT( @basepath, len(@basepath)-CHARINDEX('\',@basepath)+1))

	declare @c int set @c = @filecount 
	while @c >= 1 begin
		BEGIN TRY
			set @q='ALTER DATABASE '+DB_NAME()+' ADD FILE ( NAME = N'''+DB_NAME()+'_'+@n+cast(@c as nvarchar(255))+''', FILENAME = N'''+
				@basepath+'z3_'+@n+cast(@c as nvarchar(255))+'.ndf'' , SIZE = '+cast(@filesize as nvarchar(255))+'MB , FILEGROWTH = 5% ) TO FILEGROUP ['+@n+']'
			print @q
			exec sp_executesql @q
		END TRY
		BEGIN CATCH
		END CATCH
		set @c = @c - 1
	end

	IF @isdefault = 1 BEGIN
		set @q='ALTER DATABASE '+DB_NAME()+' MODIFY FILEGROUP '+@n+' DEFAULT '
		BEGIN TRY
			exec sp_executesql @q
		END TRY 
		BEGIN CATCH
		END CATCH
	end

	IF @withidx = 1 BEGIN
		set @n = @n +'IDX'
		exec #ensurefg @n, @filecount,@filesize,0,0
	END
end
GO
");
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
	--declare @upd nvarchar(255) set @upd = ' ON UPDATE CASCADE '
	--if(@cu = 0) set @upd = ''
	if not exists (SELECT *  FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS 
    WHERE CONSTRAINT_NAME = @n) begin
		declare @q nvarchar(max) set @q = 'ALTER TABLE '+@t+' ADD CONSTRAINT '+@n+' FOREIGN KEY ('+@f+')  REFERENCES '+@rt +'('+@rf+')' --+@upd
		exec sp_executesql @q
	end
END
GO");
			}
		}
		/// <summary>
		/// /
		/// </summary>
		/// <param name="schema"></param>
		/// <returns></returns>
		protected override string GetDropSchema(string schema){
			if (schema == "dbo") return "";
			return string.Format("IF SCHEMA_ID('{0}') IS NOT NULL DROP SCHEMA {0}", schema);
		}

		protected override void GenerateConstraints(DbObject[] ordered, StringBuilder sb, DbGenerationMode mode, object hintObject){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				var nonpkfields = ordered.OfType<DbTable>().SelectMany(_ => _.Fields.Values).Where(_ => !_.IsPrimaryKey);
				foreach (var unq in nonpkfields.Where(_=>_.IsUnique)){
						sb.AppendLine(string.Format("exec #ensureunq '{0}.{1}','{2}'", unq.Table.Schema, unq.Table.Name,
						                            unq.Name));
						CheckScriptDelimiter(mode,sb);
					
					
				}
				foreach (var refer in nonpkfields.Where(_=>_.IsRef)){

						sb.AppendLine(string.Format("exec #ensurefk '{0}.{1}','{2}','{3}','{4}',{5}", refer.Table.Schema, refer.Table.Name,
							refer.Name, refer.RefTable, refer.RefField, refer.NoCascadeUpdates ? 0 : 1));
						CheckScriptDelimiter(mode, sb);
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
			sb.AppendLine("DROP PROC #ensurefg");
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

			sb.AppendLine(") ON ["+dbTable.FileGroupName+"]");
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
				if (pk.IsIdentity && !UseNewFeatures){
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
				if (pk.IsIdentity && !UseNewFeatures){
					sb.AppendLine("\tSET IDENTITY_INSERT " + fullname + " OFF");
				}
				sb.AppendLine("END");
				CheckScriptDelimiter(mode, sb);
			}
		}

		private  void WriteTableDescription(DbTable dbTable, DbGenerationMode mode, StringBuilder sb){
			if (!string.IsNullOrWhiteSpace(dbTable.Comment)){
				CheckStartSafeBlock(mode, sb);
				sb.AppendLine("EXECUTE sp_addextendedproperty N'MS_Description', '" + dbTable.Comment.ToSqlString() + "', N'SCHEMA', N'" +
				              dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "'");
				if (mode.HasFlag(DbGenerationMode.Safe)){
					WriteEndSafeBlock(sb);
					sb.AppendLine("EXECUTE sp_updateextendedproperty N'MS_Description', '" + dbTable.Comment.ToSqlString() + "', N'SCHEMA', N'" +
					              dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "'");
					WriteEndExceptionBlock(sb);
				}
			}
			foreach (var fld in dbTable.Fields.Values.Where(_ => !string.IsNullOrWhiteSpace(_.Comment)))
			{
				CheckStartSafeBlock(mode, sb);
				sb.AppendLine("EXECUTE sp_addextendedproperty N'MS_Description', '" + fld.Comment.ToSqlString() + "', N'SCHEMA', N'" +
							  dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "', N'COLUMN', N'" + fld.Name + "'");
				CheckException(mode, sb, "EXECUTE sp_updateextendedproperty N'MS_Description', '" + fld.Comment.ToSqlString() + "', N'SCHEMA', N'" +
							  dbTable.Schema + "', N'TABLE', N'" + dbTable.Name + "', N'COLUMN', N'" + fld.Name + "'");
				CheckScriptDelimiter(mode, sb);
			}
		}

		private void WriteSafeModeFileds(DbGenerationMode mode, object hintObject, DbField[] _fields, StringBuilder sb){
			if (mode.HasFlag(DbGenerationMode.Safe)){
				foreach (var fld in _fields.Where(_ => !_.IsPrimaryKey)){
					sb.AppendLine(string.Format("EXEC #ensurefld '{0}.{1}','{2}','{3}'", fld.Table.Schema, fld.Table.Name, fld.Name,
					                            GetField(fld, mode, hintObject, true).ToSqlString()));
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
				if (UseNewFeatures){
					var seq = field.Table.Schema + "." + field.Table.Name + "_SEQ";
					result += " DEFAULT  (NEXT VALUE FOR " + seq + ") ";
				}
				else{
					result += " IDENTITY (10,10) ";
				}
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
					//if (!field.NoCascadeUpdates){
					//	result += " ON UPDATE CASCADE ";
					//}
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
					return " DEFAULT '" + defaultValue.Value.ToSqlString() + "'";
				}
				if (dbObject.DataType.DbType == DbType.Decimal){
					return " DEFAULT " + defaultValue.Value.ToDecimal();
				}
				if (dbObject.DataType.DbType == DbType.DateTime){
					try{
						return " DEFAULT '" + defaultValue.Value.ToDate().ToString("yyyyMMdd hh:mm:ss") + "'";
					}
					catch{
						return " DEFAULT (" + defaultValue.Value + ")";
					}
				}
				return " DEFAULT " + defaultValue.Value.ToInt();
			}
			if (defaultValue.DefaultValueType == DbDefaultValueType.String){
				return " DEFAULT '" + defaultValue.Value.ToSqlString() + "'";
			}
			return " DEFAULT (" + defaultValue.Value.ToStr()+")";
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
				case DbType.DateTime:
					return "datetime";
				default: throw new Exception("unknown type "+dataType.DbType );

			}
		}
	}
}