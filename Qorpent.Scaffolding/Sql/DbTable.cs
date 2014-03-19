using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Описывает таблицы
	/// </summary>
	public class DbTable : DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbTable(){
			ObjectType = DbObjectType.Table;
		}
		private const string FIELDS = "fields";
		private const string TYPES = "types";
		private const string REQUIREDEFAULTRECORD = "requiredefaultrecord";
		/// <summary>
		/// Входящие зависимости
		/// </summary>
		public IDictionary<string,DbField> Fields
		{
			get
			{
				var result = Get<IDictionary<string,DbField>>(FIELDS);
				if (null == result){
					Set(FIELDS, result = new Dictionary<string, DbField>());
				}
				return result;
			}
			set
			{
				Set(FIELDS, value);
			}
		}
		/// <summary>
		/// Признак необходимости наличия дефолтной записи для внешних ключей
		/// </summary>
		public bool RequireDefaultRecord{
			get { return Get<bool>(REQUIREDEFAULTRECORD); }
			set {Set(REQUIREDEFAULTRECORD,value);}
		}

		/// <summary>
		/// Добавить поле в список
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public DbTable AddField(DbField field){
			field.Table = this;
			Fields[field.Name] = field;
			return this;
		}


		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected override IEnumerable<DbObject> Setup(System.Xml.Linq.XElement xml)
		{
			foreach (var adv in base.Setup(xml).ToArray()){
				yield return adv;
			}
			var types_ = xml.Elements("datatype").Select(DbDataType.Create);
			this.Types = new Dictionary<string,DbDataType>();
			foreach (var t in types_){
				if (!(Types.ContainsKey(t.Code)))
				{
					Types[t.Code] = t;
				}
			}
			var deftypes = DbDataType.GetDefaultTypes();
			foreach (var dbDataType in deftypes){
				if (!Types.ContainsKey(dbDataType.Key))
				{
					Types[dbDataType.Key] = dbDataType.Value;
				}
			}
			foreach (var e in xml.Elements()){
				if (e.Name == "function" || e.Name=="void"){
					yield return new DbFunction().Initialize(this,e);
					continue;
				}
				if (e.Name == "partitioned"){
					var dbpart = new DbAutoPartition().Initialize(this, e);
					dbpart.Name = this.Name + "_AUTO_SPLIT_PARTITION";
					yield return dbpart;
					var dbscheme = new DbPartitionScheme().Initialize(this, e);
					dbscheme.Name = this.Name + "_PARTITION";
					dbscheme.InDependency.Clear();
					dbscheme.OutDependency.Add(this);
					InDependency.Add(dbscheme);
					yield return dbscheme;
					this.PartitionScheme = (DbPartitionScheme)dbscheme;
					continue;
				}
				if (e.Name == "trigger")
				{
					yield return new DbTrigger().Initialize(this, e);
					continue;
				}
				if (e.Name == "view")
				{
					yield return new DbView().Initialize(this, e);
					continue;
				}
				var n = e.Name.LocalName;
				DbDataType type = null;
				if (Types.ContainsKey(n))
				{
					type = Types[n];
				}
				if (null != type){
					if (!string.IsNullOrWhiteSpace(e.Value) || e.Attributes().Any(_ => _.Name.LocalName.StartsWith("@"))){ //marks 'methods'
						yield return new DbFunction().Initialize(this, e);
						continue;
					}
					else{

						var code = e.Attr("code");
						if (!Fields.ContainsKey(code)){
							AddField(DbField.CreateField(this, type, e));
						}
					}
				}else if (n == "ref"){
					var code = e.Attr("code");
					type = new DbDataType{Code="ref"};
					if (!Fields.ContainsKey(code))
					{
						AddField(DbField.CreateField(this, type, e));
					}
				}
				
			}
			yield break;
		}
		/// <summary>
		/// Схема партицирования
		/// </summary>
		public DbPartitionScheme PartitionScheme { get; set; }


		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, DbDataType> Types{
			get{
				var result = Get<IDictionary<string, DbDataType>>(TYPES);
				if (null == result){
					Set(TYPES, result = new Dictionary<string, DbDataType>());
				}
				return result;
			}
			set{
				Set(TYPES, value);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public DbFileGroup FileGroup { get; set; }
		/// <summary>
		/// Проверяет внешние ключи, которые требуется сформировать позже в сейфовом режиме
		/// </summary>
		/// <param name="tables"></param>
		public void CheckLateReferences(DbTable[] tables){
			var fkeys = Fields.Values.Where(_ => _.IsRef).ToArray();
			var thisidx = Array.IndexOf(tables, this);
			var requirecheck = tables.Skip(thisidx + 1).ToArray();
			foreach (var fkey in fkeys){
				if (requirecheck.Any(_ => _.FullName == fkey.RefTable)){
					fkey.IsLateRef = true;
				}
			}
		}
	}
}
