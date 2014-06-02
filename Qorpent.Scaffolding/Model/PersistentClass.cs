using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model
{
	/// <summary>
	/// Wraps persistent B# class to provide data for Sql Schema and Orm generators
	/// </summary>
	public class PersistentClass
	{
		/// <summary>
		/// Настраивает отдельный хранимый класс из B#
		/// </summary>
		/// <param name="c"></param>
		public PersistentClass Setup(IBSharpClass c){
			var xml = c.Compiled;
			ReadMainData(c, xml);
			ReadDataTypes(c,xml);
			ReadFields(c,xml);
			ReadAllocationInfo(c, xml);
			return this;
		}



		private void ReadAllocationInfo(IBSharpClass c, XElement xml){
			var a = new AllocationInfo{MyClass = this};
			this.AllocationInfo = a;
			a.FileGroupName = xml.Attr("filegroup", a.FileGroupName);
			var pt = xml.Element("partitioned");
			if (null != pt){
				a.Partitioned = true;
				a.PartitionFieldName = pt.Attr("with");
				a.PartitioningStart = pt.Attr("start");
			}
		}

		
		/// <summary>
		/// Связанные Sql-объекты
		/// </summary>
		public IList<SqlObject> SqlObjects { get; private set; }
		/// <summary>
		/// Информация о физическом размещении объекта
		/// </summary>
		public AllocationInfo AllocationInfo { get; set; }

		private void ReadFields(IBSharpClass c, XElement xml){
			foreach (var e in xml.Elements()){
				var name = e.Name.LocalName;
				if (DataTypeMap.ContainsKey(name) || name == "ref"){
					if(!string.IsNullOrWhiteSpace(e.Value))continue;
					var field = new Field().Setup(c, e,this);
					Fields[field.Name.ToLowerInvariant()] = field;
				}
			}
		}

		private void ReadDataTypes(IBSharpClass c, XElement xml){
			foreach (var dt in xml.Elements("datatype")){
				var dtypeDef = new DataType().Setup(c,dt);
				DataTypeMap[dtypeDef.Code] = dtypeDef;
			}
		}

		private void ReadMainData(IBSharpClass c, XElement xml){
			this.TargetClass = c;
			this.Name = c.Name;
			this.Namespace = c.Namespace;
			this.Schema = xml.Attr("schema", "dbo").ToLowerInvariant();
			this.Comment = xml.Attr("name");
		}

		/// <summary>
		/// Комментарий по классу
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public PersistentClass(){
			Fields = new Dictionary<string, Field>();
			ReverseFields = new Dictionary<string, Field>();
			DataTypeMap = DataType.GetDefaultMapping();
			SqlObjects = new List<SqlObject>();
		}
		/// <summary>
		/// Каталог типов данных
		/// </summary>
		public IDictionary<string, DataType> DataTypeMap { get; private set; }

		/// <summary>
		/// Ссылка на исходный класс
		/// </summary>
		public IBSharpClass TargetClass { get; set; }
		/// <summary>
		/// Имя класса/таблицы
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Схема таблицы (для SQL)
		/// </summary>
		public string Schema { get; set; }
		/// <summary>
		/// Пространство имен (для C#)
		/// </summary>
		public string Namespace { get; set; }
		/// <summary>
		/// Полное имя для SQL
		/// </summary>
		public string FullSqlName{
			get { return Schema + "." + Name; }
		}
		/// <summary>
		/// Полное имя для кодогерации
		/// </summary>
		public string FullCodeName{
			get { return TargetClass.FullName; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Field this[string name]
		{
			get{
				var n = name.ToLowerInvariant();
				if (Fields.ContainsKey(n)){
					return Fields[n];
				}
				return null;
			}
		}
		/// <summary>
		/// Поля класса/таблицы
		/// </summary>
		public IDictionary<string, Field> Fields { get; private set; }
		/// <summary>
		/// Обратные коллекции
		/// </summary>
		public IDictionary<string, Field> ReverseFields { get; private set; }
		/// <summary>
		/// Связанная модель
		/// </summary>
		public PersistentModel Model { get; set; }
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Field> GetReferences(){
			return Fields.Values.Where(_ => _.IsReference);
		} 
		/// <summary>
		/// Возвращает список связанных по внешним ключам таблиц
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PersistentClass> GetReferencedClasses(){
			return GetReferences().Select(_ => _.ReferenceClass).Where(_ => null != _).Distinct();
		}
		/// <summary>
		/// Поле - первичный ключ
		/// </summary>
		public Field PrimaryKey{
			get { return Fields.Values.FirstOrDefault(_ => _.IsPrimaryKey); }
		}



		/// <summary>
		/// Обходит все дерево зависимостей
		/// </summary>
		/// <param name="visited"></param>
		/// <param name="onlyforeigns"></param>
		/// <returns></returns>
		public IEnumerable<PersistentClass> GetAccessibleClasses(IList<PersistentClass> visited = null, bool onlyforeigns = false){
			visited = visited ?? new List<PersistentClass>();
			foreach (var cls in GetReferencedClasses().ToArray()){
				if(onlyforeigns && cls ==this)continue;
				if (!visited.Contains(cls)){
					visited.Add(cls);
					yield return cls;
					foreach (var ch in cls.GetAccessibleClasses(visited)){
						yield return ch;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int Rank { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public void UpgadeRank(int i = 1, IList<PersistentClass> visited = null){
			if (null != visited && visited.Contains(this)) return;
			Rank += i;
			visited = visited ?? new List<PersistentClass>();
			visited.Add(this);
			foreach (var dep in GetReferencedClasses()){
				if (dep.Rank == 0) dep.Rank = 1;
				dep.UpgadeRank(i + 1, visited);
			}
		}
	}
}
