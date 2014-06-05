using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.SqlObjects{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SqlObject{
		private string _name;

		/// <summary>
		/// 
		/// </summary>
		public SqlObject(){
			Schema = "dbo";
		}
		/// <summary>
		/// Тип объекта
		/// </summary>
		public SqlObjectType ObjectType { get; set; }
		/// <summary>
		/// Ссылка на класс-контейнер
		/// </summary>
		public PersistentClass Table { get; set; }
		/// <summary>
		/// B# с определением
		/// </summary>
		public IBSharpClass BSClass { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public XElement Definition { get; set; }
		/// <summary>
		/// Признак объекта, который должен формироваться до определения таблицы
		/// </summary>
		public bool PreTable { get; set; }

		/// <summary>
		/// Формирует SQL-объект
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
				this.External = xml.GetSmartValue("external");
				this.ExternalBody = xml.GetSmartValue("externalbody");
				this.Dialect = SqlDialect.Ansi;
				var dialect = xml.GetSmartValue("dialect");
				if (!string.IsNullOrWhiteSpace(dialect))
				{
					this.Dialect = dialect.To<SqlDialect>();
				}
				this.Body = xml.Value;
			}
			
			return this;
		}
		/// <summary>
		/// Комментарий
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// Обратная ссылка на модель
		/// </summary>
		protected PersistentModel Model { get; set; }

		/// <summary>
		/// Имя SQL
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
		/// Схема SQL
		/// </summary>
		public string Schema { get; set; }
		/// <summary>
		/// Полное имя SQL
		/// </summary>
		public string FullName{
			get{
				if (string.IsNullOrWhiteSpace(Schema)){
					Schema = "dbo";
				}
				return Schema + "." + Name;
			}
		}

		/// <summary>
		/// Требование к именам - иметь верхний регистр
		/// </summary>
		public bool UpperCase { get; set; }

		/// <summary>
		/// Требование чтобы имена были в нижнем регистре
		/// </summary>
		public bool LowerCase { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Body { get; set; }

		/// <summary>
		/// Диалект
		/// </summary>
		public SqlDialect Dialect { get; set; }

		/// <summary>
		/// Файл определением (только тело)
		/// </summary>
		public string ExternalBody { get; set; }

		/// <summary>
		/// Файл с внешним (полным) определением
		/// </summary>
		public string External { get; set; }

		/// <summary>
		/// Формирует глобальные объекты уровня базы данных
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDatabaseWide(PersistentModel model){
			foreach (var schema in GenerateSchemas(model)){
				yield return schema;
			}
			foreach (var fgroup in GenerateFileGroups(model)){
				yield return fgroup;
			}
			
			
		}

		private static IEnumerable<SqlObject> GenerateSchemas(PersistentModel model){
			foreach (var schema in model.Classes.Values.Select(_ => _.Schema.ToLowerInvariant()).Distinct().ToArray())
			{
				if ("dbo" == schema) continue;
				yield return new Schema{Name = schema.ToLowerInvariant()};
			}
		}

		private static IEnumerable<SqlObject> GenerateFileGroups(PersistentModel model){
			var fgs = new Dictionary<string, FileGroup>();
			foreach (var fgd in model.Context.ResolveAll(model.FileGroupPrototype)){
				FgFromClass(model, fgd, fgs);
			}
			foreach (var pcls in model.Classes.Values){
				var _pcls = model.Context.Get(pcls.AllocationInfo.FileGroupName);
				pcls.AllocationInfo.FileGroupName = pcls.AllocationInfo.FileGroupName.ToUpper();
				
				if (null != _pcls ){
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
			var hasdef = fgs.Values.Any(_ => _.IsDefault);
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
		/// Формирует стандартные объекты для таблицы
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDefaults(PersistentClass cls){
			if (cls.PrimaryKey.IsAutoIncrement){
				yield return new Sequence().Setup(null, cls, null, null);
			}
			if (cls.Model.GenerationOptions.GeneratePartitions && cls.AllocationInfo.Partitioned){
				yield return new PartitionDefinition().Setup(null, cls, null, null);
			}
		}
		/// <summary>
		/// Формирует специальные объекты, определенные в таблице, конкретный элемент
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> Create(PersistentClass cls, XElement e){
				var name = e.Name.LocalName;
				if (name == "trigger"){
					yield return new SqlTrigger().Setup(null, cls, null, e);
				}else if (name == "view"){
					yield return new SqlView().Setup(null, cls, null, e);
				}else if (name == "function" || name=="void" || !string.IsNullOrWhiteSpace(e.Value) ){
					yield return new SqlFunction().Setup(null, cls, null, e);
				}
			
		}

		/// <summary>
		/// Возвращает контент
		/// </summary>
		/// <returns></returns>
		public string ResolveBody(){
			var result = GetRawBody().Trim();
			if (result.StartsWith("(") && result.EndsWith(")")){
				result = result.Substring(1, result.Length - 2).Trim();
			}
			return result;
		}

		private string GetRawBody(){
			if (!string.IsNullOrWhiteSpace(Body) && Body != "NULL") return Body;
			if (!string.IsNullOrWhiteSpace(ExternalBody)){
				return Model.ResolveExternalContent(this.Definition, ExternalBody);
			}
			if (!string.IsNullOrWhiteSpace(External)){
				return Model.ResolveExternalContent(this.Definition, External);
			}
			return "print 'NO BODY'";
		}

		/// <summary>
		/// Определяет, является ли объект полностью детерминированным на создание внешним скриптом
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