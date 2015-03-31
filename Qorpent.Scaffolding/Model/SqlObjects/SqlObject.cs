using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Data;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// </summary>
	public abstract class SqlObject{
		private string _name;

		/// <summary>
		/// </summary>
		public SqlObject(){
			Schema = "dbo";
			UseSchemaName = true;
			UseTablePrefixedName = false;
		}

		/// <summary>
		///     Тип объекта
		/// </summary>
		public SqlObjectType ObjectType { get; set; }

		/// <summary>
		/// </summary>
		public bool UseSchemaName { get; set; }

		/// <summary>
		/// </summary>
		public bool UseTablePrefixedName { get; set; }

		/// <summary>
		///     Ссылка на класс-контейнер
		/// </summary>
		public PersistentClass Table { get; set; }

		/// <summary>
		///     B# с определением
		/// </summary>
		public IBSharpClass BSClass { get; set; }

		/// <summary>
		/// </summary>
		public XElement Definition { get; set; }

		/// <summary>
		///     Признак объекта, который должен формироваться до определения таблицы
		/// </summary>
		public bool PreTable { get; set; }

		/// <summary>
		///     Комментарий
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		///     Обратная ссылка на модель
		/// </summary>
		protected PersistentModel Model { get; set; }

		/// <summary>
		///     Имя SQL
		/// </summary>
		public string Name{
			get { return _name; }
			set{
				_name = value;
				if (UpperCase){
					_name = _name.ToUpperInvariant();
				}
				if (LowerCase){
					_name = _name.ToLowerInvariant();
				}
			}
		}

		/// <summary>
		///     Схема SQL
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///     Полное имя SQL
		/// </summary>
		public string FullName{
			get{
				if (string.IsNullOrWhiteSpace(Schema)){
					Schema = "dbo";
				}
				if (string.IsNullOrWhiteSpace(Name)){
					Name = "DEFAULT";
				}
				string result = Name;
				if (UseTablePrefixedName){
					if (null != Table && !result.StartsWith(Table.FullSqlName) && !result.Contains(".")){
						result = Table.FullSqlName + result;
					}
					if (!string.IsNullOrWhiteSpace(TableName) && !result.StartsWith(TableName) && !result.Contains(".")){
						result = TableName + result;
					}
				}
				else if (UseSchemaName){
					if (!result.Contains(".")){
						result = Schema + "." + result;
					}
				}
				result = result.SqlQuoteName(!UseTablePrefixedName);
				return result;
			}
		}

		/// <summary>
		///     Требование к именам - иметь верхний регистр
		/// </summary>
		public bool UpperCase { get; set; }

		/// <summary>
		///     Требование чтобы имена были в нижнем регистре
		/// </summary>
		public bool LowerCase { get; set; }

		/// <summary>
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		///     Диалект
		/// </summary>
		public SqlDialect Dialect { get; set; }

		/// <summary>
		///     Файл определением (только тело)
		/// </summary>
		public string ExternalBody { get; set; }

		/// <summary>
		///     Файл с внешним (полным) определением
		/// </summary>
		public string External { get; set; }

		/// <summary>
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		///     Формирует SQL-объект
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cls"></param>
		/// <param name="bscls"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public virtual SqlObject Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml){
			Table = cls;
			Model = model;
			BSClass = bscls;
			if (null == Model && null != cls){
				Model = cls.Model;
			}
			Definition = xml;
			if (null != xml){
				Name = xml.ChooseAttr("sqlname", "code");
				Comment = xml.Attr("name");
				External = xml.GetSmartValue("external");
				ExternalBody = xml.GetSmartValue("externalbody");
				Dialect = SqlDialect.Ansi;
				string dialect = xml.GetSmartValue("dialect");
				if (!string.IsNullOrWhiteSpace(dialect)){
					Dialect = dialect.To<SqlDialect>();
				}
				Body = xml.Value;
			}


			return this;
		}

		/// <summary>
		///     Формирует глобальные объекты уровня базы данных
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDatabaseWide(PersistentModel model){
			foreach (SqlObject schema in GenerateSchemas(model)){
				yield return schema;
			}
			foreach (SqlObject fgroup in GenerateFileGroups(model)){
				yield return fgroup;
			}
		}

		private static IEnumerable<SqlObject> GenerateSchemas(PersistentModel model){
			foreach (string schema in model.Classes.Values.Select(_ => _.Schema.ToLowerInvariant()).Distinct().ToArray()){
				if ("dbo" == schema) continue;
				yield return new Schema{Name = schema.ToLowerInvariant()};
			}
		}

		private static IEnumerable<SqlObject> GenerateFileGroups(PersistentModel model){
			var fgs = new Dictionary<string, FileGroup>();
			foreach (IBSharpClass fgd in model.Context.ResolveAll(model.FileGroupPrototype)){
				FgFromClass(model, fgd, fgs);
			}
			foreach (PersistentClass pcls in model.Classes.Values){
				IBSharpClass _pcls = model.Context.Get(pcls.AllocationInfo.FileGroupName);
				pcls.AllocationInfo.FileGroupName = pcls.AllocationInfo.FileGroupName.ToUpper();

				if (null != _pcls){
					pcls.AllocationInfo.FileGroup = FgFromClass(model, _pcls, fgs);
				}
				else{
					if (!fgs.ContainsKey(pcls.AllocationInfo.FileGroupName)){
						fgs[pcls.AllocationInfo.FileGroupName] = new FileGroup{Name = pcls.AllocationInfo.FileGroupName};
					}
					pcls.AllocationInfo.FileGroup = fgs[pcls.AllocationInfo.FileGroupName];
				}
			}
			if (!fgs.ContainsKey("SECONDARY")){
				fgs["SECONDARY"] = new FileGroup{Name = "SECONDARY"};
			}
			bool hasdef = fgs.Values.Any(_ => _.IsDefault);
			if (!hasdef){
				fgs["SECONDARY"].IsDefault = true;
			}
			return fgs.Values;
		}

		private static FileGroup FgFromClass(PersistentModel model, IBSharpClass fgd, Dictionary<string, FileGroup> fgs){
			if (fgs.ContainsKey(fgd.Name)) return fgs[fgd.Name.ToUpper()];
			var fg = new FileGroup();
			fg.Setup(model, null, fgd, fgd.Compiled);
			fg.Name = fg.Name.ToUpper();
			fgs[fg.Name.ToUpper()] = fg;
			return fg;
		}

		/// <summary>
		///     Формирует стандартные объекты для таблицы
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDefaults(PersistentClass cls){
			if (cls.PrimaryKey.IsAutoIncrement){
				yield return new Sequence().Setup(null, cls, null, null);
			}
			if (cls.Model.GenerationOptions.GeneratePartitions && cls.AllocationInfo.Partitioned){
				yield return new PartitionDefinition().Setup(null, cls, null, null);
				if (null != cls.AllocationInfo.PartitionField){
					yield return GetAutoPartition(cls);
				}
			}
			yield return PreventDeleteSysTrigger(cls);

		}

		private static SqlTrigger PreventDeleteSysTrigger(PersistentClass cls){
			var result = new SqlTrigger();
			result.Dialect = SqlDialect.SqlServer;
			result.Table = cls;
			result.TableName = cls.FullSqlName;
			result.Name = "PreventDeletionOfSystemDefinedRows";
			result.Delete = true;
			result.Before = true;
			result.Body = "delete @this from deleted d join @this on @this.id = d.id where @this.id not in (0,-1)";
			return result;
		}

		private static SqlFunction GetAutoPartition(PersistentClass cls)
		{
			var sb = new StringBuilder();
			var a = cls.AllocationInfo;
			
			var dtype = a.PartitionField.DataType.ResolveSqlDataType(SqlDialect.SqlServer);
			var result = new SqlFunction();
			result.UseTablePrefixedName = false;
			result.UseSchemaName = true;
			result.Schema = cls.Schema;
			result.Name = cls.Name + "AlignPartitions";
			result.IsProcedure = true;
			result.Dialect = SqlDialect.SqlServer;

			sb.AppendFormat(@"declare @fullparts table ( num int, limit {0})
while ( 1 = 1 ) begin
	delete @fullparts
	insert @fullparts (num) 
		select partition_number from sys.dm_db_partition_stats where object_id = OBJECT_ID('{1}') and index_id =0 and row_count > 1000000
	if (select count (*) from @fullparts ) = 0  break;
	update @fullparts set limit = lq.{2} from (
		select part, min({2}) as {2} from (
		select  part, {2},sum(cnt) over (order by {2}) as sum from (
		select  $PARTITION.{3}_{4}_PARTITIONFunc({2}) as part,{2},count(*) as cnt from {1}
		where $PARTITION.{3}_{4}_PARTITIONFunc({2}) in (select num from @fullparts)
		group by {2},$PARTITION.{3}_{4}_PARTITIONFunc({2})
		) as t 
		)as c where c.sum >=900000 group by part 
	) as lq join @fullparts f2 on f2.num = lq.part
	declare @q nvarchar(max) set @q = ''
	if '{0}' = 'datetime' 
		select @q = @q + '
			ALTER PARTITION SCHEME {3}_{4}_PARTITION NEXT USED {5}
			ALTER PARTITION FUNCTION {3}_{4}_PARTITIONFunc() SPLIT RANGE ('''+convert(varchar(10),limit,112)+''')
		' from @fullparts
	else 
			select @q = @q + '
			ALTER PARTITION SCHEME {3}_{4}_PARTITION NEXT USED {5}
			ALTER PARTITION FUNCTION {3}_{4}_PARTITIONFunc() SPLIT RANGE ('+cast(limit as varchar(20))+')
		' from @fullparts
	print @q
	exec sp_executesql @q
end
", dtype, cls.FullSqlName,
	a.PartitionField.Name, cls.Schema, cls.Name, a.FileGroupName);
			result.Body = sb.ToString();
			return result;
		}

		/// <summary>
		///     Формирует специальные объекты, определенные в таблице, конкретный элемент
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> Create(PersistentClass cls, XElement e){
			string name = e.Name.LocalName;
			if (name == "trigger"){
				yield return new SqlTrigger().Setup(null, cls, null, e);
			}
			else if (name == "view"){
				yield return new SqlView().Setup(null, cls, null, e);
			}
			else if (name == "function" || name == "void" || !string.IsNullOrWhiteSpace(e.Value)){
				yield return new SqlFunction().Setup(null, cls, null, e);
            }
            else if (name == "script") {
                yield return new SqlScript().Setup(cls.Model,cls.TargetClass, e);
            }
		}

		/// <summary>
		///     Возвращает контент
		/// </summary>
		/// <returns></returns>
		public string ResolveBody(){
			string result = GetRawBody().Trim();
			if (result.StartsWith("(") && result.EndsWith(")")){
				result = result.Substring(1, result.Length - 2).Trim();
			}
			string tname = Table == null ? TableName.SqlQuoteName() : Table.FullSqlName;
			result = Regex.Replace(result, @"@this\.([\w_]+)\s*\(",  tname.Substring(0,tname.Length-1) + "$1\"(");
			result = Regex.Replace(result, @"@this\.(_SEQ)",  tname.Substring(0,tname.Length-1) + "$1\"");
			result = Regex.Replace(result, @"(?i)exec\s+@this\.([\w_]+)","exec "+ tname.Substring(0, tname.Length - 1) + "$1\"");
			result = Regex.Replace(result, @"((^)|(\s))@this(($)|(\W))", "$1" + tname + "$4");
			return result;
		}

		private string GetRawBody(){
			if (!string.IsNullOrWhiteSpace(Body) && Body != "NULL") return Body;
			if (!string.IsNullOrWhiteSpace(ExternalBody)){
				return Model.ResolveExternalContent(Definition, ExternalBody);
			}
			if (!string.IsNullOrWhiteSpace(External)){
				return Model.ResolveExternalContent(Definition, External);
			}
			return "";
		}

		/// <summary>
		///     Определяет, является ли объект полностью детерминированным на создание внешним скриптом
		/// </summary>
		/// <returns></returns>
		public bool IsFullyExternal(){
			if (!string.IsNullOrWhiteSpace(Body) && Body != "NULL") return false;
			if (!string.IsNullOrWhiteSpace(ExternalBody)) return false;
			if (!string.IsNullOrWhiteSpace(External)) return true;
			return false;
		}
	}
}