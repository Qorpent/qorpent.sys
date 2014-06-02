﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Scaffolding.Model.SqlWriters;
using Qorpent.Scaffolding.Sql;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	/// Описывает целостную промежуточную модель данных
	/// </summary>
	public class PersistentModel{
		private PersistentClass[] _cachedTables;

		/// <summary>
		/// 
		/// </summary>
		public PersistentModel(){
			Classes = new Dictionary<string, PersistentClass>();
			DatabaseSqlObjects = new List<SqlObject>();
			Errors = new List<BSharpError>();
			ExtendedScripts = new List<SqlScript>();
			TablePrototype = "dbtable";
			FileGroupPrototype = "filegroup";
			ScriptPrototype = "dbscript";
			GenerationOptions = new GenerationOptions();
		}
		/// <summary>
		/// Настройки генерации продукций
		/// </summary>
		public GenerationOptions GenerationOptions { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="error"></param>
		public void RegisterError(BSharpError error){
			Errors.Add(error);
			Context.RegisterError(error);
		}
		/// <summary>
		/// Прототип дополнительных скриптов
		/// </summary>
		public string ScriptPrototype { get; set; }

		/// <summary>
		/// BS прототип для файловых групп
		/// </summary>
		public string FileGroupPrototype { get; set; }

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
			SetupDefaultScripts();
			foreach (var obj in SqlObject.CreateDatabaseWide(this)){
				DatabaseSqlObjects.Add(obj);
			}
			BuildModel();
			ReadScripts();
			return this;
		}

		private void SetupDefaultScripts(){
			if (GenerationOptions.IncludeDialect.HasFlag(SqlDialect.SqlServer) && GenerationOptions.GenerateCreateScript)
			{
				if (GenerationOptions.Supports(SqlObjectType.FileGroup))
				{
					ExtendedScripts.Add(new SqlScript { Name = "sys:support_for_filegroups_begin", Mode = ScriptMode.Create, SqlDialect = SqlDialect.SqlServer, Position = ScriptPosition.Before, Text = DefaultScripts.SqlServerCreatePeramble });
					ExtendedScripts.Add(new SqlScript { Name = "sys:support_for_filegroups_end", Mode = ScriptMode.Create, SqlDialect = SqlDialect.SqlServer, Position = ScriptPosition.After, Text = DefaultScripts.SqlServerCreateFinisher });
				}
			}
			if (GenerationOptions.IncludeDialect.HasFlag(SqlDialect.PostGres)){
				ExtendedScripts.Add(new SqlScript { Name = "sys:psql_start", Mode = ScriptMode.Create, SqlDialect = SqlDialect.PostGres, Position = ScriptPosition.Before, Text = DefaultScripts.PostgresqlPeramble });
				ExtendedScripts.Add(new SqlScript { Name = "sys:psql_end", Mode = ScriptMode.Create, SqlDialect = SqlDialect.PostGres, Position = ScriptPosition.After, Text = DefaultScripts.PostgresqlFinisher });
				ExtendedScripts.Add(new SqlScript { Name = "sys:psql_start", Mode = ScriptMode.Drop, SqlDialect = SqlDialect.PostGres, Position = ScriptPosition.Before, Text = DefaultScripts.PostgresqlPeramble });
				ExtendedScripts.Add(new SqlScript { Name = "sys:psql_end", Mode = ScriptMode.Drop, SqlDialect = SqlDialect.PostGres, Position = ScriptPosition.After, Text = DefaultScripts.PostgresqlFinisher });
			}
		}



		private void ReadScripts(){
			foreach (var scriptDef in Context.ResolveAll(ScriptPrototype)){
				this.ExtendedScripts.Add(new SqlScript().Setup(this,scriptDef,scriptDef.Compiled));
			}
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
			//обеспечивает целостность расположения таблиц
			SetupAllocation();
			BuildAdvancedObjects();
		}

		private void BuildAdvancedObjects(){
			foreach (var persistentClass in Classes.Values){
				ReadAdvancedDataObjects(persistentClass,persistentClass.TargetClass.Compiled);
			}
		}

		private void SetupAllocation(){
			var allocations = Classes.Values.Select(_ => _.AllocationInfo).ToArray();
			foreach (var allocationInfo in allocations){
				var cls = Context.Get(allocationInfo.FileGroupName);
				if (null != cls && cls.FullName == allocationInfo.FileGroupName){
					allocationInfo.FileGroupName = cls.Compiled.Attr("sqlname", cls.Name).ToUpper();
				}
				if (allocationInfo.Partitioned && null == allocationInfo.PartitionField){
					if (string.IsNullOrWhiteSpace(allocationInfo.PartitionFieldName)){
						ProcessNullPartitionField(allocationInfo.MyClass);
					}
					else{
						var fld = allocationInfo.MyClass[allocationInfo.PartitionFieldName];
						if (null == fld){
							ProcessInvalidPartitionField(allocationInfo.MyClass);
						}
						else{
							allocationInfo.PartitionField = fld;
						}
					}
				}
			}
			
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
					c.Fields["id"] = new Field{Table = c, DataType = c.DataTypeMap["int"], Name = "Id", Idx = 10,IsPrimaryKey =true,IsAutoIncrement = true};
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
						if (reference.ReferenceField=="primarykey" || refcls.Fields.ContainsKey(reference.ReferenceField)){
							var reffld = reference.ReferenceField == "primarykey" ? refcls.PrimaryKey : refcls.Fields[reference.ReferenceField];
							reference.DataType = reffld.DataType;
							reference.ReferenceField = reffld.Name;
							reference.ReferenceClass = refcls;
							reference.DefaultSqlValue = reference.DataType.IsString
								                            ? new DefaultValue{DefaultValueType = DbDefaultValueType.String, Value = "/"}
								                            : new DefaultValue{DefaultValueType = DbDefaultValueType.Native, Value = 0};
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

		private void ProcessNullPartitionField(PersistentClass cls)
		{
			var error = new BSharpError
			{
				Class = cls.TargetClass,
				Level = ErrorLevel.Error,
				Message = "Таблица отмечена к партицированию, но поле не указано"
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}

		private void ProcessInvalidPartitionField(PersistentClass cls)
		{
			var error = new BSharpError
			{
				Class = cls.TargetClass,
				Level = ErrorLevel.Error,
				Message = "Поле, отмеченное к партицироанию не найдено"
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}

		private void ProcessInvalidReferenceError(Field reference){
			var error = new BSharpError{
				Class = reference.Table.TargetClass,
				Level = ErrorLevel.Error,
				Xml = reference.Definition,
				Message =
					"Не могу найти в модели целевой таблицы для поля " + reference.Name + " объекта " +
					reference.Table.FullCodeName + " с ссылкой на " + reference.ReferenceTable
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}
		private void ProcessInvalidFieldReferenceError(Field reference)
		{
			var error = new BSharpError
			{
				Class = reference.Table.TargetClass,
				Level = ErrorLevel.Error,
				Xml = reference.Definition,
				Message =
					"Не могу найти в модели целевого поля таблицы для поля " + reference.Name + " объекта " +
					reference.Table.FullCodeName + " с ссылкой на " + reference.ReferenceTable + " ("+reference.ReferenceField+")"
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

		/// <summary>
		/// Дополнительные SQL - скрипты для построения схемы
		/// </summary>
		public IList<SqlScript> ExtendedScripts { get; private set; }
		/// <summary>
		/// Возваращает все скрипты для указанной позиции и языка в генерации
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="mode"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public IEnumerable<SqlScript> GetScripts(SqlDialect dialect, ScriptMode mode, ScriptPosition position){
			return ExtendedScripts.SelectMany(_ => _.GetRealScripts(dialect, position, mode));
		}
		private void ReadAdvancedDataObjects(PersistentClass cls, XElement xml)
		{
			foreach (var obj in SqlObject.CreateDefaults(cls))
			{
				obj.MyClass = cls;
				cls.SqlObjects.Add(obj);
			}
			foreach (var e in xml.Elements())
			{
				var name = e.Name.LocalName;
				if (name == "ref") continue;
				if (cls.DataTypeMap.ContainsKey(name) && string.IsNullOrWhiteSpace(e.Value)) continue;
				foreach (var obj in SqlObject.Create(cls, e))
				{
					cls.SqlObjects.Add(obj);
				}
			}
		}
		/// <summary>
		/// Получить последовательность генерации
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SqlCommandWriter> GetWriters(SqlDialect dialect, ScriptMode mode){
			if(mode==ScriptMode.Create && !GenerationOptions.GenerateCreateScript)yield break;
			if(mode==ScriptMode.Drop && !GenerationOptions.GenerateDropScript)yield break;
			if(!GenerationOptions.IncludeDialect.HasFlag(dialect))yield break;
			if (mode == ScriptMode.Create){
				foreach (var writer in GetCreateOrderedWriters(dialect)){
					yield return writer;
				}
			}
			else foreach (var writer in GetDropWriters(dialect))
			{
				yield return writer;
			}
		}

		private IEnumerable<SqlCommandWriter> GetDropWriters(SqlDialect dialect){
			return GetCreateOrderedWriters(dialect, ScriptMode.Drop).Reverse();
		}
		/// <summary>
		/// 
		/// </summary>
		protected PersistentClass[] Tables{
			get{
				return _cachedTables ??
				       (_cachedTables = Classes.Values.OrderByDescending(_ => _.Rank).ThenBy(_ => _.Name).ToArray());
			}
		}

		private IEnumerable<SqlCommandWriter> GetCreateOrderedWriters(SqlDialect dialect,ScriptMode mode = ScriptMode.Create){
			foreach (var script in GetScripts(dialect,mode, ScriptPosition.Before)){
				yield return new ScriptWriter(script){
					Model = this,
					Mode = mode,
					Dialect = dialect,
				};
			}
			if (GenerationOptions.Supports(SqlObjectType.FileGroup) && mode==ScriptMode.Create){
				foreach (var fg in DatabaseSqlObjects.OfType<FileGroup>()){
					yield return new FileGroupWriter(fg){
						Model = this,
						Dialect = dialect,
						Mode = mode
					};
				}
			}
			if (GenerationOptions.Supports(SqlObjectType.Schema)){
				foreach (var schema in DatabaseSqlObjects.OfType<Schema>()){
					yield return new SchemaWriter(schema){
						Model = this,
						Dialect = dialect,
						Mode = mode,
						Optional = true
					};
				}
			}
			if (GenerationOptions.Supports(SqlObjectType.Sequence)){
				foreach (var sequence in Tables.SelectMany(_ => _.SqlObjects.OfType<Sequence>())){
					yield return new SequenceWriter(sequence){
						Model = this,
						Dialect = dialect,
						Mode = mode,
						Optional = true,
					};
				}
			}

			if (GenerationOptions.Supports(SqlObjectType.PartitionScheme) && IsSupportPartitioning(dialect))
			{
				foreach (var part in Tables.SelectMany(_ => _.SqlObjects.OfType<PartitionDefinition>()))
				{
					yield return new PartitionDefinitionWriter(part)
					{
						Model = this,
						Dialect = dialect,
						Mode = mode,
						Optional = true,
					};
				}
			}
			if (GenerationOptions.Supports(SqlObjectType.Table)){
				foreach (var script in GetScripts(dialect, mode, ScriptPosition.BeforeTables))
				{
					yield return new ScriptWriter(script)
					{
						Model = this,
						Mode = mode,
						Dialect = dialect,
					};
				}
				foreach (var cls in Tables)
				{
					yield return new TableWriter(cls){
						Model = this,
						Dialect = dialect,
						Mode = mode,
					};

				}
				foreach (var circularRef in Tables.SelectMany(_=>_.Fields.Values.Where(__=>__.GetIsCircular()))){
					yield return new LateForeignKeyWriter(circularRef){
						Model = this,
						Dialect = dialect,
						Mode = mode
					};
				}
				
				foreach (var script in GetScripts(dialect, mode, ScriptPosition.AfterTables))
				{
					yield return new ScriptWriter(script)
					{
						Model = this,
						Mode = mode,
						Dialect = dialect,
					};
				}
			}

			foreach (var script in GetScripts(dialect, mode, ScriptPosition.After))
			{
				yield return new ScriptWriter(script)
				{
					Model = this,
					Mode = mode,
					Dialect = dialect,
				};
			}
			
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public bool IsSupportPartitioning(SqlDialect dialect){
			if (!GenerationOptions.GeneratePartitions) return false;
			return dialect == SqlDialect.SqlServer;

		}
		
	}
}