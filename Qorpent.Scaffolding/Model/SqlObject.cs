using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// 
	/// </summary>
	public abstract class SqlObject{
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
		public PersistentClass MyClass { get; set; }
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
		public virtual void Setup(PersistentModel model, PersistentClass cls, IBSharpClass bscls, XElement xml){
			MyClass = cls;
			Model = model;
			BSClass = bscls;
			Definition = xml;
			Name = xml.ChooseAttr("sqlname", "code");
			Comment = xml.Attr("name");

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
		public string Name { get; set; }
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
		/// Формирует глобальные объекты уровня базы данных
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDatabaseWide(PersistentModel model){
			foreach (var fgroup in GenerateFileGroups(model)){
				yield return fgroup;
			}
		}

		private static IEnumerable<SqlObject> GenerateFileGroups(PersistentModel model){
			var fgs = new Dictionary<string, FileGroup>();
			foreach (var fgd in model.Context.ResolveAll(model.FileGroupPrototype)){
				var fg = new FileGroup();
				fg.Setup(model,null,fgd,fgd.Compiled);
				fg.Name = fg.Name.ToUpper();
				fgs[fg.Name.ToUpper()] = fg;
			}
			foreach (var pcls in model.Classes.Values){
				pcls.AllocationInfo.FileGroupName = pcls.AllocationInfo.FileGroupName.ToUpper();
				if (!fgs.ContainsKey(pcls.AllocationInfo.FileGroupName)){
					fgs[pcls.AllocationInfo.FileGroupName] = new FileGroup{Name = pcls.AllocationInfo.FileGroupName};
				}
				pcls.AllocationInfo.FileGroup = fgs[pcls.AllocationInfo.FileGroupName];
			}
			if (!fgs.ContainsKey("SECONDARY")){
				fgs["SECONDARY"] = new FileGroup{Name = "SECONDARY", IsDefault = true};
			}
			var hasdef = fgs.Values.Any(_ => _.IsDefault);
			if (!hasdef){
				fgs["SECONDARY"].IsDefault = true;
			}
			return fgs.Values;
		}

		/// <summary>
		/// Формирует стандартные объекты для таблицы
		/// </summary>
		/// <param name="cls"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> CreateDefaults(PersistentClass cls){
			yield break;
		}
		/// <summary>
		/// Формирует специальные объекты, определенные в таблице, конкретный элемент
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static IEnumerable<SqlObject> Create(PersistentClass cls, XElement e){
			yield break;
		}
	}
}