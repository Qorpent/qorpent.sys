using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.SqlObjects;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Описывает целостную промежуточную модель данных
	/// </summary>
	public class PersistentModel{
		private PersistentClass[] _cachedTables;

		/// <summary>
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
		///     Настройки генерации продукций
		/// </summary>
		public GenerationOptions GenerationOptions { get; set; }

		/// <summary>
		///     Прототип дополнительных скриптов
		/// </summary>
		public string ScriptPrototype { get; set; }

		/// <summary>
		///     BS прототип для файловых групп
		/// </summary>
		public string FileGroupPrototype { get; set; }

		/// <summary>
		///     Имя прототипа для таблиц
		/// </summary>
		public string TablePrototype { get; set; }

		/// <summary>
		///     Признак неправильной модели
		/// </summary>
		public bool IsValid{
			get { return Errors.Count == 0; }
		}

		/// <summary>
		///     Ошибки построения модели
		/// </summary>
		public IList<BSharpError> Errors { get; private set; }

		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public PersistentClass this[string name]{
			get { return Resolve(name); }
		}

		/// <summary>
		///     Ссылка на общий контекст классов B#
		/// </summary>
		public IBSharpContext Context { get; set; }

		/// <summary>
		///     Определения хранимых классов
		/// </summary>
		public IDictionary<string, PersistentClass> Classes { get; private set; }

		/// <summary>
		///     Коллекция объектов уровня БД
		/// </summary>
		public IList<SqlObject> DatabaseSqlObjects { get; private set; }

		/// <summary>
		///     Дополнительные SQL - скрипты для построения схемы
		/// </summary>
		public IList<SqlScript> ExtendedScripts { get; private set; }

		/// <summary>
		/// </summary>
		protected PersistentClass[] Tables{
			get{
				return _cachedTables ??
				       (_cachedTables = Classes.Values.OrderByDescending(_ => _.Rank).ThenBy(_ => _.Name).ToArray());
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="error"></param>
		public void RegisterError(BSharpError error){
			Errors.Add(error);
			Context.RegisterError(error);
		}

		/// <summary>
		///     Конфигурирует модель из B#
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public PersistentModel Setup(IBSharpContext context){
			Context = context;
			IEnumerable<IBSharpClass> tables = Context.ResolveAll(TablePrototype);
			foreach (IBSharpClass table in tables){
				var pclass = new PersistentClass();
				pclass.Setup(table);
				pclass.Model = this;
				Classes[pclass.FullSqlName.ToLowerInvariant()] = pclass;
			}
			SetupDefaultScripts();
			foreach (SqlObject obj in SqlObject.CreateDatabaseWide(this)){
				DatabaseSqlObjects.Add(obj);
			}
			BuildModel();
			ReadScripts();
			return this;
		}


		private void SetupDefaultScripts(){
			if (GenerationOptions.IncludeDialect.HasFlag(SqlDialect.SqlServer) && GenerationOptions.GenerateCreateScript){
				if (GenerationOptions.Supports(SqlObjectType.FileGroup)){
					ExtendedScripts.Add(new SqlScript{
						Name = "sys:support_for_filegroups_begin",
						Mode = ScriptMode.Create,
						SqlDialect = SqlDialect.SqlServer,
						Position = ScriptPosition.Before,
						Text = DefaultScripts.SqlServerCreatePeramble
					});
					ExtendedScripts.Add(new SqlScript{
						Name = "sys:support_for_filegroups_end",
						Mode = ScriptMode.Create,
						SqlDialect = SqlDialect.SqlServer,
						Position = ScriptPosition.After,
						Text = DefaultScripts.SqlServerCreateFinisher
					});
				}
			}
			if (GenerationOptions.IncludeDialect.HasFlag(SqlDialect.PostGres)){
				ExtendedScripts.Add(new SqlScript{
					Name = "sys:psql_start",
					Mode = ScriptMode.Create,
					SqlDialect = SqlDialect.PostGres,
					Position = ScriptPosition.Before,
					Text = DefaultScripts.PostgresqlPeramble
				});
				ExtendedScripts.Add(new SqlScript{
					Name = "sys:psql_end",
					Mode = ScriptMode.Create,
					SqlDialect = SqlDialect.PostGres,
					Position = ScriptPosition.After,
					Text = DefaultScripts.PostgresqlFinisher
				});
				ExtendedScripts.Add(new SqlScript{
					Name = "sys:psql_start",
					Mode = ScriptMode.Drop,
					SqlDialect = SqlDialect.PostGres,
					Position = ScriptPosition.Before,
					Text = DefaultScripts.PostgresqlPeramble
				});
				ExtendedScripts.Add(new SqlScript{
					Name = "sys:psql_end",
					Mode = ScriptMode.Drop,
					SqlDialect = SqlDialect.PostGres,
					Position = ScriptPosition.After,
					Text = DefaultScripts.PostgresqlFinisher
				});
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="definition"></param>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public string ResolveExternalContent(XElement definition, string descriptor){
			string Directory = Path.GetDirectoryName(definition.Attr("file", Environment.CurrentDirectory + "/1.bxls"));
			string filename = Path.Combine(Directory, descriptor);
			if (File.Exists(filename)){
				return File.ReadAllText(filename);
			}
			else{
				RegisterError(new BSharpError{
					Level = ErrorLevel.Error,
					Xml = definition,
					Message = "Не могу найти файл скрипта " + filename
				});
				return "-- ERROR : cannot find file " + filename;
			}
		}


		private void ReadScripts(){
			foreach (IBSharpClass scriptDef in Context.ResolveAll(ScriptPrototype)){
				ExtendedScripts.Add(new SqlScript().Setup(this, scriptDef, scriptDef.Compiled));
			}
		}

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
			foreach (PersistentClass persistentClass in Classes.Values){
				ReadAdvancedDataObjects(persistentClass, persistentClass.TargetClass.Compiled);
			}
		}

		private void SetupAllocation(){
			AllocationInfo[] allocations = Classes.Values.Select(_ => _.AllocationInfo).ToArray();
			foreach (AllocationInfo allocationInfo in allocations){
				IBSharpClass cls = Context.Get(allocationInfo.FileGroupName);
				if (null != cls && cls.FullName == allocationInfo.FileGroupName){
					allocationInfo.FileGroupName = cls.Compiled.Attr("sqlname", cls.Name).ToUpper();
				}
				if (allocationInfo.Partitioned && null == allocationInfo.PartitionField){
					if (string.IsNullOrWhiteSpace(allocationInfo.PartitionFieldName)){
						ProcessNullPartitionField(allocationInfo.MyClass);
					}
					else{
						Field fld = allocationInfo.MyClass[allocationInfo.PartitionFieldName];
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
		///     Компилирует модель из кода B#
		/// </summary>
		/// <param name="codeFiles"></param>
		/// <returns></returns>
		public static PersistentModel Compile(params string[] codeFiles){
			return new PersistentModel().Setup(BSharpCompiler.Compile(codeFiles));
		}

		/// <summary>
		///     Компилирует модель из кода B#
		/// </summary>
		/// <param name="options"></param>
		/// <param name="codeFiles"></param>
		/// <returns></returns>
		public static PersistentModel Compile(GenerationOptions options ,params string[] codeFiles)
		{
			return new PersistentModel{GenerationOptions = options}.Setup(BSharpCompiler.Compile(codeFiles));
		}

		private void SetupPrimaryKeys(){
			PersistentClass[] nonkeyd = Classes.Values.Where(_ => null == _.PrimaryKey).ToArray();
			foreach (PersistentClass c in nonkeyd){
				if (c.Fields.ContainsKey("id")){
					c.Fields["id"].IsPrimaryKey = true;
					c.Fields["id"].IsAutoIncrement = true;
				}
				else{
					c.Fields["id"] = new Field{
						Table = c,
						DataType = c.DataTypeMap["int"],
						Name = "Id",
						Idx = 10,
						IsPrimaryKey = true,
						IsAutoIncrement = true
					};
				}
			}
		}

		private void DetermineCircularLinks(){
			foreach (Field referense in GetAllReferences().ToArray()){
				referense.GetIsCircular();
			}
		}

		private void SetClassRanks(){
			PersistentClass[] classes = Classes.Values.OrderBy(_ => _.FullSqlName).ToArray();
			foreach (PersistentClass c in classes){
				if (c.Rank == 0){
					c.UpgadeRank();
				}
			}
		}

		private void SetupClassLinks(){
			foreach (Field reference in GetAllReferences()){
				if (null == reference.ReferenceClass){
					PersistentClass refcls = Resolve(reference.ReferenceTable);
					if (null != refcls){
						if (reference.ReferenceField == "primarykey" || refcls.Fields.ContainsKey(reference.ReferenceField)){
							Field reffld = reference.ReferenceField == "primarykey"
								               ? refcls.PrimaryKey
								               : refcls.Fields[reference.ReferenceField];
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

		private void ProcessNullPartitionField(PersistentClass cls){
			var error = new BSharpError{
				Class = cls.TargetClass,
				Level = ErrorLevel.Error,
				Message = "Таблица отмечена к партицированию, но поле не указано"
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}

		private void ProcessInvalidPartitionField(PersistentClass cls){
			var error = new BSharpError{
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

		private void ProcessInvalidFieldReferenceError(Field reference){
			var error = new BSharpError{
				Class = reference.Table.TargetClass,
				Level = ErrorLevel.Error,
				Xml = reference.Definition,
				Message =
					"Не могу найти в модели целевого поля таблицы для поля " + reference.Name + " объекта " +
					reference.Table.FullCodeName + " с ссылкой на " + reference.ReferenceTable + " (" + reference.ReferenceField + ")"
			};
			Errors.Add(error);
			Context.RegisterError(error);
		}

		/// <summary>
		///     Находит таблицу по ссылке
		/// </summary>
		/// <param name="reference"></param>
		/// <returns></returns>
		public PersistentClass Resolve(string reference){
			string normalized = reference.ToLowerInvariant().SqlQuoteName();
			//первый приоритет - поиск по полному имени таблицы
			if (Classes.ContainsKey(normalized)) return Classes[normalized];
			PersistentClass byfullname = Classes.Values.FirstOrDefault(_ => _.FullCodeName.ToLowerInvariant() == normalized);
			if (null != byfullname) return byfullname;
			PersistentClass bypartialname =
				Classes.Values.FirstOrDefault(_ => _.Name.SqlQuoteName().ToLowerInvariant() == normalized);
			return bypartialname;
		}

		/// <summary>
		///     Возвращает все ссылочные поля
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Field> GetAllReferences(){
			return Classes.Values.SelectMany(_ => _.GetReferences());
		}

		/// <summary>
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public string GetScript(SqlDialect dialect, ScriptMode mode){
			var sb = new StringBuilder();
			foreach (SqlCommandWriter sw in GetWriters(dialect, mode)){
				if (null == sw) continue;
				string subresult = sw.ToString();
				if (!string.IsNullOrWhiteSpace(subresult)){
					sb.AppendLine(subresult);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public string GetDigest(SqlDialect dialect, ScriptMode mode){
			var sb = new StringBuilder();
			foreach (SqlCommandWriter sw in GetWriters(dialect, mode)){
				if (null == sw) continue;
				string subresult = sw.GetDigest();
				if (!string.IsNullOrWhiteSpace(subresult)){
					sb.AppendLine(subresult);
				}
			}
			return sb.ToString();
		}

		/// <summary>
		///     Возваращает все скрипты для указанной позиции и языка в генерации
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="mode"></param>
		/// <param name="position"></param>
		/// <returns></returns>
		public IEnumerable<SqlScript> GetScripts(SqlDialect dialect, ScriptMode mode, ScriptPosition position){
			return ExtendedScripts.SelectMany(_ => _.GetRealScripts(dialect, position, mode));
		}

		private void ReadAdvancedDataObjects(PersistentClass cls, XElement xml){
			foreach (SqlObject obj in SqlObject.CreateDefaults(cls)){
				obj.Table = cls;
				cls.SqlObjects.Add(obj);
			}
			foreach (XElement e in xml.Elements()){
				string name = e.Name.LocalName;
				if (name == "ref") continue;
				if (cls.DataTypeMap.ContainsKey(name) && string.IsNullOrWhiteSpace(e.Value)) continue;
				foreach (SqlObject obj in SqlObject.Create(cls, e)){
					cls.SqlObjects.Add(obj);
				}
			}
		}

		/// <summary>
		///     Получить последовательность генерации
		/// </summary>
		/// <returns></returns>
		public IEnumerable<SqlCommandWriter> GetWriters(SqlDialect dialect, ScriptMode mode){
			if (mode == ScriptMode.Create && !GenerationOptions.GenerateCreateScript) yield break;
			if (mode == ScriptMode.Drop && !GenerationOptions.GenerateDropScript) yield break;
			if (!GenerationOptions.IncludeDialect.HasFlag(dialect)) yield break;
			var factory = new SqlCommandWriterFactory{Mode = mode, Dialect = dialect, Model = this};
			IEnumerable<object> objset = mode == ScriptMode.Create ? GetCreateOrderedWriters(dialect) : GetDropWriters(dialect);
			foreach (SqlCommandWriter w in factory.Get(objset)){
				yield return w;
			}
		}

		private IEnumerable<object> GetDropWriters(SqlDialect dialect){
			return GetCreateOrderedWriters(dialect, ScriptMode.Drop).Reverse();
		}

		private IEnumerable<object> GetCreateOrderedWriters(SqlDialect dialect, ScriptMode mode = ScriptMode.Create){
			foreach (SqlScript script in GetScripts(dialect, mode, ScriptPosition.Before)){
				yield return script;
			}
			foreach (FileGroup fg in DatabaseSqlObjects.OfType<FileGroup>()){
				yield return fg;
			}
			foreach (Schema schema in DatabaseSqlObjects.OfType<Schema>()){
				yield return schema;
			}
			foreach (Sequence sequence in Tables.SelectMany(_ => _.SqlObjects.OfType<Sequence>())){
				yield return sequence;
			}
			foreach (PartitionDefinition part in Tables.SelectMany(_ => _.SqlObjects.OfType<PartitionDefinition>())){
				yield return part;
			}

			foreach (SqlScript script in GetScripts(dialect, mode, ScriptPosition.BeforeTables)){
				yield return script;
			}
			foreach (PersistentClass cls in Tables){
				yield return cls;
			}
			foreach (Field circularRef in Tables.SelectMany(_ => _.Fields.Values.Where(__ => __.GetIsCircular()))){
				yield return circularRef;
			}

			foreach (SqlScript script in GetScripts(dialect, mode, ScriptPosition.AfterTables)){
				yield return script;
			}

			foreach (SqlFunction function in Tables.SelectMany(_ => _.SqlObjects.OfType<SqlFunction>())){
				yield return function;
			}
			foreach (SqlView view in Tables.SelectMany(_ => _.SqlObjects.OfType<SqlView>())){
				yield return view;
			}
			foreach (SqlTrigger trigger in Tables.SelectMany(_ => _.SqlObjects.OfType<SqlTrigger>())){
				yield return trigger;
			}


			foreach (SqlScript script in GetScripts(dialect, mode, ScriptPosition.After)){
				yield return script;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		public bool IsSupportPartitioning(SqlDialect dialect){
			if (!GenerationOptions.GeneratePartitions) return false;
			return dialect == SqlDialect.SqlServer;
		}
	}
}