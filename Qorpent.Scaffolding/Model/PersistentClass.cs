using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Wraps persistent B# class to provide data for Sql Schema and Orm generators
	/// </summary>
	public class PersistentClass{
		/// <summary>
		/// </summary>
		public PersistentClass(){
			Fields = new Dictionary<string, Field>();
			ReverseFields = new Dictionary<string, Field>();
			DataTypeMap = DataType.GetDefaultMapping();
			SqlObjects = new List<SqlObject>();
			CSharpInterfaces = new List<string>();
		}

		/// <summary>
		/// </summary>
		public IList<string> CSharpInterfaces { get; private set; }

		/// <summary>
		///     Связанные Sql-объекты
		/// </summary>
		public IList<SqlObject> SqlObjects { get; private set; }

		/// <summary>
		/// Ищет дочерние SQL-объекты по типу и имени
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public SqlObject this[SqlObjectType type,string name]{
			get{
				var objects = SqlObjects.Where(_ => _.Name.ToLowerInvariant() == name.ToLowerInvariant() || _.FullName.ToLowerInvariant() == name.ToLowerInvariant()).ToArray();
				return objects.FirstOrDefault(_ => _.ObjectType == type);
			}
		}

		/// <summary>
		///     Информация о физическом размещении объекта
		/// </summary>
		public AllocationInfo AllocationInfo { get; set; }

		/// <summary>
		///     Комментарий по классу
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		///     Каталог типов данных
		/// </summary>
		public IDictionary<string, DataType> DataTypeMap { get; private set; }

		/// <summary>
		///     Ссылка на исходный класс
		/// </summary>
		public IBSharpClass TargetClass { get; set; }

		/// <summary>
		///     Имя класса/таблицы
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		///     Схема таблицы (для SQL)
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		///     Пространство имен (для C#)
		/// </summary>
		public string Namespace { get; set; }

		/// <summary>
		///     Полное имя для SQL
		/// </summary>
		public string FullSqlName{
			get { return Schema.SqlQuoteName() + "." + Name.SqlQuoteName(); }
		}


		/// <summary>
		///     Полное имя для кодогерации
		/// </summary>
		public string FullCodeName{
			get { return TargetClass.FullName; }
		}

		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Field this[string name]{
			get{
				string n = name.ToLowerInvariant();
				if (Fields.ContainsKey(n)){
					return Fields[n];
				}
				return null;
			}
		}

		/// <summary>
		///     Поля класса/таблицы
		/// </summary>
		public IDictionary<string, Field> Fields { get; private set; }

		/// <summary>
		///     Обратные коллекции
		/// </summary>
		public IDictionary<string, Field> ReverseFields { get; private set; }

		/// <summary>
		///     Связанная модель
		/// </summary>
		public PersistentModel Model { get; set; }

		/// <summary>
		///     Поле - первичный ключ
		/// </summary>
		public Field PrimaryKey{
			get { return Fields.Values.FirstOrDefault(_ => _.IsPrimaryKey); }
		}


		/// <summary>
		/// </summary>
		public int Rank { get; set; }

		/// <summary>
		///     Признак сущности, подлежащей клонированию
		/// </summary>
		public bool Cloneable { get; set; }

		/// <summary>
		///     Признак поддержки резолюции тегов
		/// </summary>
		public bool ResolveAble { get; set; }

		/// <summary>
		///     Настраивает отдельный хранимый класс из B#
		/// </summary>
		/// <param name="c"></param>
		public PersistentClass Setup(IBSharpClass c){
			XElement xml = c.Compiled;
			ReadMainData(c, xml);
			ReadDataTypes(c, xml);
			ReadFields(c, xml);
			ReadAllocationInfo(c, xml);
			ReadInterfaces(c, xml);
			return this;
		}

		private void ReadInterfaces(IBSharpClass c, XElement xml){
			foreach (string e in xml.Elements("implements").Select(_ => _.Attr("code")).Distinct()
			                        .OrderBy(_ => _.StartsWith("I") ? "ZZZ" + _ : _)){
				CSharpInterfaces.Add(e);
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Field[] GetOrderedFields(){
			return Fields.Values.OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public Field[] GetOrderedReverse(){
			return ReverseFields.Values.OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
		}

		private void ReadAllocationInfo(IBSharpClass c, XElement xml){
			var a = new AllocationInfo{MyClass = this};
			AllocationInfo = a;
			a.FileGroupName = xml.Attr("filegroup", a.FileGroupName);
			XElement pt = xml.Element("partitioned");
			if (null != pt){
				a.Partitioned = true;
				a.PartitionFieldName = pt.Attr("with");
				a.PartitioningStart = pt.Attr("start");
			}
		}

		private void ReadFields(IBSharpClass c, XElement xml){
			foreach (XElement e in xml.Elements()){
				string name = e.Name.LocalName;
				if (DataTypeMap.ContainsKey(name) || name == "ref"){
					if (!string.IsNullOrWhiteSpace(e.Value)) continue;
					Field field = new Field().Setup(c, e, this);
					Fields[field.Name.ToLowerInvariant()] = field;
				}
			}
		}

		private void ReadDataTypes(IBSharpClass c, XElement xml){
			foreach (XElement dt in xml.Elements("datatype")){
				DataType dtypeDef = new DataType().Setup(c, dt);
				DataTypeMap[dtypeDef.Code] = dtypeDef;
			}
		}

		private void ReadMainData(IBSharpClass c, XElement xml){
			TargetClass = c;
			Name = c.Name;
			Namespace = c.Namespace;
			Schema = xml.Attr("schema", "dbo").ToLowerInvariant();
			Comment = xml.Attr("name");
			Cloneable = xml.GetSmartValue("cloneable").ToBool();
			ResolveAble = xml.GetSmartValue("resolve").ToBool();
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Field> GetReferences(){
			return Fields.Values.Where(_ => _.IsReference);
		}

		/// <summary>
		///     Возвращает список связанных по внешним ключам таблиц
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PersistentClass> GetReferencedClasses(){
			return GetReferences().Select(_ => _.ReferenceClass).Where(_ => null != _).Distinct();
		}

		/// <summary>
		///     Обходит все дерево зависимостей
		/// </summary>
		/// <param name="visited"></param>
		/// <param name="onlyforeigns"></param>
		/// <returns></returns>
		public IEnumerable<PersistentClass> GetAccessibleClasses(IList<PersistentClass> visited = null,
		                                                         bool onlyforeigns = false){
			visited = visited ?? new List<PersistentClass>();
			foreach (PersistentClass cls in GetReferencedClasses().ToArray()){
				if (onlyforeigns && cls == this) continue;
				if (!visited.Contains(cls)){
					visited.Add(cls);
					yield return cls;
					foreach (PersistentClass ch in cls.GetAccessibleClasses(visited)){
						yield return ch;
					}
				}
			}
		}

		/// <summary>
		/// </summary>
		public void UpgadeRank(int i = 1, IList<PersistentClass> visited = null){
			if (null != visited && visited.Contains(this)) return;
			Rank += i;
			visited = visited ?? new List<PersistentClass>();
			visited.Add(this);
			foreach (PersistentClass dep in GetReferencedClasses()){
				if (dep.Rank == 0) dep.Rank = 1;
				dep.UpgadeRank(i + 1, visited);
			}
		}
	}
}