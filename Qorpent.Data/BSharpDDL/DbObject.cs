using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.BSharpDDL
{
	/// <summary>
	/// Определение DDL объекта
	/// </summary>
	public abstract class DbObject : TreeConfigBase<DbObject>{
		/// <summary>
		/// Признак генерации из готового файла SQL
		/// </summary>
		public bool IsRawSql { get; set; }
		/// <summary>
		/// "Сырой SQL"
		/// </summary>
		public string RawSql { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected IDictionary<string, DbDataType> _types;
		private const string INDEP = "indep";
		private const string OUTDEP = "outdep";
		private const string SCHEMA = "schema";
		private const string NAME = "name";
		private const string CREATETYPE = "createtype";
		private const string OBJECTTYPE = "objecttype";
		private const string COMMENT = "comment";
		private const string BODY = "body";
		/// <summary>
		/// 
		/// </summary>
		public string FullName{
			get { return Schema + "." + Name; }
		}

		/// <summary>
		/// Возвращает SQL строку в заданном диалекте
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="dialect"></param>
		/// <param name="hintObject"></param>
		/// <returns></returns>
		public virtual string GetSql(DbGenerationMode mode = DbGenerationMode.Script, DbDialect dialect = DbDialect.TSQL, object hintObject = null){
			return SqlProvider.Get(dialect).GetSql(this, mode, hintObject);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="allobjects"></param>
		/// <param name="mode"></param>
		/// <param name="dialect"></param>
		/// <param name="hintObject"></param>
		/// <returns></returns>
		public static string GetSql(IEnumerable<DbObject> allobjects, DbGenerationMode mode = DbGenerationMode.Script,
		                            DbDialect dialect = DbDialect.TSQL, object hintObject = null){

			var objarray = allobjects.ToArray();
			SetupForeignKeyDependency(objarray);
			SetupFileGroups(objarray);
			return SqlProvider.Get(dialect).GetSql(objarray, mode, hintObject);
		}

		private static void SetupFileGroups(DbObject[] objarray){
			var filegroups = objarray.OfType<DbFileGroup>().ToDictionary(_=>_.Name,_=>_);
			var tables = objarray.OfType<DbTable>().ToArray();
			foreach (var dbTable in tables){
				if (dbTable.FileGroupName == "PRIMARY") continue;
				
				if (!string.IsNullOrWhiteSpace(dbTable.FileGroupName)){
					var name = dbTable.FileGroupName.SmartSplit(false, true, '.').Last();
					if (!filegroups.ContainsKey(name)){
						throw new Exception("cannot find group " + name + " for " + dbTable.Name);
					}
					var fgroup = filegroups[name];
					dbTable.FileGroupName = fgroup.Name;
					dbTable.FileGroup = fgroup;
				}
				else{
					dbTable.FileGroupName = "PRIMARY";
				}
			}

		}

		private static void SetupForeignKeyDependency(DbObject[] objarray){
			var fkeys =
				objarray.OfType<DbTable>().SelectMany(_ => _.Fields.Values).Where(_ => _.IsRef).ToArray();
			foreach (var dbField in fkeys){
				var table = objarray.OfType<DbTable>().FirstOrDefault(_ =>{
					var simplified = dbField.RefTable.Replace("[", "").Replace("]", "");
					if (_.Name == simplified) return true;
					if ((_.Schema + "." + _.Name) == simplified) return true;
					return false;
				});
				if (null != table){
					dbField.RefTable = table.Schema + "." + table.Name;
					table.RequireDefaultRecord = true;
					//if (dbField.DataType.Code == "ref"){
						var realfld = table.Fields[dbField.RefField];
						dbField.DataType = realfld.DataType;
					//}
					if (table == dbField.Table){
						dbField.NoCascadeUpdates = true;
					}else
					if (!dbField.Table.InDependency.Contains(table)){
						dbField.Table.InDependency.Add(table);
						table.OutDependency.Add(dbField.Table);
					}
					if (dbField.DataType.DbType == DbType.String){
						dbField.DefaultValue = new DbDefaultValue{Value = "/",DefaultValueType=DbDefaultValueType.String};
					}
				}
			}
		}

		/// <summary>
		/// Входящие зависимости
		/// </summary>
		public IList<DbObject> InDependency{
			get {
				var result = Get<IList<DbObject>>(INDEP); 
				if (null == result){
					Set(INDEP,result = new List<DbObject>()); 
				}
				return result;
			}
			set{
				Set(INDEP,value);
			}
		}

		/// <summary>
		/// Исходящие зависимости
		/// </summary>
		public IList<DbObject> OutDependency
		{
			get
			{
				var result = Get<IList<DbObject>>(OUTDEP);
				if (null == result)
				{
					Set(OUTDEP, result = new List<DbObject>());
				}
				return result;
			}
			set
			{
				Set(OUTDEP, value);
			}
		}
		/// <summary>
		/// Схема
		/// </summary>
		public string Schema{
			get { return Get<string>(SCHEMA); }
			set {  Set(SCHEMA,value); }
		}
		/// <summary>
		/// Имя
		/// </summary>
		public string Name
		{
			get { return Get<string>(NAME); }
			set { Set(NAME, value); }
		}
		/// <summary>
		/// Имя
		/// </summary>
		public string Comment
		{
			get { return Get<string>(COMMENT); }
			set { Set(COMMENT, value); }
		}
		/// <summary>
		/// Тип объекта
		/// </summary>
		public DbObjectType ObjectType{
			get { return Get<DbObjectType>(OBJECTTYPE); } 
			protected set { Set(OBJECTTYPE,value);}
		}
		/// <summary>
		/// Тип создания объекта
		/// </summary>
		public DbObjectCreateType CreateType{
			get { return Get(CREATETYPE, DbObjectCreateType.Ensure); }
			set { Set(CREATETYPE,value); }
		}

		/// <summary>
		/// Тело объекта
		/// </summary>
		public string Body{
			get { return Get<string>(BODY); }
			set { Set(BODY,value); }
		}

		/// <summary>
		/// Перекрыть для настройки DBObject из B# класса
		/// </summary>
		/// <param name="sourceClass"></param>
		/// <param name="context"></param>
		protected virtual IEnumerable<DbObject> Setup(IBSharpClass sourceClass, IBSharpContext context=null){
			this.SourceClass = sourceClass;
			this.Context = context;
			return Setup(sourceClass.Compiled);
			
		}
		/// <summary>
		/// 
		/// </summary>
		protected IBSharpContext Context { get; set; }

		/// <summary>
		/// 
		/// </summary>
		protected IBSharpClass SourceClass { get; set; }

		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected virtual IEnumerable<DbObject> Setup(XElement xml){
			Name = xml.Describe().Code;
			Schema = xml.Attr("schema",Schema);
			if (string.IsNullOrWhiteSpace(Schema)){
				Schema = xml.Parent.Attr("schema");
			}
			Comment = xml.Attr("name",Comment);
			SetupFileGroup(xml);
			if (xml.Attr("external").ToBool()){
				ReadExternalSql(Path.GetDirectoryName(xml.ResolveAttr("file")),xml.Attr("external"));
			}
			var body = xml.Value;
			if (body.StartsWith("("))
			{
				body = body.Substring(1, body.Length - 2);
			}
			Body = body;
			yield break;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		private void SetupFileGroup(XElement xml){
			var filegroup = xml.Attr("filegroup");
			FileGroupName = filegroup;
		}
		/// <summary>
		/// Имя файловой группы
		/// </summary>
		public string FileGroupName { get; set; }

		private void ReadExternalSql(string dir, string attr){
			var name = this.GetType().Name + "_" + Schema + "_" + Name + ".sql";
			if (attr != "1"){
				name = attr;
			}
			var file = Path.Combine(dir, name);
			if (!File.Exists(file)){
				throw new Exception("File "+file+" required for "+this.Schema+"."+this.Name+" not found");
			}
			var sql = File.ReadAllText(file);
			this.IsRawSql = true;
			this.RawSql = sql;
		}


		/// <summary>
		/// Создать объект из B# класса
		/// </summary>
		/// <param name="sourceClass"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IEnumerable<DbObject> Create(IBSharpClass sourceClass, IBSharpContext context=null){
			DbObject result = null;
			if (sourceClass.Prototype == "dbtable")
			{
				result = new DbTable();
			}
			else if (sourceClass.Prototype == "filegroup"){
				result = new DbFileGroup();
			}

			if (null != result){
				var advanced = result.Setup(sourceClass).ToArray();
				yield return result;
				foreach (var a in advanced){
					yield return a;
				}
			}
			else{
				throw new Exception("cannot create DBObject for this sourceClass "+sourceClass.Name+" "+sourceClass.Prototype);
			}
			
		}
		/// <summary>
		/// Фабричный класс создания объектов из XML
		/// </summary>
		/// <param name="sourceClass"></param>
		/// <returns></returns>
		public static DbObject Create(XElement sourceClass){
			DbObject result = null;
			if (sourceClass.Attr("prototype") == "dbtable"){
				result =  new DbTable();
			}
			if (null != result){
				result.Setup(sourceClass);
			}
			else
			{
				throw new Exception("cannot create DBObject for this XML");
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		public virtual void SetParent(DbObject obj){
			this.ParentElement = obj;
			InDependency.Add(obj);
			obj.OutDependency.Add(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tab"></param>	
		/// <param name="e"></param>
		/// <returns></returns>
		public DbObject Initialize(DbTable tab, XElement e){
			this._types = tab.Types;
			SetParent(tab);
			
			Setup(e).ToArray();
			this.Schema = tab.Schema;
			return this;
		}
	}
}
