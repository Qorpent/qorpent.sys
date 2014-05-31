using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// Описывает целостную промежуточную модель данных
	/// </summary>
	public class PersistentModel{
		/// <summary>
		/// 
		/// </summary>
		public PersistentModel(){
			Classes = new Dictionary<string, PersistentClass>();
			DatabaseSqlObjects = new List<SqlObject>();
			Errors = new List<BSharpError>();
			TablePrototype = "dbtable";
		}
		/// <summary>
		/// Имя прототипа для таблиц
		/// </summary>
		public string TablePrototype { get; set; }

		/// <summary>
		/// Конфигурирует модель из B#
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public PersistentModel Setup(IBSharpContext context){
			Context = context;
			var tables = Context.ResolveAll(TablePrototype);
			foreach (var table in tables){
				var pclass = new PersistentClass();
				pclass.Setup(table);
				pclass.Model = this;
				Classes[pclass.FullSqlName.ToLowerInvariant()] = pclass;
			}
			foreach (var obj in SqlObject.CreateDatabaseWide(this)){
				DatabaseSqlObjects.Add(obj);
			}
			BuildModel();
			return this;
		}
		/// <summary>
		/// Признак неправильной модели
		/// </summary>
		public bool IsValid { 
			get { return Errors.Count == 0; }
		}
		/// <summary>
		/// Ошибки построения модели
		/// </summary>
		public IList<BSharpError> Errors { get;  private  set; } 
		private void BuildModel(){
			//устанавливаем поля Id по умолчанию - если их нет и нет первичных ключей, добавляем. Если есть но не отмечены как ключ и нет другого ключа-выставляем
			SetupPrimaryKeys();
			//сначала просто связываем внешние ключи по полям
			SetupClassLinks();
			//теперь устанавливаем правильный порядок таблиц по зависимостям
			SetClassRanks();
			//теперь выявляем циркулярные ссылки
			DetermineCircularLinks();
			
		}
		/// <summary>
		/// Компилирует модель из кода B#
		/// </summary>
		/// <param name="codeFiles"></param>
		/// <returns></returns>
		public  static PersistentModel Compile(params string[] codeFiles)
		{
			return new PersistentModel().Setup(BSharpCompiler.Compile(codeFiles));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public PersistentClass this[string name]{
			get { return Resolve(name); }
		}

		private void SetupPrimaryKeys(){
			var nonkeyd = Classes.Values.Where(_ => null == _.PrimaryKey).ToArray();
			foreach (var c in nonkeyd){
				if (c.Fields.ContainsKey("id")){
					c.Fields["id"].IsPrimaryKey = true;
					c.Fields["id"].IsAutoIncrement = true;
				}
				else{
					c.Fields["id"] = new Field{MyClass = c, DataType = c.DataTypeMap["int"], Name = "Id", Idx = 10,IsPrimaryKey =true,IsAutoIncrement = true};
				}
			}
		}

		private void DetermineCircularLinks(){
			foreach (var referense in GetAllReferences().ToArray()){
				referense.GetIsCircular();
			}
		}

		private void SetClassRanks(){
			var classes = Classes.Values.OrderBy(_=>_.FullSqlName).ToArray();
			foreach (var c in classes){
				if (c.Rank == 0){
					c.UpgadeRank();
				}
			}
		}

		private void SetupClassLinks(){
			foreach (var reference in GetAllReferences()){
				if (null == reference.ReferenceClass){
					var refcls = Resolve(reference.ReferenceTable);
					if (null != refcls){
						if (refcls.Fields.ContainsKey(reference.ReferenceField)){
							var reffld = refcls.Fields[reference.ReferenceField];
							reference.DataType = reffld.DataType;
							reference.ReferenceClass = refcls;
							refcls.ReverseFields[reference.ReverseCollectionName] = reference;
						}
						else{
							ProcessInvalidFieldReferenceError(reference);
						}
					}
					else{
						ProcessInvalidReferenceError(reference);
					}
				}
			}
		}

		private void ProcessInvalidReferenceError(Field reference){
			var error = new BSharpError{
				Class = reference.MyClass.TargetClass,
				Level = ErrorLevel.Error,
				Xml = reference.Definition,
				Message =
					"Не могу найти в модели целевой таблицы для поля " + reference.Name + " объекта " +
					reference.MyClass.FullCodeName + " с ссылкой на " + reference.ReferenceTable
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}
		private void ProcessInvalidFieldReferenceError(Field reference)
		{
			var error = new BSharpError
			{
				Class = reference.MyClass.TargetClass,
				Level = ErrorLevel.Error,
				Xml = reference.Definition,
				Message =
					"Не могу найти в модели целевого поля таблицы для поля " + reference.Name + " объекта " +
					reference.MyClass.FullCodeName + " с ссылкой на " + reference.ReferenceTable + " ("+reference.ReferenceField+")"
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}

		/// <summary>
		/// Находит таблицу по ссылке
		/// </summary>
		/// <param name="reference"></param>
		/// <returns></returns>
		public PersistentClass Resolve(string reference){
			var ignorecase = reference.ToLowerInvariant();
			//первый приоритет - поиск по полному имени таблицы
			if (Classes.ContainsKey(ignorecase)) return Classes[ignorecase];
			var byfullname = Classes.Values.FirstOrDefault(_ => _.FullCodeName.ToLowerInvariant() == ignorecase);
			if (null != byfullname) return byfullname;
			var bypartialname = Classes.Values.FirstOrDefault(_ => _.Name.ToLowerInvariant() == ignorecase);
			return bypartialname;
		}
		/// <summary>
		/// Возвращает все ссылочные поля
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Field> GetAllReferences(){
			return Classes.Values.SelectMany(_ => _.GetReferences());
		}

		/// <summary>
		/// Ссылка на общий контекст классов B#
		/// </summary>
		public IBSharpContext Context { get; set; }

		/// <summary>
		/// Определения хранимых классов
		/// </summary>
		public IDictionary<string,PersistentClass> Classes { get; private set; }
		/// <summary>
		/// Коллекция объектов уровня БД
		/// </summary>
		public IList<SqlObject> DatabaseSqlObjects { get; private set; } 
	}
}