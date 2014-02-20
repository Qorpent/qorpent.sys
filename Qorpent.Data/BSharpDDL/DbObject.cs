using System;
using System.Collections.Generic;
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
		private const string INDEP = "indep";
		private const string OUTDEP = "outdep";
		private const string SCHEMA = "schema";
		private const string NAME = "name";
		private const string CREATETYPE = "createtype";
		private const string OBJECTTYPE = "objecttype";
		private const string COMMENT = "comment";

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
			return SqlProvider.Get(dialect).GetSql(objarray, mode, hintObject);
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
					if (table == dbField.Table){
						dbField.NoCascadeUpdates = true;
					}else
					if (!dbField.Table.InDependency.Contains(table)){
						dbField.Table.InDependency.Add(table);
						table.OutDependency.Add(dbField.Table);
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
		/// Перекрыть для настройки DBObject из B# класса
		/// </summary>
		/// <param name="sourceClass"></param>
		/// <param name="context"></param>
		protected virtual IEnumerable<DbObject> Setup(IBSharpClass sourceClass, IBSharpContext context=null){
			return Setup(sourceClass.Compiled);
		}
		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected virtual IEnumerable<DbObject> Setup(XElement xml){
			Name = xml.Describe().Code;
			Schema = xml.Attr("schema",Schema);
			Comment = xml.Attr("name",Comment);
			yield break;
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
	}
}
